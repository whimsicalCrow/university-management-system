# PR-08 Demo Script — University Thesis Management System

**Date:** 2026-07-09 (rehearsal) / 2026-07-10 (live)  
**Total budget:** ≤ 12 minutes  
**App URL:** `http://localhost:5118`  
**Password (all accounts):** `TempPass123!`

---

## Pre-Flight Checklist

Complete all items before entering the room. Mark each ✅.

| # | Check | Expected Result |
|---|---|---|
| 1 | Docker container running | `docker ps` shows `sql` container, port 1433 |
| 2 | App started via `.\scripts\start-demo.ps1` | Banner prints `http://localhost:5118`, no errors |
| 3 | Browser window open at `http://localhost:5118/login` | Login page renders, no 500 error |
| 4 | Browser zoom set to 125% | Text legible on projector |
| 5 | Second browser tab open at `/login` (professor) | Ready for role switch |
| 6 | Credentials visible on notepad (off-screen) | `student1@univ.edu` / `prof1@univ.edu` / `TempPass123!` |
| 7 | A test file on Desktop ready for upload | `thesis-update-v1.pdf` (any small PDF, ≤ 5 MB) |
| 8 | Fallback screenshots folder open in File Explorer | `C:\demo-fallback\` |
| 9 | Demo script open on phone or second screen | This file |
| 10 | Presentation slides advanced to "Live Demo" slide | Slide deck ready |

---

## Demo Script

### Phase 0 — Intro (on slides, 0:00 – 2:00)

Deliver slides: Title → Problem → Architecture → Tech Stack → Feature Overview.  
**Cue to switch to browser:** *"Let me show you the system in action."*

---

### Phase 1 — Student Login and Dashboard (2:00 – 4:30)

| Step | Action | Narration | Time |
|---|---|---|---|
| 1 | Click the browser tab showing `/login` | — | 2:00 |
| 2 | Enter `student1@univ.edu` in the Email field | *"I'll log in as a student — student1."* | 2:05 |
| 3 | Enter `TempPass123!` in the Password field | — | 2:10 |
| 4 | Click **Sign In** | *"The system authenticates via ASP.NET Core Identity and redirects to the student dashboard."* | 2:12 |
| 5 | Dashboard loads — point to the thesis title and status badge | *"The student can immediately see their assigned thesis topic, current status, and their supervisor — here, Επίκ. Χριστοδούλου."* | 2:20 |
| 6 | Click **Thesis Updates** in the navigation | *"The timeline shows all progress updates submitted so far."* | 2:35 |
| 7 | Scroll through any existing entries | *"Each entry captures the date, content, and the professor's feedback status."* | 2:45 |

---

### Phase 2 — Student Submits a Thesis Update with Attachment (2:45 – 5:30)

| Step | Action | Narration | Time |
|---|---|---|---|
| 8 | Click **Submit Update** (or the new-update form trigger) | *"The student submits a progress note. This is how they keep their supervisor informed."* | 2:50 |
| 9 | Type in the update text field: `Completed literature review. Draft chapter 1 attached.` | *"They can write rich text — markdown is supported."* | 3:00 |
| 10 | Click **Choose file** / drag `thesis-update-v1.pdf` onto the upload zone | *"Attachments — PDF, DOCX, PPTX, or ZIP — up to 20 MB. The file is stored in the configured storage backend."* | 3:20 |
| 11 | Click **Submit** | *"One click. The system validates the file type and size, stores the artifact, and persists the update."* | 3:35 |
| 12 | Point to the new entry at the top of the timeline | *"The update is immediately visible with an 'Under Review' status badge. The attachment is linked as a signed download token — time-limited, tamper-proof."* | 3:50 |
| 13 | Click the download link on the attachment | *"The token expires in 15 minutes; anyone with a stale link gets a 404."* | 4:05 |

---

### Phase 3 — Role Switch to Professor (5:30 – 6:00)

| Step | Action | Narration | Time |
|---|---|---|---|
| 14 | Click the second browser tab (pre-loaded at `/login`) | *"Now I'll switch to the professor's perspective — Επίκ. Χριστοδούλου Σωτήριος."* | 5:30 |
| 15 | Enter `prof1@univ.edu` / `TempPass123!` and click **Sign In** | — | 5:35 |

---

### Phase 4 — Professor Reviews Update and Submits Feedback (6:00 – 8:30)

| Step | Action | Narration | Time |
|---|---|---|---|
| 16 | Home page loads — point to the student list / thesis queue | *"The professor sees a list of their supervised students with pending updates highlighted."* | 6:05 |
| 17 | Click on the student row for student1 | *"Clicking a student opens their full thesis timeline."* | 6:15 |
| 18 | Point to the update just submitted | *"Here's the update student1 just submitted."* | 6:25 |
| 19 | Click **Review** on the update | *"The professor clicks Review to open the feedback form."* | 6:35 |
| 20 | Select status **Approved** from the dropdown | *"Status can be Approved or Needs Revision."* | 6:45 |
| 21 | Type in the comment field: `Good progress. Please expand the related work section.` | *"Free-text feedback — rendered as markdown on the student side."* | 6:55 |
| 22 | Click **Submit Feedback** | *"Persisted immediately to the database."* | 7:10 |
| 23 | Point to the updated status badge on the timeline entry | *"The status flips to Approved in real time."* | 7:20 |

---

### Phase 5 — Student Sees Feedback (8:30 – 9:30)

| Step | Action | Narration | Time |
|---|---|---|---|
| 24 | Switch back to the first browser tab (student session) | *"Back to the student view — no page reload needed."* | 8:30 |
| 25 | Refresh / navigate back to Thesis Updates | — | 8:35 |
| 26 | Point to the Approved badge and the professor's comment | *"The feedback is visible immediately. The student sees the status change and the professor's exact comment."* | 8:45 |

---

### Phase 6 — Thesis Topics (Optional, 9:30 – 10:30)

| Step | Action | Narration | Time |
|---|---|---|---|
| 27 | Navigate to **Thesis Topics** | *"Finally, the Thesis Topics board — where professors publish topics and students express interest."* | 9:30 |
| 28 | Point to the topic cards and status badges (Draft / Open / Assigned / Archived) | *"Topics move through a lifecycle. Once a student is assigned, the topic closes automatically."* | 9:45 |
| 29 | (If time allows) Click **Express Interest** on an Open topic | *"Interest is persisted; the professor sees the queue and picks the student."* | 10:00 |

---

### Phase 7 — Quality Evidence (on slides, 10:30 – 12:00)

Switch back to slides: Test Results → Performance → Security → Conclusions → Q&A.

**Speaker note:** *"The system has 97 automated tests — 82 unit tests with bunit, 15 integration tests with EF Core InMemory. Performance load tests show login p95 at 89 ms and page-load p95 at 175 ms under 10 concurrent virtual users. An OWASP pass patched 4 high-severity findings before the demo."*

---

## Timing Summary

| Phase | Planned End | Actual (rehearsal) |
|---|---|---|
| Phase 0 — Slides intro | 2:00 | |
| Phase 1 — Student dashboard | 4:30 | |
| Phase 2 — Submit update | 5:30 | |
| Phase 3 — Role switch | 6:00 | |
| Phase 4 — Professor feedback | 8:30 | |
| Phase 5 — Student sees feedback | 9:30 | |
| Phase 6 — Thesis topics (optional) | 10:30 | |
| Phase 7 — Slides + Q&A | 12:00 | |

Fill in "Actual" column during PR-09 dress rehearsal.
