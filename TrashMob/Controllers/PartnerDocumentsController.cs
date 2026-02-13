namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for managing partner documents, including retrieval, creation, file upload, and download.
    /// </summary>
    [Authorize]
    [Route("api/partnerdocuments")]
    public class PartnerDocumentsController : SecureController
    {
        private readonly IPartnerDocumentManager manager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IPartnerDocumentStorageManager storageManager;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerDocumentsController"/> class.
        /// </summary>
        /// <param name="partnerManager">The partner manager.</param>
        /// <param name="manager">The partner document manager.</param>
        /// <param name="storageManager">The partner document storage manager.</param>
        public PartnerDocumentsController(
            IKeyedManager<Partner> partnerManager,
            IPartnerDocumentManager manager,
            IPartnerDocumentStorageManager storageManager)
        {
            this.manager = manager;
            this.partnerManager = partnerManager;
            this.storageManager = storageManager;
        }

        /// <summary>
        /// Gets all documents for a given partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of partner documents.</remarks>
        [HttpGet("getbypartner/{partnerId}")]
        public async Task<IActionResult> GetPartnerDocuments(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var documents = await manager.GetByParentIdAsync(partnerId, cancellationToken);
            return Ok(documents);
        }

        /// <summary>
        /// Gets a partner document by its unique identifier. Requires a valid user.
        /// </summary>
        /// <param name="partnerDocumentId">The partner document ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The partner document.</remarks>
        [HttpGet("{partnerDocumentId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Get(Guid partnerDocumentId, CancellationToken cancellationToken)
        {
            var partnerDocument = await manager.GetAsync(partnerDocumentId, cancellationToken);

            return Ok(partnerDocument);
        }

        /// <summary>
        /// Adds a new partner document (metadata only, for external URL documents).
        /// </summary>
        /// <param name="partnerDocument">The partner document to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result.</remarks>
        [HttpPost]
        public async Task<IActionResult> Add(PartnerDocument partnerDocument, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerDocument.PartnerId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await manager.AddAsync(partnerDocument, UserId, cancellationToken);
            TrackEvent(nameof(Add) + typeof(PartnerDocument));

            return Ok();
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
        [HttpPost("upload")]
        [RequestSizeLimit(26_214_400)]
        public async Task<IActionResult> Upload(
            [FromForm] Guid partnerId,
            [FromForm] string name,
            [FromForm] int documentTypeId,
            [FromForm] DateTimeOffset? expirationDate,
            IFormFile formFile,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            if (formFile == null || formFile.Length == 0)
            {
                return BadRequest("No file provided.");
            }

            if (formFile.Length > MaxFileSizeBytes)
            {
                return BadRequest($"File size exceeds the maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)} MB.");
            }

            if (!AllowedContentTypes.Contains(formFile.ContentType))
            {
                return BadRequest($"File type '{formFile.ContentType}' is not allowed. Allowed types: PDF, Word, Excel, PNG, JPEG.");
            }

            var currentUsage = await storageManager.GetPartnerStorageUsageBytesAsync(partnerId, cancellationToken);
            if (currentUsage + formFile.Length > MaxPartnerStorageBytes)
            {
                return BadRequest($"Uploading this file would exceed the partner storage limit of {MaxPartnerStorageBytes / (1024 * 1024)} MB.");
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

            await manager.AddAsync(document, UserId, cancellationToken);

            var blobPath = await storageManager.UploadDocumentAsync(partnerId, document.Id, formFile, cancellationToken);
            document.BlobStoragePath = blobPath;
            await manager.UpdateAsync(document, UserId, cancellationToken);

            TrackEvent(nameof(Upload) + typeof(PartnerDocument));

            return Ok(document);
        }

        /// <summary>
        /// Generates a time-limited download URL for a partner document.
        /// </summary>
        /// <param name="partnerDocumentId">The partner document ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{partnerDocumentId}/download")]
        public async Task<IActionResult> Download(Guid partnerDocumentId, CancellationToken cancellationToken)
        {
            var partner = await manager.GetPartnerForDocument(partnerDocumentId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var document = await manager.GetAsync(partnerDocumentId, cancellationToken);
            if (string.IsNullOrWhiteSpace(document?.BlobStoragePath))
            {
                return BadRequest("This document has no uploaded file.");
            }

            var downloadUrl = await storageManager.GetDownloadUrlAsync(document.BlobStoragePath, cancellationToken);
            return Ok(new { downloadUrl });
        }

        /// <summary>
        /// Gets storage usage in bytes for a partner, including the total limit.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("storageusage/{partnerId}")]
        public async Task<IActionResult> GetStorageUsage(Guid partnerId, CancellationToken cancellationToken)
        {
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
        /// <param name="partnerDocument">The partner document to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result.</remarks>
        [HttpPut]
        public async Task<IActionResult> Update(PartnerDocument partnerDocument, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerDocument.PartnerId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var result = await manager.UpdateAsync(partnerDocument, UserId, cancellationToken);
            TrackEvent(nameof(Update) + typeof(PartnerDocument));

            return Ok(result);
        }

        /// <summary>
        /// Deletes a partner document by its unique identifier, including any uploaded blob.
        /// </summary>
        /// <param name="partnerDocumentId">The partner document ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result.</remarks>
        [HttpDelete("{partnerDocumentId}")]
        public async Task<IActionResult> Delete(Guid partnerDocumentId, CancellationToken cancellationToken)
        {
            var partner = await manager.GetPartnerForDocument(partnerDocumentId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var document = await manager.GetAsync(partnerDocumentId, cancellationToken);
            if (!string.IsNullOrWhiteSpace(document?.BlobStoragePath))
            {
                await storageManager.DeleteDocumentAsync(document.BlobStoragePath, cancellationToken);
            }

            await manager.DeleteAsync(partnerDocumentId, cancellationToken);
            TrackEvent(nameof(Delete) + typeof(PartnerDocument));

            return NoContent();
        }
    }
}
