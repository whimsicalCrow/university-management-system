---
title: "STORY-US-023: Multilingual UI — English and Greek (i18n)"
epic: "User Experience"
sprint: "Post-Presentation"
priority: "P2 - Medium"
status: "not-started"
date_created: 2026-07-04
baseline_commit: "d5978adb30c846d06a4f4723a2beeba8636e67ed"
branch: "feature/us-023-i18n-en-el"
presentation_task: null
---

# STORY-US-023: Multilingual UI — English and Greek (i18n)

**Epic:** User Experience  
**Sprint:** Post-Presentation  
**Priority:** P2 — Medium  
**Estimate:** 6 hours  
**Acceptance Criteria Count:** 5 (AC1–AC5)

---

## User Story

**As a** student or professor  
**I want** to switch the application UI between English and Greek  
**So that** I can use the system comfortably in my preferred language

---

## Context & Dependencies

### Current State

The UI is currently in mixed language: some labels are English ("Thesis Topics", "Submit Update", "Approved"), some are Greek ("Χρονογραμμή Ενημερώσεων", "Αρχική"), and navigation items are inconsistently split. There is no localization infrastructure in the project.

### Scope

| In scope | Out of scope |
|---|---|
| All user-facing UI strings in Razor components | Database content (thesis titles, update bodies, feedback text) |
| Navigation, labels, buttons, status badges, empty states, error messages | Admin/configuration pages |
| Language toggle persisted per browser session (cookie) | Third-party library strings (Bootstrap, MediatR error messages) |
| Two locales: `en` (English default), `el` (Greek) | More than two languages |

### Pages and Components to Localize

| File | Key strings |
|---|---|
| `Components/Layout/NavMenu.razor` | Nav links, brand name, sign out |
| `Components/Pages/Login.razor` | Page title, labels, button, error messages |
| `Components/Pages/Home.razor` | Section heading, subtitle, metric labels, quick-nav buttons |
| `Components/Pages/StudentDashboard.razor` | Heading, status labels, supervisor, empty state |
| `Components/Pages/ThesisTopics.razor` | Headings, form labels, topic status badges, buttons, empty states |
| `Components/Pages/ThesisUpdates.razor` | Headings, status badges, form labels, feedback section, empty states |
| `Components/Pages/Meetings.razor` | Heading, empty state |

### .NET Localization Approach

ASP.NET Core built-in localization via `Microsoft.Extensions.Localization`:

- `AddLocalization()` + `UseRequestLocalization()` in `Program.cs`
- Supported cultures: `en` (default), `el`
- Culture selected from: (1) cookie `ui-culture`, (2) `Accept-Language` header, (3) default `en`
- Resource files: one shared file `Resources/Shared.en.resx` + `Resources/Shared.el.resx` for cross-component strings (status badges, common buttons); per-component files for page-specific strings
- `IStringLocalizer<T>` injected via `@inject` in each Razor component
- Language switcher: a `<select>` or two flag buttons in NavMenu that POSTs to a culture endpoint setting the `ui-culture` cookie

### Resource File Naming

```
University.Web/
  Resources/
    Shared.en.resx          ← default (English keys = values)
    Shared.el.resx          ← Greek translations
    Pages/
      Login.el.resx
      Home.el.resx
      StudentDashboard.el.resx
      ThesisTopics.el.resx
      ThesisUpdates.el.resx
      Meetings.el.resx
    Layout/
      NavMenu.el.resx
```

> **Convention:** English resource files (`*.en.resx`) are optional when key = English value and the default culture is `en`. Only Greek translation files are strictly required. English keys fall back to the key name automatically when no `.en.resx` is present.

### Culture Cookie Endpoint

```csharp
// Program.cs — minimal culture-switch endpoint
app.MapGet("/culture", (string culture, string redirectUri, HttpContext ctx) =>
{
    ctx.Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
    return Results.LocalRedirect(redirectUri);
});
```

### Dependencies

- No new NuGet packages required (`Microsoft.Extensions.Localization` ships with ASP.NET Core)
- Blazor Server re-renders on culture change automatically when circuit refreshes
- `UseRequestLocalization` must be added before `UseRouting` / `MapBlazorHub`

---

## Acceptance Criteria

### AC1: Localization Infrastructure Configured

**Given** the `Program.cs` is updated,  
**When** the application starts,  
**Then**:
- `AddLocalization(o => o.ResourcesPath = "Resources")` is registered
- `UseRequestLocalization` is configured with supported cultures `["en", "el"]`, default culture `en`, and `CookieRequestCultureProvider` as the first provider
- The culture-switch GET endpoint `/culture` is mapped and correctly sets the `CultureInfo.CurrentCulture` and `CurrentUICulture` cookie

### AC2: Language Switcher in NavMenu

**Given** any authenticated user is on any page,  
**When** they look at the navigation bar,  
**Then** a language toggle (two buttons or a `<select>`) is visible showing `EN` / `ΕΛ`  
**And** clicking a language option navigates to `/culture?culture={code}&redirectUri={currentPath}`, setting the cookie and reloading the page in the selected language  
**And** the active language is visually distinguished (bold, underline, or active class)

### AC3: All Core Pages Localized

**Given** the locale is set to `el`,  
**When** a user navigates to Login, Home, StudentDashboard, ThesisTopics, ThesisUpdates, or Meetings,  
**Then** all visible UI strings (headings, labels, buttons, empty-state messages, status badge text, validation messages) render in Greek  
**And** when locale is `en`, the same strings render in English

### AC4: No Hardcoded UI Strings Remain in Razor Files

**Given** the localization is applied,  
**When** a developer inspects any localized Razor component,  
**Then** every user-visible string that varies by language is replaced with `@L["KeyName"]` (or equivalent `IStringLocalizer` call)  
**And** no raw English or Greek string literals appear outside of `@L[...]` calls for translatable content  
**Exception:** Code-only constants (`const string`, status comparisons), route values, CSS classes, and aria labels that are language-invariant are excluded

### AC5: Build and Tests Pass

**Given** all resource files and code changes are in place,  
**When** `dotnet build` and `dotnet test` are run,  
**Then** 0 compile errors, 0 test failures (97 tests baseline)

---

## Technical Context

### Key Strings Inventory (Selected)

| Component | English | Greek key suggestion |
|---|---|---|
| NavMenu | Thesis Topics | Θέματα Διπλωματικής |
| NavMenu | Thesis Updates | Ενημερώσεις Διπλωματικής |
| NavMenu | Meetings | Συναντήσεις |
| NavMenu | Sign out | Αποσύνδεση |
| Login | Sign in to continue | Συνδεθείτε για να συνεχίσετε |
| Login | Email | Email |
| Login | Password | Κωδικός |
| Login | Sign In | Σύνδεση |
| Home | Welcome to Thesis Collaboration | Καλώς ήρθατε στη Συνεργασία Διπλωματικών |
| Home | Active Supervisions | Ενεργές Επιβλέψεις |
| Home | Pending Reviews | Εκκρεμείς Αξιολογήσεις |
| Home | Upcoming Meetings | Επερχόμενες Συναντήσεις |
| ThesisUpdates | Under review | Υπό Αξιολόγηση |
| ThesisUpdates | Approved | Εγκεκριμένο |
| ThesisUpdates | Needs Revision | Χρειάζεται Αναθεώρηση |
| ThesisUpdates | No ongoing theses | Δεν υπάρχουν ενεργές διπλωματικές |
| ThesisTopics | Thesis Topics | Θέματα Διπλωματικής |
| ThesisTopics | Draft / Open / Assigned / Archived | Πρόχειρο / Ανοιχτό / Ανατεθειμένο / Αρχειοθετημένο |
| StudentDashboard | Under Supervision | Υπό Επίβλεψη |

### Blazor Server Culture Refresh

Blazor Server circuits capture the culture at circuit creation. A full page reload (not a Blazor navigation) is required for culture changes to take effect. The `/culture` endpoint redirects to the originating URL with a hard reload, so the new Blazor circuit picks up the updated cookie.

### Shared vs Per-Component Localizer

Use a shared `IStringLocalizer<SharedResources>` (marker class `SharedResources` in `University.Web`) for strings used in multiple components (status badges, common button labels). Use per-component localizers for page-specific strings.

### File Plan

```
University.Web/
  Resources/
    Shared.el.resx
    Pages/Login.el.resx
    Pages/Home.el.resx
    Pages/StudentDashboard.el.resx
    Pages/ThesisTopics.el.resx
    Pages/ThesisUpdates.el.resx
    Pages/Meetings.el.resx
    Layout/NavMenu.el.resx
```

---

## Tasks / Subtasks

### Task 1: Infrastructure (AC1)
- [ ] 1.1 Add `AddLocalization(o => o.ResourcesPath = "Resources")` to `Program.cs`
- [ ] 1.2 Add `UseRequestLocalization` with cultures `["en", "el"]`, default `en`, `CookieRequestCultureProvider` first
- [ ] 1.3 Add the `/culture` cookie-setting endpoint
- [ ] 1.4 Add `SharedResources.cs` marker class

### Task 2: Resource Files (AC3, AC4)
- [ ] 2.1 Create `Resources/Shared.el.resx` — status badges and common button labels
- [ ] 2.2 Create `Resources/Layout/NavMenu.el.resx`
- [ ] 2.3 Create `Resources/Pages/Login.el.resx`
- [ ] 2.4 Create `Resources/Pages/Home.el.resx`
- [ ] 2.5 Create `Resources/Pages/StudentDashboard.el.resx`
- [ ] 2.6 Create `Resources/Pages/ThesisTopics.el.resx`
- [ ] 2.7 Create `Resources/Pages/ThesisUpdates.el.resx`
- [ ] 2.8 Create `Resources/Pages/Meetings.el.resx`

### Task 3: Component Updates (AC3, AC4)
- [ ] 3.1 Update `NavMenu.razor` — inject `IStringLocalizer`, replace hardcoded strings, add language switcher
- [ ] 3.2 Update `Login.razor` — inject localizer, replace strings
- [ ] 3.3 Update `Home.razor` — inject localizer, replace strings
- [ ] 3.4 Update `StudentDashboard.razor` — inject localizer, replace strings
- [ ] 3.5 Update `ThesisTopics.razor` — inject localizer, replace strings and status badge labels
- [ ] 3.6 Update `ThesisUpdates.razor` — inject localizer, replace strings and status badge labels
- [ ] 3.7 Update `Meetings.razor` — inject localizer, replace strings

### Task 4: Language Switcher (AC2)
- [ ] 4.1 Add EN / ΕΛ toggle to `NavMenu.razor` (visible when authenticated)
- [ ] 4.2 Style active language state
- [ ] 4.3 Verify cookie persists across page navigations

### Task 5: Build + Test Gate (AC5)
- [ ] 5.1 Run `dotnet build UniversitySystem.sln -c Release` — confirm 0 errors
- [ ] 5.2 Run `dotnet test` — confirm 97/97 pass
- [ ] 5.3 Manual smoke test: toggle language on Login, Home, ThesisUpdates; verify Greek renders correctly

---

## Dev Agent Record

### Implementation Notes

- Add `@using Microsoft.Extensions.Localization` to `_Imports.razor` so all components can use `IStringLocalizer` without per-file usings.
- Status badge strings (`Approved`, `Under review`, `Needs Revision`) are compared case-insensitively in code — the `IStringLocalizer` should be used only for **display**, not for status comparisons. Keep the `ThesisUpdateStatuses` constants in English as the internal code values.
- The `/culture` endpoint must be a GET (not a POST) to work with plain anchor `<a href="">` navigation from Blazor without JavaScript interop.
- Test the Greek locale by setting `?culture=el` or using the toggle; verify the `.el.resx` files are embedded correctly in the build output.

### Open Questions

- [ ] OQ-1: Should the language preference be per-user (stored in DB) or per-browser (cookie only)? Current design uses cookie for simplicity — escalate if per-user persistence is required.
- [ ] OQ-2: Should the default language be Greek (since the department is Greek-speaking) rather than English? Decision affects the default culture and which `.resx` is the fallback.

---

## File List

### To Create
- `University.Web/Resources/SharedResources.cs` — marker class
- `University.Web/Resources/Shared.el.resx`
- `University.Web/Resources/Pages/Login.el.resx`
- `University.Web/Resources/Pages/Home.el.resx`
- `University.Web/Resources/Pages/StudentDashboard.el.resx`
- `University.Web/Resources/Pages/ThesisTopics.el.resx`
- `University.Web/Resources/Pages/ThesisUpdates.el.resx`
- `University.Web/Resources/Pages/Meetings.el.resx`
- `University.Web/Resources/Layout/NavMenu.el.resx`

### To Modify
- `University.Web/Program.cs` — AddLocalization, UseRequestLocalization, /culture endpoint
- `University.Web/_Imports.razor` — add `@using Microsoft.Extensions.Localization`
- `University.Web/Components/Layout/NavMenu.razor` — localizer + language switcher
- `University.Web/Components/Pages/Login.razor` — localizer
- `University.Web/Components/Pages/Home.razor` — localizer
- `University.Web/Components/Pages/StudentDashboard.razor` — localizer
- `University.Web/Components/Pages/ThesisTopics.razor` — localizer
- `University.Web/Components/Pages/ThesisUpdates.razor` — localizer
- `University.Web/Components/Pages/Meetings.razor` — localizer
