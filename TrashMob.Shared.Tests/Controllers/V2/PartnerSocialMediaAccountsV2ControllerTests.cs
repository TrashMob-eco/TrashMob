namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class PartnerSocialMediaAccountsV2ControllerTests
    {
        private readonly Mock<IPartnerSocialMediaAccountManager> socialMediaAccountManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<PartnerSocialMediaAccountsV2Controller>> logger = new();
        private readonly PartnerSocialMediaAccountsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public PartnerSocialMediaAccountsV2ControllerTests()
        {
            controller = new PartnerSocialMediaAccountsV2Controller(
                socialMediaAccountManager.Object, partnerManager.Object,
                authorizationService.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        private void SetupAuthSuccess()
        {
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
        }

        [Fact]
        public async Task GetByPartner_Authorized_ReturnsOkWithList()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var accounts = new List<PartnerSocialMediaAccount>
            {
                new() { Id = Guid.NewGuid(), PartnerId = partnerId, AccountIdentifier = "@partner_twitter", SocialMediaAccountTypeId = 1 },
                new() { Id = Guid.NewGuid(), PartnerId = partnerId, AccountIdentifier = "partner_insta", SocialMediaAccountTypeId = 2 },
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            socialMediaAccountManager
                .Setup(m => m.GetByParentIdAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(accounts);

            var result = await controller.GetByPartner(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<PartnerSocialMediaAccountDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task GetByPartner_PartnerNotFound_Returns404()
        {
            var partnerId = Guid.NewGuid();

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.GetByPartner(partnerId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Get_Found_ReturnsOk()
        {
            var accountId = Guid.NewGuid();
            var account = new PartnerSocialMediaAccount
            {
                Id = accountId,
                PartnerId = Guid.NewGuid(),
                AccountIdentifier = "@partner",
                SocialMediaAccountTypeId = 1,
            };

            socialMediaAccountManager
                .Setup(m => m.GetAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            var result = await controller.Get(accountId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PartnerSocialMediaAccountDto>(okResult.Value);
            Assert.Equal(accountId, dto.Id);
            Assert.Equal("@partner", dto.AccountIdentifier);
        }

        [Fact]
        public async Task Get_NotFound_Returns404()
        {
            var accountId = Guid.NewGuid();

            socialMediaAccountManager
                .Setup(m => m.GetAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerSocialMediaAccount)null);

            var result = await controller.Get(accountId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Add_Authorized_ReturnsOk()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var dto = new PartnerSocialMediaAccountDto
            {
                PartnerId = partnerId,
                AccountIdentifier = "@new_account",
                SocialMediaAccountTypeId = 1,
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            socialMediaAccountManager
                .Setup(m => m.AddAsync(It.IsAny<PartnerSocialMediaAccount>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PartnerSocialMediaAccount
                {
                    Id = Guid.NewGuid(),
                    PartnerId = partnerId,
                    AccountIdentifier = "@new_account",
                    SocialMediaAccountTypeId = 1,
                });

            var result = await controller.Add(dto, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultDto = Assert.IsType<PartnerSocialMediaAccountDto>(okResult.Value);
            Assert.Equal("@new_account", resultDto.AccountIdentifier);
        }

        [Fact]
        public async Task Add_PartnerNotFound_Returns404()
        {
            var dto = new PartnerSocialMediaAccountDto
            {
                PartnerId = Guid.NewGuid(),
                AccountIdentifier = "@account",
                SocialMediaAccountTypeId = 1,
            };

            partnerManager
                .Setup(m => m.GetAsync(dto.PartnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.Add(dto, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Authorized_ReturnsNoContent()
        {
            SetupAuthSuccess();

            var accountId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();
            var account = new PartnerSocialMediaAccount
            {
                Id = accountId,
                PartnerId = partnerId,
                AccountIdentifier = "@delete_me",
                SocialMediaAccountTypeId = 1,
            };
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };

            socialMediaAccountManager
                .Setup(m => m.GetAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            socialMediaAccountManager
                .Setup(m => m.DeleteAsync(accountId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            var result = await controller.Delete(accountId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_AccountNotFound_Returns404()
        {
            var accountId = Guid.NewGuid();

            socialMediaAccountManager
                .Setup(m => m.GetAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerSocialMediaAccount)null);

            var result = await controller.Delete(accountId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
