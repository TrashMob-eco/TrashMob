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

    public class PartnerDocumentsV2ControllerTests
    {
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IPartnerDocumentManager> documentManager = new();
        private readonly Mock<IPartnerDocumentStorageManager> storageManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<PartnerDocumentsV2Controller>> logger = new();
        private readonly PartnerDocumentsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public PartnerDocumentsV2ControllerTests()
        {
            controller = new PartnerDocumentsV2Controller(
                partnerManager.Object,
                documentManager.Object,
                storageManager.Object,
                authorizationService.Object,
                logger.Object);

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
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var documents = new List<PartnerDocument>
            {
                new() { Id = Guid.NewGuid(), PartnerId = partnerId, Name = "Doc 1", DocumentTypeId = 1 },
                new() { Id = Guid.NewGuid(), PartnerId = partnerId, Name = "Doc 2", DocumentTypeId = 2 },
            };

            partnerManager.Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            SetupAuthSuccess();
            documentManager.Setup(m => m.GetByParentIdAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(documents);

            var result = await controller.GetByPartner(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<PartnerDocumentDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task GetByPartner_Unauthorized_ReturnsForbid()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };

            partnerManager.Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var result = await controller.GetByPartner(partnerId, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Get_Found_ReturnsOk()
        {
            var documentId = Guid.NewGuid();
            var document = new PartnerDocument
            {
                Id = documentId,
                PartnerId = Guid.NewGuid(),
                Name = "Test Document",
                DocumentTypeId = 1,
                ContentType = "application/pdf",
            };

            documentManager.Setup(m => m.GetAsync(documentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(document);

            var result = await controller.Get(documentId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PartnerDocumentDto>(okResult.Value);
            Assert.Equal("Test Document", dto.Name);
        }

        [Fact]
        public async Task Get_NotFound_ReturnsNotFound()
        {
            var documentId = Guid.NewGuid();

            documentManager.Setup(m => m.GetAsync(documentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerDocument)null);

            var result = await controller.Get(documentId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Add_Authorized_ReturnsCreated()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var dto = new PartnerDocumentDto
            {
                PartnerId = partnerId,
                Name = "New Document",
                DocumentTypeId = 1,
                Url = "https://example.com/doc.pdf",
            };
            var created = new PartnerDocument
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                Name = "New Document",
                DocumentTypeId = 1,
                Url = "https://example.com/doc.pdf",
            };

            partnerManager.Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            SetupAuthSuccess();
            documentManager
                .Setup(m => m.AddAsync(It.IsAny<PartnerDocument>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            var result = await controller.Add(dto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var resultDto = Assert.IsType<PartnerDocumentDto>(createdResult.Value);
            Assert.Equal("New Document", resultDto.Name);
        }

        [Fact]
        public async Task Delete_Authorized_ReturnsNoContent()
        {
            var documentId = Guid.NewGuid();
            var partner = new Partner { Id = Guid.NewGuid(), Name = "Test Partner" };
            var document = new PartnerDocument
            {
                Id = documentId,
                PartnerId = partner.Id,
                Name = "Doc to Delete",
                BlobStoragePath = "partners/doc.pdf",
            };

            documentManager
                .Setup(m => m.GetPartnerForDocument(documentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            SetupAuthSuccess();
            documentManager
                .Setup(m => m.GetAsync(documentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(document);
            storageManager
                .Setup(m => m.DeleteDocumentAsync(document.BlobStoragePath, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            documentManager
                .Setup(m => m.DeleteAsync(documentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.Delete(documentId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            storageManager.Verify(
                m => m.DeleteDocumentAsync(document.BlobStoragePath, It.IsAny<CancellationToken>()), Times.Once);
            documentManager.Verify(
                m => m.DeleteAsync(documentId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_Unauthorized_ReturnsForbid()
        {
            var documentId = Guid.NewGuid();
            var partner = new Partner { Id = Guid.NewGuid(), Name = "Test Partner" };

            documentManager
                .Setup(m => m.GetPartnerForDocument(documentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var result = await controller.Delete(documentId, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Delete_NoBlobPath_SkipsStorageDeletion()
        {
            var documentId = Guid.NewGuid();
            var partner = new Partner { Id = Guid.NewGuid(), Name = "Test Partner" };
            var document = new PartnerDocument
            {
                Id = documentId,
                PartnerId = partner.Id,
                Name = "External URL Doc",
                BlobStoragePath = null,
            };

            documentManager
                .Setup(m => m.GetPartnerForDocument(documentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            SetupAuthSuccess();
            documentManager
                .Setup(m => m.GetAsync(documentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(document);
            documentManager
                .Setup(m => m.DeleteAsync(documentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.Delete(documentId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            storageManager.Verify(
                m => m.DeleteDocumentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
