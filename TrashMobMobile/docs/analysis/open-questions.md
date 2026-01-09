# Open Questions and Follow-Ups

These are areas where the mobile repo does not provide full context, or where the analysis could be extended:

- Backend/API contract: confirm endpoint specs, authentication requirements, and error responses.
- Shared model ownership: verify versioning and sync strategy with `TrashMob.Models`.
- Analytics and telemetry: Sentry is configured, but confirm logging requirements and retention.
- App configuration: confirm how API base URLs are managed across environments and CI.
- Release pipeline: confirm how mobile builds are produced, signed, and deployed.
- Testing strategy: no test projects are present in this repo; confirm expectations for unit/UI tests.
