# Project 53 — Mobile Offline Persistence: Routes, Metrics, and Photos

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress (Phases 1–3 Complete) |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Large |
| **Dependencies** | Project 15 (Route Tracing), Project 48 (Enhanced Route Tracking), Project 4 (Mobile Robustness) |

---

## Business Rationale

The mobile app currently operates with a synchronous, online-only architecture. GPS route points are held in memory during recording, event photos live temporarily in the cache directory, and attendee metrics exist only in the form's UI state. All three data types are uploaded immediately via HTTP — and permanently lost if the upload fails.

This has caused real-world data loss:

1. **Route loss from poor connectivity:** A 4-mile route recording was lost because the upload never reached the server. Production App Insights confirmed zero failed requests, meaning the client exhausted its 3 Polly retries and silently dropped the data. The volunteer's effort was unrecoverable.

2. **No crash recovery:** If the app is killed by the OS during route recording (common on Android for battery optimization), the `ObservableCollection<Location>` in `ViewEventViewModel` is garbage collected. There is no way to resume or recover.

3. **Photo upload fragility:** Compressed photos are written to `FileSystem.CacheDirectory` before upload, but on upload failure there is no queue to retry. The photo stays in cache but the user must manually re-select and re-upload.

4. **Metrics loss on timeout:** If the attendee metrics POST times out (60-second limit), the data is gone. The user must re-enter bags, weight, and duration from memory.

With Polly's 3-retry policy (2s → 4s → 8s = 14 seconds total), any connectivity gap longer than ~15 seconds results in permanent data loss. Volunteers cleaning up parks, trails, and rural areas regularly encounter dead zones lasting minutes to hours.

---

## Objectives

### Primary Goals
- **Zero data loss** for GPS route recordings, regardless of connectivity or app lifecycle
- **Automatic background sync** for routes, metrics, and photos when connectivity returns
- **Crash recovery** for active route recordings — resume from last persisted point
- **Retry queue** with visible status so users know what's pending

### Secondary Goals (Nice-to-Have)
- Offline viewing of upcoming event details (cached from last fetch)
- Conflict resolution for metrics edited offline and online simultaneously
- Battery-aware sync scheduling (defer large photo uploads to Wi-Fi)
- Sync progress indicator in the app shell

---

## Scope

### Phase 1 — Local Database & Route Persistence

**Goal:** Introduce SQLite via `sqlite-net-pcl` and persist GPS points during route recording so no data is lost on crash, kill, or upload failure.

#### Database Setup

- [x] Add NuGet packages: `sqlite-net-pcl`, `SQLitePCLRaw.bundle_green`
- [x] Create `OfflineDatabase` singleton with lazy-initialized `SQLiteAsyncConnection`
  - Database path: `FileSystem.AppDataDirectory/trashmob_offline.db`
  - Create tables on first access
- [x] Register `OfflineDatabase` as singleton in `MauiProgram.cs`

#### Route Point Persistence

- [x] Create `PendingRoutePoint` SQLite table:
  - `Id` (int, auto-increment PK)
  - `SessionId` (Guid) — groups points from a single recording session
  - `EventId` (Guid)
  - `UserId` (Guid)
  - `Latitude` (double)
  - `Longitude` (double)
  - `Altitude` (double?)
  - `Timestamp` (DateTimeOffset)
  - `Order` (int) — sequence number within session
- [x] Create `PendingRouteSession` SQLite table:
  - `SessionId` (Guid, PK)
  - `EventId` (Guid)
  - `UserId` (Guid)
  - `StartTime` (DateTimeOffset)
  - `EndTime` (DateTimeOffset?) — null while recording
  - `Status` (string) — `Recording`, `PendingUpload`, `Uploading`, `Uploaded`, `Failed`
  - `SkipDefaultTrim` (bool)
  - `LastError` (string?)
  - `RetryCount` (int)
  - `CreatedAt` (DateTimeOffset)
- [x] Update `ViewEventViewModel.RealTimeLocationTracker`:
  - On each GPS callback, write `PendingRoutePoint` to SQLite in addition to the in-memory collection
  - On "Start Route": create `PendingRouteSession` with status `Recording`
  - On "Stop Route": update session status to `PendingUpload`, set `EndTime`
- [x] Update `RouteTrackingSessionManager`:
  - Persist `ActiveSessionId` to `Preferences` so it survives app restart
  - On app startup, check for `Recording` sessions → offer to resume or discard
- [x] Route crash recovery:
  - On `ViewEventPage` load, check for any `PendingRouteSession` with status `Recording`
  - If found, reload points from SQLite into the `Locations` collection
  - Resume GPS tracking from where it left off
  - Show notification: "Resuming route recording from [X] minutes ago"

#### Route Upload from SQLite

- [x] Update `SaveRoute()` flow:
  1. Mark session as `PendingUpload` in SQLite
  2. Attempt upload immediately
  3. On success: mark as `Uploaded`, delete route points from SQLite
  4. On failure: mark as `Failed`, increment `RetryCount`, store `LastError`
  5. Background sync will retry later (Phase 3)
- [x] Keep in-memory `Locations` collection in sync with SQLite for live map rendering

### Phase 2 — Metrics & Photo Offline Queue

**Goal:** Queue failed metric submissions and photo uploads for automatic retry.

#### Metrics Queue

- [x] Create `PendingMetrics` SQLite table:
  - `Id` (int, auto-increment PK)
  - `EventId` (Guid)
  - `UserId` (Guid)
  - `BagsCollected` (int?)
  - `PickedWeight` (decimal?)
  - `PickedWeightUnitId` (int?)
  - `DurationMinutes` (int?)
  - `Notes` (string?)
  - `Status` (string) — `PendingUpload`, `Uploading`, `Uploaded`, `Failed`
  - `LastError` (string?)
  - `RetryCount` (int)
  - `CreatedAt` (DateTimeOffset)
- [x] Update `LogImpact()` / `SubmitMyMetricsAsync()` flow:
  1. Save metrics to SQLite first
  2. Attempt upload immediately
  3. On success: mark as `Uploaded`
  4. On failure: mark as `Failed` — background sync retries later
- [x] Prevent duplicate submissions: server-side upsert semantics on `SubmitMyMetrics` (POST replaces existing)

#### Photo Queue

- [x] Create `PendingPhoto` SQLite table:
  - `Id` (int, auto-increment PK)
  - `PhotoId` (Guid) — client-generated for dedup
  - `EventId` (Guid)
  - `UserId` (Guid)
  - `LocalFilePath` (string) — path to compressed image in `AppDataDirectory` (not cache)
  - `PhotoType` (int) — EventPhotoType enum value
  - `Status` (string) — `PendingUpload`, `Uploading`, `Uploaded`, `Failed`
  - `LastError` (string?)
  - `RetryCount` (int)
  - `CreatedAt` (DateTimeOffset)
- [x] Move compressed photos from `CacheDirectory` to `AppDataDirectory/pending_photos/` (cache can be cleared by OS)
- [x] Update `PickPhoto()` flow:
  1. Compress photo → save to `AppDataDirectory/pending_photos/{guid}.jpg`
  2. Create `PendingPhoto` record in SQLite
  3. Attempt upload immediately
  4. On success: mark as `Uploaded`, delete local file
  5. On failure: mark as `Failed` — file preserved for retry
- [x] Clean up uploaded photos: delete local files for records with status `Uploaded` in `OfflineDatabase.CleanupAsync()`
- [x] Update `SyncService` to process metrics and photo queues (routes → metrics → photos order)
- [x] Update `OfflineDatabase.CleanupAsync()` to clean metrics and photos

### Phase 3 — Background Sync Service

**Goal:** Automatically retry failed uploads when connectivity returns, with exponential backoff and a user-visible sync status.

#### Sync Engine

- [x] Create `SyncService` (singleton):
  - Monitors connectivity via `Connectivity.Current.NetworkAccess`
  - On connectivity change from `None` → `Internet`: trigger sync
  - Periodic sync every 5 minutes when app is in foreground
  - Processes queues in order: routes first, then metrics, then photos
- [x] Sync retry policy:
  - Exponential backoff: 1min, 5min, 15min, 30min, 1hr (max)
  - Max retry count: 20 (spans ~24 hours)
  - After max retries: mark as `PermanentlyFailed`, notify user
  - Reset retry count when user manually triggers sync
- [x] Register `SyncService` in `MauiProgram.cs`, start on app launch
- [x] Toast notification when background sync completes

#### Sync UI

- [x] Create `SyncStatusPage` + `SyncStatusViewModel`:
  - List of pending items grouped by type (Routes, Metrics, Photos)
  - Each item shows: description, status, last error, retry count
  - "Retry All" button
  - "Discard" option for route items
  - Pull-to-refresh
- [x] Add navigation to Sync Status from More page
- [x] Register page/ViewModel in DI and Shell routes

#### Background Processing (Platform-Specific) — Deferred to Phase 4

- [ ] **Android:** Register a `WorkManager` periodic work request for sync when app is backgrounded
  - Constraint: requires network connectivity
  - Minimum interval: 15 minutes (Android WorkManager minimum)
  - Battery-not-low constraint for photo uploads
- [ ] **iOS:** Use `BGTaskScheduler` for background sync
  - Register `BGAppRefreshTask` for periodic sync
  - Register `BGProcessingTask` for large photo uploads (with `requiresNetworkConnectivity`)

### Phase 4 — Robustness & Edge Cases

**Goal:** Handle edge cases, add telemetry, and ensure data integrity.

#### Data Integrity

- [ ] Implement idempotent uploads:
  - Routes: include `SessionId` in upload request; server checks for duplicate `SessionId` before inserting
  - Metrics: server upserts (update if exists, insert if not) based on `EventId` + `UserId`
  - Photos: include client-generated `PhotoId` (Guid); server checks for duplicate
- [ ] Add backend endpoints for idempotent operations (if not already present):
  - `POST /api/eventattendeeroutes` — check `SessionId` uniqueness
  - `PUT /api/events/{eventId}/attendee-metrics/my-metrics` — upsert semantics (already exists)
  - `POST /api/events/{eventId}/photos` — check `PhotoId` header for dedup
- [ ] Database migration versioning: include version number in SQLite schema, handle upgrades gracefully

#### Telemetry

- [ ] Track sync events in Sentry:
  - `OfflineRouteQueued` — route saved to SQLite (count, point count)
  - `OfflineRouteSynced` — route uploaded from queue (retry count, time in queue)
  - `OfflineRouteFailed` — route permanently failed (last error)
  - Same trio for metrics and photos
- [ ] Track sync health metrics:
  - Average time-in-queue before successful upload
  - Retry count distribution
  - Permanent failure rate
  - Queue size at app launch (indicates chronic connectivity issues)

#### Storage Management

- [ ] Set maximum pending photo storage: 500 MB
  - When exceeded, show warning: "Pending photo storage is full. Connect to upload before taking more photos."
  - Oldest `Uploaded` records cleaned first, then `PermanentlyFailed`
- [ ] Set maximum route point storage: 100,000 points (~50 hours of recording at 1 point/2 seconds)
  - When exceeded, reduce recording frequency or warn user
- [ ] Periodic cleanup: on app launch, delete `Uploaded` records older than 7 days
- [ ] SQLite database VACUUM on cleanup to reclaim space

---

## Out of Scope

- Full offline mode (browsing events, viewing maps without connectivity)
- Sync conflict resolution for concurrent edits (last-write-wins is acceptable for v1)
- Cloud-to-device sync (pushing server data down to the phone)
- Offline event creation or sign-up
- Background route recording when app is fully killed (requires foreground service, separate project)
- Cross-device sync (user's offline queue is device-specific)

---

## Success Metrics

| Metric | Target | How to Measure |
|--------|--------|----------------|
| Route data loss rate | 0% (down from estimated 5-10%) | Sentry: `OfflineRouteFailed` / (`OfflineRouteQueued` + direct uploads) |
| Crash recovery rate | ≥ 90% of interrupted recordings resume | Sentry: `RouteSessionResumed` / `RouteSessionCrashed` |
| Sync success rate | ≥ 99% of queued items eventually upload | Sentry: `OfflineRouteSynced` / `OfflineRouteQueued` |
| Average time-in-queue | < 30 minutes for routes, < 2 hours for photos | Sentry breadcrumbs: time between `Queued` and `Synced` events |
| User awareness | < 5% of users with pending items don't know about them | Sync badge visibility — no direct metric, qualitative feedback |

---

## Dependencies

| Dependency | Type | Status |
|------------|------|--------|
| Project 15 (Route Tracing) | Required | In Progress — route recording architecture |
| Project 48 (Enhanced Route Tracking) | Related | In Progress — route metrics, may need offline support for "Log Pickup" |
| Project 4 (Mobile Robustness) | Related | In Progress — error handling patterns |
| `sqlite-net-pcl` NuGet package | Required | Available — widely used in MAUI apps |
| `SQLitePCLRaw.bundle_green` | Required | Available — SQLite native bindings |
| Android WorkManager | Required | Built into AndroidX (already referenced) |
| iOS BGTaskScheduler | Required | Built into iOS 13+ |

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| SQLite write contention during GPS recording (main thread vs. background) | Medium | Medium | Use `SQLiteAsyncConnection` with WAL journal mode; batch writes every 5 seconds instead of per-point |
| Large photo queue consumes device storage | Medium | Low | 500 MB cap with user warning; auto-cleanup of uploaded files |
| Background sync killed by OS (especially iOS) | High | Medium | Foreground sync on app resume as primary mechanism; background sync as bonus; sync badge keeps user informed |
| Duplicate uploads on retry (route uploaded but response lost) | Medium | High | Idempotent uploads using `SessionId`/`PhotoId`; server-side dedup check |
| SQLite database corruption | Low | High | WAL mode for crash safety; backup SQLite file before schema migrations; catch `SQLiteException` and recreate if corrupted |
| Android Doze mode prevents sync | Medium | Low | Use `WorkManager` with network constraint — Android handles Doze windows; sync also triggers on app resume |
| Route recording battery drain from frequent SQLite writes | Low | Medium | Batch writes (every 5 seconds or 10 points, whichever comes first); use WAL mode to minimize I/O |

---

## Implementation Plan

### Technical Architecture

#### SQLite Schema

```sql
-- Route recording sessions
CREATE TABLE PendingRouteSessions (
    SessionId TEXT PRIMARY KEY,          -- Guid as string
    EventId TEXT NOT NULL,
    UserId TEXT NOT NULL,
    StartTime TEXT NOT NULL,             -- ISO 8601
    EndTime TEXT,
    Status TEXT NOT NULL DEFAULT 'Recording',  -- Recording, PendingUpload, Uploading, Uploaded, Failed, PermanentlyFailed
    SkipDefaultTrim INTEGER NOT NULL DEFAULT 0,
    LastError TEXT,
    RetryCount INTEGER NOT NULL DEFAULT 0,
    NextRetryAt TEXT,                    -- ISO 8601, for exponential backoff
    CreatedAt TEXT NOT NULL
);

-- GPS points for route recordings
CREATE TABLE PendingRoutePoints (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SessionId TEXT NOT NULL,
    Latitude REAL NOT NULL,
    Longitude REAL NOT NULL,
    Altitude REAL,
    Timestamp TEXT NOT NULL,
    PointOrder INTEGER NOT NULL,
    FOREIGN KEY (SessionId) REFERENCES PendingRouteSessions(SessionId) ON DELETE CASCADE
);
CREATE INDEX IX_PendingRoutePoints_SessionId ON PendingRoutePoints(SessionId);

-- Queued attendee metrics
CREATE TABLE PendingMetrics (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EventId TEXT NOT NULL,
    UserId TEXT NOT NULL,
    BagsCollected INTEGER,
    PickedWeight TEXT,                   -- decimal as string for precision
    PickedWeightUnitId INTEGER,
    DurationMinutes INTEGER,
    Notes TEXT,
    Status TEXT NOT NULL DEFAULT 'PendingUpload',
    LastError TEXT,
    RetryCount INTEGER NOT NULL DEFAULT 0,
    NextRetryAt TEXT,
    CreatedAt TEXT NOT NULL
);

-- Queued photo uploads
CREATE TABLE PendingPhotos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PhotoId TEXT NOT NULL,               -- Client-generated Guid for dedup
    EventId TEXT NOT NULL,
    UserId TEXT NOT NULL,
    LocalFilePath TEXT NOT NULL,
    Status TEXT NOT NULL DEFAULT 'PendingUpload',
    LastError TEXT,
    RetryCount INTEGER NOT NULL DEFAULT 0,
    NextRetryAt TEXT,
    CreatedAt TEXT NOT NULL
);
```

#### Key Classes

| Class | Location | Responsibility |
|-------|----------|---------------|
| `OfflineDatabase` | `Services/Offline/OfflineDatabase.cs` | SQLite connection management, table creation, schema migration |
| `RoutePointWriter` | `Services/Offline/RoutePointWriter.cs` | Batched GPS point persistence (buffers in memory, flushes every 5s) |
| `SyncQueue` | `Services/Offline/SyncQueue.cs` | Enqueue/dequeue pending items, manage status transitions |
| `SyncService` | `Services/Offline/SyncService.cs` | Background sync engine, connectivity monitoring, retry scheduling |
| `SyncStatusViewModel` | `ViewModels/SyncStatusViewModel.cs` | Sync UI state, manual retry/discard actions |
| `SyncStatusPage` | `Pages/SyncStatusPage.xaml` | Pending items list, retry buttons |

#### Data Flow: Route Recording (After)

```
GPS Callback
    ↓
RoutePointWriter.AddPoint()  →  In-memory buffer (ObservableCollection for map)
    ↓ (every 5s)
SQLite batch write (PendingRoutePoints)
    ↓ (user taps "Stop")
Mark PendingRouteSession as PendingUpload
    ↓
Attempt immediate upload
    ↓
Success? → Mark Uploaded, delete points from SQLite
Failure? → Mark Failed, SyncService retries later
```

#### Data Flow: Photo Upload (After)

```
User selects photo
    ↓
ImageCompressor.CompressAsync()
    ↓
Copy to AppDataDirectory/pending_photos/{guid}.jpg
    ↓
Create PendingPhoto record in SQLite
    ↓
Attempt immediate upload
    ↓
Success? → Mark Uploaded, delete local file
Failure? → Mark Failed, SyncService retries later
```

### Backend Changes

| Change | File | Description |
|--------|------|-------------|
| Route dedup | `EventAttendeeRouteManager.cs` | Check for existing route with same `SessionId` before insert; return existing if found |
| Photo dedup | `EventPhotoManager.cs` or controller | Accept `X-Photo-Id` header; check for duplicate before saving |
| SessionId field | `DisplayEventAttendeeRoute.cs` | Add `SessionId` (Guid?) property for client-to-server correlation |
| SessionId column | Migration | Add `SessionId` (Guid?, nullable, indexed) to `EventAttendeeRoutes` table |

---

## Implementation Phases

### Phase 1 — Local Database & Route Persistence (2-3 weeks)
1. Add SQLite NuGet packages and create `OfflineDatabase`
2. Create `PendingRouteSession` and `PendingRoutePoint` tables
3. Implement `RoutePointWriter` with batched writes
4. Update `ViewEventViewModel` to persist points during recording
5. Update `RouteTrackingSessionManager` to persist session state
6. Implement crash recovery (detect interrupted sessions on app launch)
7. Update `SaveRoute()` to upload from SQLite queue
8. Add `SessionId` to backend for idempotent route uploads
9. Unit tests for `RoutePointWriter` batching and session recovery

### Phase 2 — Metrics & Photo Queue (1-2 weeks)
1. Create `PendingMetrics` and `PendingPhoto` tables
2. Update metrics submission to save-then-upload pattern
3. Move photo storage from `CacheDirectory` to `AppDataDirectory`
4. Update photo upload to save-then-upload pattern
5. Add photo dedup header to backend
6. Unit tests for queue operations

### Phase 3 — Background Sync & UI (2-3 weeks)
1. Implement `SyncService` with connectivity monitoring
2. Implement exponential backoff retry scheduling
3. Add platform-specific background sync (Android WorkManager, iOS BGTaskScheduler)
4. Create `SyncStatusPage` and `SyncStatusViewModel`
5. Add sync badge to app shell
6. Toast notifications for sync completion
7. Integration testing with simulated connectivity drops

### Phase 4 — Robustness & Edge Cases (1 week)
1. Storage management (caps, cleanup, VACUUM)
2. Sentry telemetry for sync events
3. Database schema versioning
4. Edge case testing (rapid on/off connectivity, full storage, long recording sessions)

---

## Open Questions

1. ~~**Which SQLite library?** `sqlite-net-pcl` (simple ORM) vs `Microsoft.EntityFrameworkCore.Sqlite` (full EF Core)?~~ → **Recommended:** `sqlite-net-pcl` — lightweight, well-tested with MAUI, avoids the overhead of a second EF Core context.

2. **Background route recording:** Should the sync service also enable route recording to continue when the app is in the background? This would require an Android Foreground Service with persistent notification. Currently route recording stops when the app is backgrounded. This is a separate concern but closely related. → **Defer to separate project** — background recording is a bigger lift and involves platform-specific foreground service work.

3. **Photo upload over Wi-Fi only?** Should large photo uploads (>1 MB) be deferred to Wi-Fi to save cellular data? → **Suggested:** Default to upload immediately on any connection; add a user setting "Upload photos on Wi-Fi only" in a future iteration.

4. **Sync on app termination:** Can we trigger a final sync attempt when the user swipes the app away? → **Answer:** Partially — Android `onDestroy` can trigger a `WorkManager` enqueue, but iOS doesn't guarantee execution time. The sync badge on next launch is the reliable fallback.

---

## Related Documents

- [Project 15 — Map Route Tracing](Project_15_Route_Tracing.md) — Base route recording implementation
- [Project 48 — Enhanced Route Tracking](Project_48_Enhanced_Route_Tracking.md) — Route metrics, density, time editing
- [Project 4 — Mobile Robustness](Project_04_Mobile_Robustness.md) — Mobile stability and error handling
- `TrashMobMobile/ViewModels/ViewEventViewModel.cs` — Current route recording logic
- `TrashMobMobile/Services/EventAttendeeRouteRestService.cs` — Route upload service
- `TrashMobMobile/Services/EventPhotoRestService.cs` — Photo upload service
- `TrashMobMobile/Services/EventAttendeeMetricsRestService.cs` — Metrics submission service
- `TrashMobMobile/Extensions/ServiceCollectionExtensions.cs` — Current Polly retry configuration
- `TrashMobMobile/ViewModels/BaseViewModel.cs` — Error handling patterns

---

## GitHub Issues

_To be created on project start._

---

**Last Updated:** March 1, 2026
**Owner:** @JoeBeernink
**Status:** Not Started
**Next Review:** Phase 1 kickoff
