# PR-08 Backup Plan — University Thesis Management System Demo

**Presenter:** Hermes  
**Date:** 2026-07-10  
**App URL:** `http://localhost:5118`

---

## Pre-Demo Machine Checklist

Complete 30 minutes before the presentation. Any ❌ triggers the mitigation in parentheses.

| # | Check | Command / Action | Pass Condition |
|---|---|---|---|
| 1 | Docker SQL Server running | `docker ps` | `sql` container shows `Up`, port `0.0.0.0:1433->1433/tcp` |
| 2 | App starts without errors | `.\scripts\start-demo.ps1` | No red exception text; banner shows `http://localhost:5118` |
| 3 | Login page reachable | `Invoke-WebRequest http://localhost:5118/login -UseBasicParsing` | `StatusCode: 200` |
| 4 | Student login works | Login as `student1@univ.edu` / `TempPass123!` | Redirected to `/dashboard` |
| 5 | Professor login works | Login as `prof1@univ.edu` / `TempPass123!` | Redirected to `/` (Home) |
| 6 | Attachment upload works | Submit a test update with a small PDF | Entry appears in timeline with download link |
| 7 | Feedback submit works | As prof1, review the update and submit feedback | Status flips to Approved on student side |
| 8 | Fallback screenshot folder exists | Open `C:\demo-fallback\` in File Explorer | All screenshots listed in Section 3 are present |
| 9 | Slides deck open and on correct slide | Presentation software | Slide 1 visible |
| 10 | Browser tabs pre-loaded | 2 tabs: `/login` for student, `/login` for professor | Both tabs show login form |

If any check fails with < 10 minutes to go, switch to **Emergency Fallback** (see Section 4).

---

## Failure Scenarios

### Scenario 1 — App Will Not Start

**Symptom:** `start-demo.ps1` throws an exception or the process exits immediately.

**Likely causes:**
- SQL Server container not running
- Port 5118 already in use
- `dotnet` runtime not found

**Mitigation steps:**
1. Run `docker start sql` — if container was stopped
2. Run `netstat -ano | findstr :5118` — kill the process using port 5118 (`taskkill /PID <pid> /F`), then retry
3. Run `dotnet run --project University.Web/University.Web.csproj` directly without the script
4. Check `appsettings.Development.json` connection string matches Docker container SA password

**Fallback:** If app still won't start after 3 minutes → switch to **offline screenshots** and narrate the demo from `C:\demo-fallback\`.

---

### Scenario 2 — Database Connection Error at Runtime

**Symptom:** Page loads but shows a SQL exception or blank data; EF Core migration error in console.

**Mitigation steps:**
1. Stop the app (Ctrl+C in terminal)
2. Run `docker start sql` if container stopped mid-session
3. Wait 10 seconds for SQL Server to become ready, then `dotnet run` again
4. If migration error: run `dotnet ef database update --project University.Infrastructure` manually

**Fallback:** If DB unavailable → show architecture slides (Appendix A2) and narrate data flow. Offer to restart after Q&A.

---

### Scenario 3 — Attachment Upload Fails

**Symptom:** File upload shows an error toast; attachment does not appear in the timeline.

**Likely causes:**
- File too large (> 20 MB)
- Wrong file type
- `wwwroot/attachments/` directory permissions

**Mitigation steps:**
1. Try a different file — use the pre-prepared `thesis-update-v1.pdf` (< 1 MB) from the Desktop
2. Check the running terminal for exception details
3. If directory missing: `mkdir University.Web/wwwroot/attachments`

**Fallback:** Skip the attachment part of the demo. Say *"Attachment storage is configured for local filesystem by default; the path is configurable for Azure Blob Storage."* Show the `appsettings.json` setting instead. Continue with the rest of the flow.

---

### Scenario 4 — Login Fails (Wrong Password / Account Locked)

**Symptom:** Login returns an error message or redirects back to `/login`.

**Likely causes:**
- Password reset not applied by `EnsureDemoUserPasswordsAsync` (app not restarted after DB change)
- Account locked after failed attempts (`LockoutEnabled` may have changed)

**Mitigation steps:**
1. Restart the app — `EnsureDemoUserPasswordsAsync` runs on every startup and forces `TempPass123!`
2. Confirm the email exactly: `student1@univ.edu` (no trailing space)
3. If locked: open SQL Server and run `UPDATE AspNetUsers SET LockoutEnd = NULL WHERE Email = 'student1@univ.edu'`

**Fallback:** Show a slide with the credentials and state *"The accounts are seeded on startup with a known password via `EnsureDemoUserPasswordsAsync`."* Continue with a working account if only one is affected.

---

### Scenario 5 — Browser / Projector Issue

**Symptom:** Browser crashes, projector loses signal, or display resolution breaks the layout.

**Mitigation steps:**
1. Reopen the browser and navigate directly to the last URL from the demo script
2. Adjust projector resolution: set to 1920×1080 or 1280×720 — both supported
3. Use the second pre-loaded browser tab if the first is corrupted

**Fallback:** Switch immediately to **offline screenshots**. Narrate the screenshots in the same order as the demo script phases.

---

## Offline Fallback Screenshots

Capture these screenshots during the PR-09 dress rehearsal. Save to `C:\demo-fallback\` with the filenames below.

| # | Filename | Page / State | When to Show |
|---|---|---|---|
| 01 | `01-login-page.png` | `/login` — empty form | Phase 0 fallback |
| 02 | `02-student-dashboard.png` | `/dashboard` — thesis assigned, status badge | Phase 1 fallback |
| 03 | `03-thesis-updates-empty.png` | `/updates` — empty timeline | Phase 1 fallback |
| 04 | `04-submit-update-form.png` | `/updates` — update form open with text entered | Phase 2 fallback |
| 05 | `05-file-upload-ready.png` | `/updates` — file selected in upload zone | Phase 2 fallback |
| 06 | `06-update-submitted.png` | `/updates` — new entry at top, status "Under Review" | Phase 2 fallback |
| 07 | `07-download-link.png` | `/updates` — download link on attachment entry | Phase 2 fallback |
| 08 | `08-professor-home.png` | `/` (Home) — professor view, student list | Phase 4 fallback |
| 09 | `09-professor-review-form.png` | `/updates` — supervisor view with review form open | Phase 4 fallback |
| 10 | `10-feedback-submitted.png` | `/updates` — entry with Approved badge + comment | Phase 4 fallback |
| 11 | `11-student-sees-feedback.png` | `/updates` — student view showing the Approved badge and professor comment | Phase 5 fallback |
| 12 | `12-thesis-topics-board.png` | `/thesis-topics` — topic cards with status badges | Phase 6 fallback |

**Capture instructions:**
1. Run the full golden flow once during rehearsal
2. At each numbered state above, press `Win+Shift+S` (Snip & Sketch) or `PrtScn` and save to `C:\demo-fallback\` with the exact filename

---

## Emergency Fallback Procedure

Activate if the app cannot be recovered within **3 minutes** of a failure.

1. Say: *"I'm going to continue with pre-captured screenshots to avoid losing time."*
2. Open `C:\demo-fallback\` in full-screen File Explorer or image viewer
3. Navigate photos in order, narrating exactly as the demo script specifies
4. After each phase, refer back to the slides for architecture and quality evidence context
5. Complete slides 7–10 (Tests, Performance, Security, Conclusions) normally
6. Offer to show the live system after Q&A if time allows

**Key message to preserve:** The system works as demonstrated; the screenshots were captured during a successful rehearsal run on the same machine.
