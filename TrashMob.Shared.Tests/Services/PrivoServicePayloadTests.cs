namespace TrashMob.Shared.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Moq.Protected;
    using TrashMob.Models;
    using TrashMob.Services;
    using Xunit;

    /// <summary>
    /// Tests that PRIVO API request payloads match the expected format from the
    /// PRIVO Postman collection (Planning/Projects/TrashMob.postman_collection.json).
    /// </summary>
    public class PrivoServicePayloadTests
    {
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
        private readonly Mock<ILogger<PrivoService>> logger = new();
        private string capturedRequestBody;
        private string capturedRequestUrl;

        public PrivoServicePayloadTests()
        {
            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Privo:BaseUrl"] = "https://consent-svc-int.privo.com",
                    ["Privo:ServiceIdentifier"] = "trashmobservice",
                    ["Privo-ClientId"] = "testClientId",
                    ["Privo-ClientSecret"] = "testClientSecret",
                })
                .Build();

            // Pre-cache a token so we skip the token request
            memoryCache.Set("Privo_AccessToken", "test-token", TimeSpan.FromMinutes(25));
        }

        private PrivoService CreateServiceWithCapture(HttpStatusCode responseCode = HttpStatusCode.OK, string responseBody = null)
        {
            responseBody ??= JsonSerializer.Serialize(new
            {
                principal_identifiers = new { sid = "principal-sid-123", eid = "eid-123" },
                granter_identifiers = new { sid = "granter-sid-456", eid = "eid-456" },
                consent_identifier = "consent-id-789",
                consent_url = "https://consent-svc-int.privo.com/consent/789",
            });

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
                {
                    capturedRequestUrl = req.RequestUri?.ToString();
                    capturedRequestBody = req.Content?.ReadAsStringAsync(CancellationToken.None).Result;
                    return new HttpResponseMessage(responseCode)
                    {
                        Content = new StringContent(responseBody),
                    };
                });

            var httpClient = new HttpClient(handler.Object);
            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(f => f.CreateClient("Privo")).Returns(httpClient);

            return new PrivoService(factory.Object, configuration, memoryCache, logger.Object);
        }

        [Fact]
        public async Task AdultVerification_PayloadMatchesPostmanFormat()
        {
            var service = CreateServiceWithCapture();
            var user = new User
            {
                Id = Guid.Parse("ee1b2ad7-fd41-40b5-a056-d157e815c2d8"),
                GivenName = "Mickey",
                Surname = "Mouse",
                Email = "mickey@example.com",
                DateOfBirth = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero),
            };

            await service.CreateAdultVerificationRequestAsync(user, CancellationToken.None);

            Assert.NotNull(capturedRequestBody);
            Assert.Contains("/trashmobservice/requests", capturedRequestUrl);

            using var doc = JsonDocument.Parse(capturedRequestBody);
            var root = doc.RootElement;

            // Top-level structure: granter, locale, principal
            Assert.True(root.TryGetProperty("granter", out var granter));
            Assert.True(root.TryGetProperty("locale", out var locale));
            Assert.True(root.TryGetProperty("principal", out var principal));

            // Locale
            Assert.Equal("en-US", locale.GetString());

            // Granter: email + notifications
            Assert.Equal("mickey@example.com", granter.GetProperty("email").GetString());
            var notifications = granter.GetProperty("notifications");
            Assert.Equal(JsonValueKind.Array, notifications.ValueKind);
            Assert.Equal(2, notifications.GetArrayLength());
            Assert.Equal("consent_request_email", notifications[0].GetProperty("notification_type").GetString());
            Assert.False(notifications[0].GetProperty("is_on").GetBoolean()); // Suppressed for adult self-verification
            Assert.Equal("consent_approved_email", notifications[1].GetProperty("notification_type").GetString());

            // Principal: given_name, birthdate, birthdate_precision, email, email_verified, eid, attributes
            Assert.Equal("Mickey", principal.GetProperty("given_name").GetString());
            Assert.Equal("19700101", principal.GetProperty("birthdate").GetString());
            Assert.Equal("yyyymmdd", principal.GetProperty("birthdate_precision").GetString());
            Assert.Equal("mickey@example.com", principal.GetProperty("email").GetString());
            Assert.True(principal.GetProperty("email_verified").GetBoolean());
            Assert.Equal("ee1b2ad7-fd41-40b5-a056-d157e815c2d8", principal.GetProperty("eid").GetString());

            // Attributes: family name
            var attributes = principal.GetProperty("attributes");
            Assert.Equal(JsonValueKind.Array, attributes.ValueKind);
            Assert.Equal(1, attributes.GetArrayLength());
            Assert.Equal("trashmobservice_att_granter_family_name", attributes[0].GetProperty("attribute_identifier").GetString());
            var valueArray = attributes[0].GetProperty("value");
            Assert.Equal(JsonValueKind.Array, valueArray.ValueKind);
            Assert.Equal("Mouse", valueArray[0].GetString());
        }

        [Fact]
        public async Task AdultVerification_BirthdateFormat_NoSeparators()
        {
            var service = CreateServiceWithCapture();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                DateOfBirth = new DateTimeOffset(1985, 12, 25, 0, 0, 0, TimeSpan.Zero),
            };

            await service.CreateAdultVerificationRequestAsync(user, CancellationToken.None);

            using var doc = JsonDocument.Parse(capturedRequestBody);
            var birthdate = doc.RootElement.GetProperty("principal").GetProperty("birthdate").GetString();

            // Must be yyyyMMdd with no separators
            Assert.Equal("19851225", birthdate);
            Assert.DoesNotContain("-", birthdate);
        }

        [Fact]
        public async Task AdultVerification_MissingBirthdate_ReturnsNull()
        {
            var service = CreateServiceWithCapture();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                DateOfBirth = null,
            };

            var result = await service.CreateAdultVerificationRequestAsync(user, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task ParentChildConsent_PayloadMatchesPostmanFormat()
        {
            var service = CreateServiceWithCapture();
            var parent = new User
            {
                Id = Guid.NewGuid(),
                Email = "parent@example.com",
                PrivoSid = "parent-privo-sid",
            };
            var child = new Dependent
            {
                Id = Guid.Parse("aabbccdd-1122-3344-5566-778899001122"),
                FirstName = "Kevin",
                LastName = "Mouse",
                DateOfBirth = new DateOnly(2013, 1, 19),
            };

            await service.CreateParentInitiatedChildConsentAsync(parent, child, CancellationToken.None);

            Assert.NotNull(capturedRequestBody);

            using var doc = JsonDocument.Parse(capturedRequestBody);
            var root = doc.RootElement;

            // Granter
            var granter = root.GetProperty("granter");
            Assert.Equal("parent@example.com", granter.GetProperty("email").GetString());
            Assert.Equal("parent-privo-sid", granter.GetProperty("sid").GetString());
            Assert.True(granter.TryGetProperty("notifications", out var notifications));
            Assert.Equal("consent_request_email", notifications[0].GetProperty("notification_type").GetString());

            // Locale
            Assert.Equal("en-US", root.GetProperty("locale").GetString());

            // Principal
            var principal = root.GetProperty("principal");
            Assert.Equal("Kevin", principal.GetProperty("given_name").GetString());
            Assert.Equal("20130119", principal.GetProperty("birthdate").GetString());
            Assert.Equal("yyyymmdd", principal.GetProperty("birthdate_precision").GetString());
            Assert.Equal("aabbccdd-1122-3344-5566-778899001122", principal.GetProperty("eid").GetString());
        }

        [Fact]
        public async Task ChildInitiatedConsent_PayloadMatchesPostmanFormat()
        {
            var service = CreateServiceWithCapture();

            await service.CreateChildInitiatedConsentAsync(
                "Alex", "alex@example.com", new DateOnly(2012, 6, 15), "parent@example.com", CancellationToken.None);

            Assert.NotNull(capturedRequestBody);

            using var doc = JsonDocument.Parse(capturedRequestBody);
            var root = doc.RootElement;

            // Granter
            var granter = root.GetProperty("granter");
            Assert.Equal("parent@example.com", granter.GetProperty("email").GetString());
            Assert.True(granter.TryGetProperty("notifications", out _));

            // Locale
            Assert.Equal("en-US", root.GetProperty("locale").GetString());

            // Principal
            var principal = root.GetProperty("principal");
            Assert.Equal("Alex", principal.GetProperty("given_name").GetString());
            Assert.Equal("alex@example.com", principal.GetProperty("email").GetString());
            Assert.Equal("20120615", principal.GetProperty("birthdate").GetString());
            Assert.Equal("yyyymmdd", principal.GetProperty("birthdate_precision").GetString());
        }

        [Fact]
        public async Task AdultVerification_ResponseParsesCorrectly()
        {
            var responseJson = JsonSerializer.Serialize(new
            {
                principal_identifiers = new { sid = "p-sid-111", eid = "p-eid-222" },
                granter_identifiers = new { sid = "g-sid-333", eid = "g-eid-444" },
                consent_identifier = "consent-555",
                consent_url = "https://consent-svc-int.privo.com/consent/555",
            });

            var service = CreateServiceWithCapture(HttpStatusCode.OK, responseJson);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                DateOfBirth = new DateTimeOffset(1990, 1, 1, 0, 0, 0, TimeSpan.Zero),
            };

            var result = await service.CreateAdultVerificationRequestAsync(user, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("p-sid-111", result.Sid);
            Assert.Equal("g-sid-333", result.GranterSid);
            Assert.Equal("consent-555", result.ConsentIdentifier);
            Assert.Equal("https://consent-svc-int.privo.com/consent/555", result.ConsentUrl);
        }

        [Fact]
        public async Task RequestUrl_IncludesServiceIdentifier()
        {
            var service = CreateServiceWithCapture();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                DateOfBirth = new DateTimeOffset(1990, 1, 1, 0, 0, 0, TimeSpan.Zero),
            };

            await service.CreateAdultVerificationRequestAsync(user, CancellationToken.None);

            Assert.Equal(
                "https://consent-svc-int.privo.com/s2s/api/v1.0/trashmobservice/requests",
                capturedRequestUrl);
        }
    }
}
