# Intro to Docker Networking

## 1) Τι σημαίνει internal και external network

Στο Docker (και ειδικά στο Docker Compose), τα containers συνδέονται σε virtual δίκτυα.

- Internal network (ιδιωτικό για το stack):
  - Συνήθως δημιουργείται αυτόματα από το Compose.
  - Τα services μιλάνε μεταξύ τους με όνομα service (DNS του Docker).
  - Δεν είναι προσβάσιμο από έξω, εκτός αν κάνεις publish port.

- External network (κοινόχρηστο, προϋπάρχον):
  - Το δίκτυο υπάρχει ήδη και το Compose απλά συνδέει services εκεί.
  - Χρήσιμο όταν θέλεις επικοινωνία ανάμεσα σε διαφορετικά projects/stacks.
  - Επιτρέπει π.χ. reverse proxy stack να βλέπει apps από άλλο stack.

Σημείωση: Στο Compose, internal/external δεν σημαίνει από μόνο του internet/no-internet. Το βασικό για πρόσβαση από έξω είναι αν έχεις published ports.

## 2) Τι είναι endpoints

Η λέξη endpoint χρησιμοποιείται σε δύο επίπεδα:

- Network endpoint (Docker πλευρά):
  - Το σημείο σύνδεσης ενός container σε ένα Docker network.
  - Κάθε container έχει IP μέσα στο network και DNS όνομα.

- Application endpoint (HTTP πλευρά):
  - Μια διαδρομή API, π.χ. /api/students.
  - Για να τη δεις από host/internet, χρειάζεται publish του container port.

Παράδειγμα:

- Η εφαρμογή ακούει μέσα στο container στο port 8080.
- Αν βάλεις ports: 5000:8080, τότε από host πας στο localhost:5000.

## 3) Πως επικοινωνούν τα containers

- Στο ίδιο Docker network:
  - Μιλάνε απευθείας με service name, π.χ. http://db:5432 ή http://api:8080.
  - Δεν χρειάζεται publish port για αυτή την εσωτερική επικοινωνία.

- Σε διαφορετικά networks:
  - Δεν βλέπονται, εκτός αν κάποιο container είναι συνδεδεμένο και στα δύο.

- Από host προς container:
  - Γίνεται μέσω published port (ports mapping).

## 4) Τι γίνεται με το traffic

### Ingress (μπαίνει traffic από έξω προς container)

Ροή:

1. Ο client στέλνει request στο host IP:port.
2. Ο Docker engine κάνει NAT/forwarding προς το σωστό container IP:port.
3. Η εφαρμογή απαντά και η απάντηση επιστρέφει μέσω host.

### East-West (container προς container)

- Η κίνηση μένει μέσα στο Docker virtual network.
- Χρήση DNS names των services.

### Egress (container προς internet)

- Το container βγαίνει προς τα έξω μέσω host networking/NAT.
- Συνήθως δουλεύει χωρίς extra ρύθμιση.

## 5) Γιατί το host.docker.internal δουλεύει στο PC σου

Στο Docker Desktop (Windows/macOS), το host.docker.internal είναι ειδικό DNS name που δείχνει στον host machine.

Τι σημαίνει πρακτικά:

- Από μέσα στο container μπορείς να καλέσεις υπηρεσία που τρέχει στο host.
- Π.χ. app στο container να μιλήσει με DB που τρέχει local στο laptop.

Γιατί παίζει στο PC σου:

- Επειδή έχεις Docker Desktop σε Windows, που το υποστηρίζει by default.

Στο Linux συνήθως θέλει επιπλέον ρύθμιση (π.χ. host-gateway mapping), άρα δεν είναι πάντα αυτόματα διαθέσιμο όπως σε Docker Desktop.

## 6) Μικρό Compose παράδειγμα

services:
  api:
    build: .
    ports:
      - "5000:8080"
    networks:
      - app_net

  db:
    image: postgres:16
    networks:
      - app_net

networks:
  app_net:
    driver: bridge

Με αυτό:

- Από host: localhost:5000 -> api:8080
- Από api προς db: db:5432 (χωρίς publish)

## 7) Γρήγορο rule of thumb

- Publish ports μόνο για ό,τι πρέπει να είναι προσβάσιμο εκτός Docker.
- Βάλε app και db στο ίδιο internal network.
- Χρησιμοποίησε external network όταν θες cross-stack επικοινωνία.
- Για πρόσβαση από container προς host σε Windows Docker Desktop, χρησιμοποίησε host.docker.internal.
