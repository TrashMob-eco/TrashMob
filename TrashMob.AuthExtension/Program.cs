using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

var tenantId = builder.Configuration["AuthExtension:TenantId"];
var clientId = builder.Configuration["AuthExtension:ClientId"];
var allowedAppId = builder.Configuration["AuthExtension:AllowedAppId"]
    ?? "99045fe1-7639-4a75-9d4a-577b6ca3810f";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
        options.Audience = clientId;
        options.TokenValidationParameters.ValidateIssuer = true;
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("EntraAuthExtension", policy =>
        policy.RequireClaim("azp", allowedAppId));

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live");

app.MapPost("/api/authext/attributecollectionsubmit",
    async (HttpContext context, ILogger<Program> logger) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    logger.LogInformation("OnAttributeCollectionSubmit event received");

    try
    {
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        // Navigate to data.userSignUpInfo.attributes
        if (!root.TryGetProperty("data", out var data) ||
            !data.TryGetProperty("userSignUpInfo", out var signUpInfo) ||
            !signUpInfo.TryGetProperty("attributes", out var attributes))
        {
            logger.LogWarning("Request missing expected attribute structure, allowing sign-up");
            return Results.Json(ContinueResponse());
        }

        // Search for dateOfBirth attribute (handles both built-in and extension_ prefix)
        string? dobValue = null;
        foreach (var attr in attributes.EnumerateObject())
        {
            if (attr.Name.Contains("dateOfBirth", StringComparison.OrdinalIgnoreCase) ||
                attr.Name.Contains("DateOfBirth", StringComparison.OrdinalIgnoreCase))
            {
                if (attr.Value.TryGetProperty("value", out var val))
                {
                    dobValue = val.GetString();
                }

                break;
            }
        }

        if (string.IsNullOrEmpty(dobValue))
        {
            logger.LogInformation("No dateOfBirth attribute found, allowing sign-up");
            return Results.Json(ContinueResponse());
        }

        if (!DateTimeOffset.TryParse(dobValue, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dob))
        {
            logger.LogWarning("Could not parse dateOfBirth '{DobValue}', allowing sign-up", dobValue);
            return Results.Json(ContinueResponse());
        }

        var age = CalculateAge(dob.Date);
        logger.LogInformation("Calculated age: {Age} from DOB {Dob}", age, dob.Date.ToString("yyyy-MM-dd"));

        if (age < 13)
        {
            logger.LogInformation("Blocking under-13 sign-up (age {Age})", age);
            return Results.Json(BlockResponse());
        }

        logger.LogInformation("Age verification passed (age {Age}), allowing sign-up", age);
        return Results.Json(ContinueResponse());
    }
    catch (JsonException ex)
    {
        logger.LogError(ex, "Failed to parse request body");
        return Results.Json(ContinueResponse());
    }
}).RequireAuthorization("EntraAuthExtension");

app.Run();

static int CalculateAge(DateTime dob)
{
    var today = DateTime.Today;
    var age = today.Year - dob.Year;
    if (dob.Date > today.AddYears(-age))
    {
        age--;
    }

    return age;
}

static Dictionary<string, object> ContinueResponse() => new()
{
    ["data"] = new Dictionary<string, object>
    {
        ["@odata.type"] = "microsoft.graph.onAttributeCollectionSubmitResponseData",
        ["actions"] = new object[]
        {
            new Dictionary<string, object>
            {
                ["@odata.type"] = "microsoft.graph.attributeCollectionSubmit.continueWithDefaultBehavior"
            }
        }
    }
};

static Dictionary<string, object> BlockResponse() => new()
{
    ["data"] = new Dictionary<string, object>
    {
        ["@odata.type"] = "microsoft.graph.onAttributeCollectionSubmitResponseData",
        ["actions"] = new object[]
        {
            new Dictionary<string, object>
            {
                ["@odata.type"] = "microsoft.graph.attributeCollectionSubmit.showBlockPage",
                ["title"] = "Age Requirement",
                ["message"] = "You must be 13 or older to join TrashMob."
            }
        }
    }
};
