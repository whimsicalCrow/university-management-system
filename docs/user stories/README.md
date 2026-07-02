# User Stories Area

This folder contains product-level user stories organized by domain.

## Structure

- `Architecture/`
- `Backend/`
- `UI/`
- `Integration/`
- `Infrastructure/`
- `DevOps/`
- `Security/`
- `Documentation/`

## Canonical Tracking Files

- `STORY-STATUS.md`: single source of truth for current status and confidence.
- `Story Priority List with Dependencies.txt`: ordered backlog list.

## Story File Format (Normalized)

Each story file should keep this section order:

1. `Title`
2. `Persona`
3. `Description`
4. `Acceptance Criteria`
5. `Tasks`
6. `Implementation Notes` (optional, evidence-based)

## Status Vocabulary

- `Completed (Verified)`: implemented and validated against current codebase.
- `Partially Implemented`: some scope delivered, acceptance criteria still open.
- `Planned`: not yet implemented.
- `Needs Verification`: file contains historical implementation notes that are not present in current codebase.

## Normalization Notes (2026-07-02)

- Legacy ad-hoc `Dev Agent Record` sections were normalized into `Implementation Notes` where present.
- A status matrix was added to reduce ambiguity between planning docs and actual implementation state.