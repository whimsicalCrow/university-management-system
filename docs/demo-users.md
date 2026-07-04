# Demo Users

**All users share the same password: `TempPass123!`**

Login endpoint: `POST /api/auth/login` — body `{ "email": "...", "password": "..." }`  
Portal URL: `http://localhost:5118`

---

## Professors (Role: `Professor`)

| # | Email | Full Name | Βαθμίδα | Γνωστικό Αντικείμενο |
|---|-------|-----------|---------|----------------------|
| 1 | `prof1@univ.edu` | Χριστοδούλου Σωτήριος | Επίκ. Καθηγητής | Τεχνολογία Λογισμικού για τον Παγκόσμιο Ιστό |
| 2 | `prof2@univ.edu` | Πετρέλλης Νικόλαος | Αναπλ. Καθηγητής | Ενσωματωμένα Συστήματα |
| 3 | `prof3@univ.edu` | Χαραλαμπάκος Βασίλειος | Επίκ. Καθηγητής | Ηλεκτρικά Συστήματα Ενέργειας |
| 4 | `prof4@univ.edu` | Κούτρας Αθανάσιος | Αναπλ. Καθηγητής | Ψηφιακή Επεξεργασία Ήχου και Εικόνας |
| 5 | `prof5@univ.edu` | Τζήμας Ιωάννης | Αναπλ. Καθηγητής | Δικτυοκεντρικά Πληροφοριακά Συστήματα |

---

## Students (Role: `Student`)

| # | Email | Specialization | Supervisor (prof #) |
|---|-------|---------------|---------------------|
| 1  | `student1@univ.edu`  | Ηλεκτρονικής, Υπολογιστών και Συστημάτων | prof1 |
| 2  | `student2@univ.edu`  | Πληροφορικής                             | prof1 |
| 3  | `student3@univ.edu`  | Σημάτων, Τηλεπικοινωνιών και Δικτύων    | prof1 |
| 4  | `student4@univ.edu`  | Πληροφορικής                             | prof2 |
| 5  | `student5@univ.edu`  | Ενεργειακών Συστημάτων                   | prof2 |
| 6  | `student6@univ.edu`  | Σημάτων, Τηλεπικοινωνιών και Δικτύων    | prof2 |
| 7  | `student7@univ.edu`  | Ηλεκτρονικής, Υπολογιστών και Συστημάτων | prof3 |
| 8  | `student8@univ.edu`  | Σημάτων, Τηλεπικοινωνιών και Δικτύων    | prof3 |
| 9  | `student9@univ.edu`  | Ενεργειακών Συστημάτων                   | prof3 |
| 10 | `student10@univ.edu` | Πληροφορικής                             | prof4 |
| 11 | `student11@univ.edu` | Ηλεκτρονικής, Υπολογιστών και Συστημάτων | prof4 |
| 12 | `student12@univ.edu` | Ενεργειακών Συστημάτων                   | prof4 |
| 13 | `student13@univ.edu` | Σημάτων, Τηλεπικοινωνιών και Δικτύων    | prof5 |
| 14 | `student14@univ.edu` | Πληροφορικής                             | prof5 |
| 15 | `student15@univ.edu` | Ηλεκτρονικής, Υπολογιστών και Συστημάτων | prof5 |

---

## Notes

- Passwords are set at startup by `EnsureDemoUserPasswordsAsync` in `Program.cs` (overrides any hash stored in `seed-data.sql`).
- `LockoutEnabled = 0` for all users — failed login attempts do not lock accounts.
- Source of truth: `seed-data.sql` (user records) + `Program.cs` (password enforcement).
