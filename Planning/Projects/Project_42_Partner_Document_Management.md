# Project 42 — Partner Document Management

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | None |

---

## Business Rationale

Partners (especially government agencies) need to store and manage important documents related to their TrashMob partnership — Organizational Volunteer Agreements, contracts, useful reports, insurance certificates, and more. These are typically PDFs and other common document formats.

The Partner Documents page already exists in the partner admin dashboard with full CRUD for document metadata (name + external URL), but it has never been connected to actual file storage. Today partners must host their own files elsewhere and paste URLs, which creates friction, broken links, and a poor user experience.

Building out this feature with direct file upload to Azure Blob Storage transforms Partner Documents from a link aggregator into a proper document management system, making it genuinely useful for partner administrators.

## Objectives

### Primary Goals
- Allow partner admins to upload documents (PDF, Word, Excel, images) directly from the partner dashboard
- Store uploaded files securely in Azure Blob Storage with proper access controls
- Enable document download for authorized partner users and site admins
- Support document categorization (e.g., Agreement, Contract, Report, Insurance, Other)
- Add document expiration dates with notification reminders for renewals

### Secondary Goals (Nice-to-Have)
- Document version history (upload new version, keep previous versions)
- Bulk upload (drag and drop multiple files)
- Document preview (inline PDF viewer)
- Automatic expiration reminders via email

## Scope

### Phase 1 — File Upload & Storage (Backend)
- ? Add Azure Blob Storage container for partner documents
- ? Extend `PartnerDocument` model with: `FileSize`, `ContentType`, `BlobStorageUrl`, `DocumentType` (enum), `ExpirationDate`
- ? Create upload endpoint that accepts multipart file upload and stores to Azure Blob Storage
- ? Create download endpoint that generates time-limited SAS URLs for authorized users
- ? Add file size limit validation (e.g., 25 MB per file, 500 MB per partner)
- ? Add content type validation (PDF, DOCX, XLSX, PNG, JPG)
- ? Update existing CRUD endpoints to handle new fields
- ? Add migration for schema changes

### Phase 2 — Upload UI & Document Types
- ? Replace URL text input with file upload component (drag-and-drop zone)
- ? Add document type selector (Agreement, Contract, Report, Insurance, Certificate, Other)
- ? Add optional expiration date field
- ? Show file size and type in document list
- ? Add download button to document list
- ? Keep backward compatibility with existing URL-only documents
- ? Update document list table with new columns (Type, Size, Expiration)

### Phase 3 — Polish & Administration
- ? Add document expiration badge (expired, expiring soon, active)
- ? Add filtering by document type
- ? Add sorting by name, date, type, expiration
- ? Add storage usage indicator per partner
- ? Site admin view of documents across all partners
- ? Add feature usage tracking (upload counts, download counts)

## Out-of-Scope

- Document editing within the app (e.g., PDF annotation)
- OCR or text extraction from uploaded documents
- E-signature integration
- Document sharing between partners
- Mobile app document management (web only for now)
- Document templates or auto-generation

## Success Metrics

### Quantitative
- Partners upload at least 1 document within 30 days of the feature launch
- 80% of new documents use file upload vs. external URL within 60 days
- Average partner stores 3+ documents within 90 days
- Zero unauthorized document access incidents

### Qualitative
- Partner admins find it easy to upload and manage documents
- Site admins can quickly locate partner agreements when needed
- Document expiration reminders reduce lapsed agreement incidents

## Dependencies

### Blockers
- None — Azure Blob Storage is already used for other features (photos)

### Enablers for Other Projects
- Supports Project 8 (Waivers V3) — signed waivers could be stored as partner documents
- Supports Project 41 (Sponsored Adoptions) — adoption agreements could be stored here

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| Azure Blob Storage costs increase | Low | Low | Enforce per-partner storage limits; use cool/archive tier for old docs |
| Large file uploads fail on slow connections | Medium | Medium | Add upload progress indicator; chunked upload for large files |
| Sensitive documents exposed | Low | High | SAS URL with short TTL; authorization checks on every download; no public blob access |
| Existing URL-only documents break | Low | Medium | Keep URL field working; treat as "external link" document type |

## Implementation Plan

### Data Model Changes

**Modify:** `TrashMob.Models/PartnerDocument.cs`
```csharp
public class PartnerDocument : KeyedModel
{
    public Guid PartnerId { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }                    // Existing — external URL or blob URL
    public string BlobStoragePath { get; set; }         // New — Azure Blob path (null for external URLs)
    public string ContentType { get; set; }             // New — MIME type (e.g., application/pdf)
    public long? FileSizeBytes { get; set; }            // New — file size in bytes
    public int DocumentTypeId { get; set; }             // New — FK to DocumentType enum
    public DateTimeOffset? ExpirationDate { get; set; } // New — optional expiration
    public virtual Partner Partner { get; set; }
}
```

**New enum:** `PartnerDocumentType` (Agreement, Contract, Report, Insurance, Certificate, Other)

### API Changes

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/partnerdocuments/upload` | POST | Multipart file upload → Blob Storage |
| `/api/partnerdocuments/{id}/download` | GET | Generate SAS URL and redirect |
| `/api/partnerdocuments` | PUT | Update metadata (existing, extended) |

### Web UX Changes

- Replace URL input with file upload dropzone + optional external URL toggle
- Add document type dropdown and expiration date picker to create/edit forms
- Enhance document list table with Type, Size, Expiration columns
- Add download action button
- Add expiration status badges

### Infrastructure Changes

- Azure Blob Storage container: `partner-documents`
- Container access level: Private (no anonymous access)
- SAS URL TTL: 15 minutes
- Blob lifecycle policy: Move to cool tier after 90 days of no access

## Implementation Phases

### Phase 1 — File Upload & Storage (Backend)
- Add Blob Storage service for partner documents
- Extend PartnerDocument model and create migration
- Build upload and download endpoints
- Add authorization and validation

### Phase 2 — Upload UI & Document Types
- Build file upload component with drag-and-drop
- Add document type and expiration to forms
- Update document list with new columns and download button

### Phase 3 — Polish & Administration
- Add expiration tracking and badges
- Add filtering, sorting, and storage usage
- Site admin cross-partner document view
- Feature usage metrics

## Open Questions

| Question | Recommendation | Owner | Due |
|----------|----------------|-------|-----|
| Maximum file size per upload? | 25 MB — covers most PDFs and scanned docs | Engineering | Before Phase 1 |
| Maximum total storage per partner? | 500 MB — generous for document storage | Engineering | Before Phase 1 |
| Should expired documents be auto-deleted? | No — flag as expired, let admins decide | Product | Before Phase 3 |
| Should we support document sharing between partner and TrashMob admin? | Yes — both partner admins and site admins should have access | Product | Before Phase 1 |

## Related Documents

- [Project 8 - Waivers V3](./Project_08_Waivers_V3.md) — Signed waivers as documents
- [Project 10 - Community Pages](./Project_10_Community_Pages.md) — Partner community management
- [Project 41 - Sponsored Adoptions](./Project_41_Sponsored_Adoptions.md) — Adoption agreements

---

**Last Updated:** February 8, 2026
**Owner:** Product & Engineering Team
**Status:** Not Started
**Next Review:** When a volunteer picks up the project
