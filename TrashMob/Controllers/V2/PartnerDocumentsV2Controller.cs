namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for managing partner documents, including file upload, download, and admin operations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/partner-documents")]
    [Authorize]
    public class PartnerDocumentsV2Controller(
        IKeyedManager<Partner> partnerManager,
        IPartnerDocumentManager documentManager,
        IPartnerDocumentStorageManager storageManager,
        IAuthorizationService authorizationService,
        ILogger<PartnerDocumentsV2Controller> logger) : ControllerBase
    {
        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "image/png",
            "image/jpeg",
        };

        private const long MaxFileSizeBytes = 25 * 1024 * 1024; // 25 MB
        private const long MaxPartnerStorageBytes = 500 * 1024 * 1024; // 500 MB

        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all documents for a given partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of partner documents.</returns>
        /// <response code="200">Returns the document list.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpGet("by-partner/{partnerId}")]
        [ProducesResponseType(typeof(IEnumerable<PartnerDocumentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByPartner(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetByPartner Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var documents = await documentManager.GetByParentIdAsync(partnerId, cancellationToken);

            return Ok(documents.Select(d => d.ToV2Dto()));
        }

        /// <summary>
        /// Gets a partner document by its unique identifier.
        /// </summary>
        /// <param name="id">The partner document ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The partner document.</returns>
        /// <response code="200">Returns the document.</response>
        /// <response code="404">Document not found.</response>
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(PartnerDocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerDocument Document={DocumentId}", id);

            var document = await documentManager.GetAsync(id, cancellationToken);

            if (document is null)
            {
                return NotFound();
            }

            return Ok(document.ToV2Dto());
        }

        /// <summary>
        /// Adds a new partner document (metadata only, for external URL documents).
        /// </summary>
        /// <param name="dto">The partner document to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created document.</returns>
        /// <response code="201">Document created.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerDocumentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Add([FromBody] PartnerDocumentDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddPartnerDocument Partner={PartnerId}", dto.PartnerId);

            var partner = await partnerManager.GetAsync(dto.PartnerId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = dto.ToEntity();
            var result = await documentManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result.ToV2Dto());
        }

        /// <summary>
        /// Uploads a document file to Azure Blob Storage and creates the document metadata.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="name">The document name.</param>
        /// <param name="documentTypeId">The document type identifier.</param>
        /// <param name="expirationDate">Optional expiration date.</param>
        /// <param name="formFile">The file to upload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Document uploaded and created.</response>
        /// <response code="400">Invalid file or storage limit exceeded.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpPost("upload")]
        [RequestSizeLimit(26_214_400)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerDocumentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Upload(
            [FromForm] Guid partnerId,
            [FromForm] string name,
            [FromForm] int documentTypeId,
            [FromForm] DateTimeOffset? expirationDate,
            IFormFile formFile,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UploadPartnerDocument Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            if (formFile is null || formFile.Length == 0)
            {
                return Problem("No file provided.", statusCode: StatusCodes.Status400BadRequest);
            }

            if (formFile.Length > MaxFileSizeBytes)
            {
                return Problem(
                    $"File size exceeds the maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)} MB.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            if (!AllowedContentTypes.Contains(formFile.ContentType))
            {
                return Problem(
                    $"File type '{formFile.ContentType}' is not allowed. Allowed types: PDF, Word, Excel, PNG, JPEG.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            var currentUsage = await storageManager.GetPartnerStorageUsageBytesAsync(partnerId, cancellationToken);

            if (currentUsage + formFile.Length > MaxPartnerStorageBytes)
            {
                return Problem(
                    $"Uploading this file would exceed the partner storage limit of {MaxPartnerStorageBytes / (1024 * 1024)} MB.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            var document = new PartnerDocument
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                Name = name,
                ContentType = formFile.ContentType,
                FileSizeBytes = formFile.Length,
                DocumentTypeId = documentTypeId,
                ExpirationDate = expirationDate,
            };

            await documentManager.AddAsync(document, UserId, cancellationToken);

            var blobPath = await storageManager.UploadDocumentAsync(partnerId, document.Id, formFile, cancellationToken);
            document.BlobStoragePath = blobPath;
            await documentManager.UpdateAsync(document, UserId, cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = document.Id }, document.ToV2Dto());
        }

        /// <summary>
        /// Generates a time-limited download URL for a partner document.
        /// </summary>
        /// <param name="id">The partner document ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the download URL.</response>
        /// <response code="400">Document has no uploaded file.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpGet("{id}/download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DownloadPartnerDocument Document={DocumentId}", id);

            var partner = await documentManager.GetPartnerForDocument(id, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var document = await documentManager.GetAsync(id, cancellationToken);

            if (string.IsNullOrWhiteSpace(document?.BlobStoragePath))
            {
                return Problem("This document has no uploaded file.", statusCode: StatusCodes.Status400BadRequest);
            }

            var downloadUrl = await storageManager.GetDownloadUrlAsync(document.BlobStoragePath, cancellationToken);

            return Ok(new { downloadUrl });
        }

        /// <summary>
        /// Gets storage usage in bytes for a partner, including the total limit.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the storage usage information.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpGet("storage-usage/{partnerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetStorageUsage(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetStorageUsage Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var usageBytes = await storageManager.GetPartnerStorageUsageBytesAsync(partnerId, cancellationToken);

            return Ok(new { usageBytes, limitBytes = MaxPartnerStorageBytes });
        }

        /// <summary>
        /// Updates an existing partner document.
        /// </summary>
        /// <param name="dto">The partner document to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated document.</returns>
        /// <response code="200">Document updated.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerDocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update([FromBody] PartnerDocumentDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdatePartnerDocument Document={DocumentId}", dto.Id);

            var partner = await partnerManager.GetAsync(dto.PartnerId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = dto.ToEntity();
            var result = await documentManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a partner document by its unique identifier, including any uploaded blob.
        /// </summary>
        /// <param name="id">The partner document ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Document deleted.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpDelete("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeletePartnerDocument Document={DocumentId}", id);

            var partner = await documentManager.GetPartnerForDocument(id, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var document = await documentManager.GetAsync(id, cancellationToken);

            if (!string.IsNullOrWhiteSpace(document?.BlobStoragePath))
            {
                await storageManager.DeleteDocumentAsync(document.BlobStoragePath, cancellationToken);
            }

            await documentManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Gets all partner documents across all partners, with partner information included. Admin only.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of all partner documents.</returns>
        /// <response code="200">Returns all partner documents.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpGet("admin/all")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<PartnerDocumentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AdminGetAll(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AdminGetAllPartnerDocuments");

            var results = await documentManager.GetAllWithPartnerAsync(cancellationToken);

            return Ok(results.Select(d => d.ToV2Dto()));
        }

        private async Task<bool> IsAuthorizedAsync(object resource, string policy)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var authResult = await authorizationService.AuthorizeAsync(User, resource, policy);
            return authResult.Succeeded;
        }
    }
}
