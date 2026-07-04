---
title: "STORY-US-021: Implement Attachment Storage Pipeline"
epic: "Epic 2: Student Thesis Workflow"
sprint: "Week 2 (2026-07-04 to 2026-07-10)"
priority: "P1 - High"
status: "completed"
date_created: 2026-07-03
date_completed: 2026-07-10
acceptance_criteria_met: true
branch: "feature/us-021-attachment-storage-pipeline"
---

# STORY-US-021: Implement Attachment Storage Pipeline

**Epic:** Epic 2 — Student Thesis Workflow  
**Sprint:** Week 2 (2026-07-04 to 2026-07-10)  
**Priority:** P1 — High (unblocks File Library in US-012 / StudentDashboard)  
**Estimate:** 4–6 hours  
**Acceptance Criteria Count:** 6 (AC1–AC6)

---

## User Story

**As a** student  
**I want to** upload thesis artefacts alongside my thesis updates  
**So that** my supervisor can review the latest documents and datasets directly from the portal

---

## Context & Dependencies

- **Blocks / unblocks:** US-012 (StudentDashboard File Library route reads persisted attachment metadata — end-to-end upload persistence is the stated remaining gap).
- **Depends on:** Story 1.3 (Identity auth in place), Story 2.2 (ThesisUpdate submission pipeline provides the parent entity for attachments).
- **Existing domain entity:** `ThesisArtifact` already exists in `University.Domain/Entities/ThesisArtifact.cs` — verify its schema before adding new properties.
- **Existing integration test scaffolding:** `tests/University.IntegrationTests/ThesisArtifactStorageServiceTests.cs` is present but likely incomplete — extend it rather than creating a new file.

---

## Acceptance Criteria

### AC1: Allowed File Types and Server-Side Size Validation
**Given** a student is submitting a thesis update,  
**When** they attach a file,  
**Then** the server accepts only: `PDF`, `DOCX`, `PPTX`, `ZIP`  
**And** maximum file size is enforced server-side (configurable via `appsettings.json`, default 20 MB)  
**And** files exceeding the limit or with disallowed MIME types are rejected with a descriptive error message  
**And** the allowed types and size cap are read from `IConfiguration` (key: `Attachments:AllowedExtensions`, `Attachments:MaxFileSizeBytes`) so they can be overridden without recompile

### AC2: Attachment Entity and Database Metadata
**Given** the `ThesisArtifact` entity in `University.Domain/Entities/ThesisArtifact.cs`,  
**When** a file is uploaded successfully,  
**Then** a `ThesisArtifact` record is persisted in the database with:
- `Id` (int, PK)
- `ThesisUpdateId` (int, FK → ThesisUpdate)
- `FileName` (string — original client filename, sanitised)
- `ContentType` (string — MIME type)
- `StorageKey` (string — opaque blob/path key, NOT a public URL)
- `FileSizeBytes` (long)
- `UploadedAt` (DateTime UTC)
- `UploadedByUserId` (string, FK → AspNetUsers)
**And** the `ThesisUpdate` navigation property exposes `ICollection<ThesisArtifact> Attachments`  
**And** an EF Core migration is generated and added to `University.Infrastructure/Migrations/`

### AC3: Storage Service Abstraction (Azure Blob or Local)
**Given** a storage service abstraction `IAttachmentStorageService` in `University.Application`,  
**When** the app runs,  
**Then** a `LocalFileAttachmentStorageService` implementation (stores under `wwwroot/attachments/`) is used by default (enabled when `Attachments:StorageProvider` = `"Local"`)  
**And** an `AzureBlobAttachmentStorageService` stub/implementation exists (enabled when `Attachments:StorageProvider` = `"AzureBlob"`)  
**And** both implementations expose:
- `Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct)` → returns opaque `StorageKey`
- `Task<(Stream stream, string contentType)> DownloadAsync(string storageKey, CancellationToken ct)`
- `Task DeleteAsync(string storageKey, CancellationToken ct)`
**And** the correct implementation is registered in DI based on the config value

### AC4: Secure Download Handler with Expiring Tokens
**Given** a student or professor requests to download an attachment,  
**When** a download URL is generated,  
**Then** the URL is served via a dedicated endpoint (e.g., `/attachments/download/{token}`)  
**And** the token is time-bound (default 15 minutes, configurable via `Attachments:DownloadTokenExpiryMinutes`)  
**And** tokens are generated using a signed mechanism (HMAC or `IDataProtector`) — no plain attachment IDs in the URL  
**And** each download attempt is logged (user ID, attachment ID, timestamp, outcome) at `Information` level via `ILogger`  
**And** expired or tampered tokens return HTTP 403 with no file content

### AC5: Virus Scanning Hook / Placeholder
**Given** a file has been uploaded and stored,  
**When** the upload completes,  
**Then** a `IVirusScanService` interface exists in `University.Application` with method `Task<VirusScanResult> ScanAsync(string storageKey, CancellationToken ct)`  
**And** a `NoOpVirusScanService` default implementation is registered that logs "Virus scan skipped (no provider configured)" and returns `VirusScanResult.Clean`  
**And** the scan result is recorded on the `ThesisArtifact` entity (`ScanStatus`: `Pending`, `Clean`, `Infected`, `Skipped`)  
**And** future real implementations can be swapped in via DI without changing upload handler logic

### AC6: Automated Tests
**Given** `tests/University.IntegrationTests/ThesisArtifactStorageServiceTests.cs`,  
**When** tests run,  
**Then** integration tests cover:
- Upload with valid file → metadata persisted, `StorageKey` non-empty
- Upload with invalid extension → rejected with validation error, no record created
- Upload exceeding size limit → rejected with validation error, no record created
- Download with valid token → returns correct stream
- Download with expired token → returns 403
- Virus scan hook invoked → `ScanStatus` updated on artifact  
**And** unit tests in `University.UnitTests` cover `SubmitUpdateWithAttachmentCommand` handler validation  
**And** all tests pass with `dotnet test`

---

## Technical Context

### Architecture Decision References
- **AD-1 (Clean Architecture):** Storage abstraction lives in `University.Application` (interface) and `University.Infrastructure` (implementations). No file I/O in Domain or Presentation.
- **AD-2 (CQRS via MediatR):** Upload is a command — `UploadAttachmentCommand` / `UploadAttachmentCommandHandler`. Download token generation can be a query.
- **AD-3 (FluentValidation):** `UploadAttachmentValidator` enforces extension and size constraints before the handler executes.
- **AD-4 (Result<T>):** Handlers return `Result<AttachmentDto>` — no bare exceptions bubble to the UI.

### Key Files to Touch
| Layer | File | Action |
|---|---|---|
| Domain | `University.Domain/Entities/ThesisArtifact.cs` | Extend with `ScanStatus`, `UploadedByUserId` if missing |
| Application | `University.Application/Commands/UploadAttachmentCommand.cs` | **Create** |
| Application | `University.Application/Interfaces/IAttachmentStorageService.cs` | **Create** |
| Application | `University.Application/Interfaces/IVirusScanService.cs` | **Create** |
| Infrastructure | `University.Infrastructure/Services/LocalFileAttachmentStorageService.cs` | **Create** |
| Infrastructure | `University.Infrastructure/Services/AzureBlobAttachmentStorageService.cs` | **Create (stub)** |
| Infrastructure | `University.Infrastructure/Services/NoOpVirusScanService.cs` | **Create** |
| Infrastructure | `University.Infrastructure/Migrations/` | Add migration for `ThesisArtifact` schema changes |
| Web | `University.Web/Components/` | Extend thesis update form with file upload input |
| Web | `University.Web/Controllers/AttachmentsController.cs` | **Create** — download endpoint |
| Tests | `tests/University.IntegrationTests/ThesisArtifactStorageServiceTests.cs` | Extend |
| Tests | `tests/University.UnitTests/` | Add `UploadAttachmentCommandHandlerTests.cs` |

### Key Constraints
- 🚫 **NO** public storage URLs stored in the database — only opaque `StorageKey` values
- 🚫 **NO** file content stored in the database (blobs only in storage provider)
- 🚫 **NO** plain attachment IDs exposed in download URLs (must use signed tokens)
- ✅ **DO** sanitise the original filename before storing (strip path traversal characters)
- ✅ **DO** validate MIME type server-side (do not trust `Content-Type` header alone — check file magic bytes for PDF/ZIP at minimum)
- ✅ **DO** make size and type limits configurable through `IConfiguration` so QA can test boundary values without recompile
- ✅ **DO** use `IDataProtector` (already available via ASP.NET Core Data Protection) for signed download tokens — no need for a separate JWT library
- ✅ **DO** run `dotnet build` and `dotnet test` locally before marking AC as met

### appsettings.json Keys to Add
```json
"Attachments": {
  "StorageProvider": "Local",
  "LocalStoragePath": "attachments",
  "MaxFileSizeBytes": 20971520,
  "AllowedExtensions": [ ".pdf", ".docx", ".pptx", ".zip" ],
  "DownloadTokenExpiryMinutes": 15
}
```

---

## Definition of Done

- [x] All 6 ACs pass in a clean `dotnet build` + `dotnet test` run  
- [x] EF Core migration added and verified: `20260703083327_AddThesisArtifactStoragePipeline`  
- [x] No new compiler warnings introduced (0 warnings, 0 errors)  
- [ ] STORY-STATUS.md updated: US-021 → `Completed (Verified)`  
- [ ] US-012 File Library end-to-end upload gap noted as resolved in STORY-STATUS.md

---

## Code Review Findings (2026-07-10)

Review type: **full** (3-layer adversarial — Blind Hunter + Edge Case Hunter + Acceptance Auditor)  
Total findings: **19** → Patched: **6** | Decision needed: **2** | Deferred: **11**

### Patched (applied before merge)

| # | Finding | File(s) |
|---|---------|--------|
| P1 | **Infected file not purged from storage** — after `Infected` scan result, `DeleteAsync` is now called (best-effort) before returning failure | `UploadAttachmentCommandHandler.cs` |
| P2 | **Legacy `DownloadByTokenAsync` threw instead of serving** — when `IDataProtectionProvider` is null (legacy/test constructor), the method now parses the token as a raw Guid (matching `GenerateDownloadToken`'s fallback) rather than throwing `InvalidOperationException` | `ThesisArtifactStorageService.cs` |
| P3 | **Old `/api/thesis-artifacts/{id}` silently returned 200+empty for storage-key artifacts** — now returns `404 Not Found` when `artifact.Data` is null (storage-key-based artifact must use signed endpoint) | `Program.cs` |
| P4 | **Hardcoded `application/octet-stream` discarded stored content-type** — `DownloadByTokenAsync` now discards the content-type returned by `LocalFileAttachmentStorageService` and uses `artifact.ContentType` from the DB; PDFs now serve with `application/pdf` | `ThesisArtifactStorageService.cs` |
| P5 | **`SanitiseFileName` truncated mid-extension at 255 chars** — extension is now preserved when capping to 255 characters | `UploadAttachmentCommandHandler.cs` |
| P6 | **Path traversal via storage key** — `LocalFileAttachmentStorageService` now canonicalizes `_basePath` with `Path.GetFullPath` and validates that every resolved path starts with the storage root | `LocalFileAttachmentStorageService.cs` |

### Decisions Needed (Hermes to resolve)

| # | Finding | Options |
|---|---------|--------|
| D1 | **Token not bound to requesting user's identity** — bearer-token model accepted. Any authenticated user presenting a valid token within its TTL may download the artifact. | ✅ **Resolved:** Option A — bearer-token model is acceptable for this threat model. No change required. |
| D2 | **Token expiry begins at page render, not at click** — `GenerateDownloadToken` is called synchronously when the `ThesisUpdates` Razor component renders. A user who leaves the page open >15 min and then clicks download gets a 403 with no explanation. | **A (current):** document the behaviour; lower default expiry to 30 min. **B:** change `href` to call a fetch API endpoint at click time to get a fresh token (requires JS interop). **C:** use a longer expiry (e.g., 60 min) as a pragmatic compromise. |

### Deferred (acceptable risks / pre-existing)

- Scanner unavailability treated as `Skipped` (fail-open) — intentional service-continuity design; exception is logged
- Full file buffered to `MemoryStream` before size check — bounded by `MaxFileSizeBytes`; acceptable tradeoff for magic-byte inspection
- No EF concurrency token on `UpdateScanStatusAsync` — theoretical risk only; scan is synchronous in current NoOp impl
- Username/name fallback in `FindByEmailAsync → FindByNameAsync` — safe in this system (username == email)
- `DeleteAsync` propagates `IOException` on locked files — standard Windows behavior, not a correctness bug
- Draft re-saves orphan superseded artifacts — pre-existing behavior, not introduced by this story
- Magic-byte switch hardcoded to 4 extensions — adding extensions via config requires a code change; document as known limitation
- Encrypted DOCX (OLE2 format) rejected by magic-byte check — rare, document as known edge case
- `AzureBlobAttachmentStorageService` throws `NotImplementedException` — stub by design, flagged in config comments
- Token timing visible in Blazor SSR — noted under D2 above
- `File.Delete` silent exception on failure — low risk, see DeleteAsync notes
