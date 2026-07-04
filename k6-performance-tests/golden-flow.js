/**
 * k6 load test — University Thesis Portal: Golden Demo Flow
 *
 * Covers the two user paths that make up the presentation golden flow:
 *   studentFlow : login → /dashboard → /updates → /thesis-topics
 *   professorFlow: login → / (home) → /thesis-topics → /updates
 *
 * Run:
 *   k6 run k6-performance-tests/golden-flow.js
 *
 * Prerequisites:
 *   - App running at http://localhost:5118 (see scripts/start-demo.ps1)
 *   - Demo accounts seeded: student1@univ.edu / prof1@univ.edu / Password123!
 *   - k6 installed: winget install k6  OR  choco install k6
 */

import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Trend, Counter, Rate } from 'k6/metrics';

// ── Custom metrics ────────────────────────────────────────────────────────────

const loginDuration    = new Trend('login_duration',    true);
const pageDuration     = new Trend('page_duration',     true);
const authErrors       = new Counter('auth_errors');
const pageErrors       = new Counter('page_errors');
const errorRate        = new Rate('error_rate');

// ── Configuration ─────────────────────────────────────────────────────────────

const BASE = __ENV.BASE_URL || 'http://localhost:5118';

const STUDENT_CREDS   = { email: 'student1@univ.edu',  password: 'TempPass123!' };
const PROFESSOR_CREDS = { email: 'prof1@univ.edu',      password: 'TempPass123!' };

// ── Thresholds ────────────────────────────────────────────────────────────────
// Targets calibrated to local demo machine (10 VUs, 40 s total)

export const options = {
  scenarios: {
    studentFlow: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '10s', target: 5 },   // ramp up
        { duration: '20s', target: 5 },   // hold
        { duration: '10s', target: 0 },   // ramp down
      ],
      exec: 'studentScenario',
      tags: { scenario: 'studentFlow' },
    },
    professorFlow: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '10s', target: 5 },
        { duration: '20s', target: 5 },
        { duration: '10s', target: 0 },
      ],
      exec: 'professorScenario',
      tags: { scenario: 'professorFlow' },
    },
  },

  thresholds: {
    // Login must be fast: bcrypt + DB round-trip
    'login_duration{scenario:studentFlow}':   ['p(95)<500'],
    'login_duration{scenario:professorFlow}': ['p(95)<500'],

    // Page cold-start (Blazor pre-render + EF queries)
    'page_duration{scenario:studentFlow}':    ['p(95)<1500'],
    'page_duration{scenario:professorFlow}':  ['p(95)<1500'],

    // Zero authentication failures
    'auth_errors':  ['count==0'],

    // Overall error rate < 1 %
    'error_rate':   ['rate<0.01'],

    // Standard k6 built-in: no 4xx/5xx
    'http_req_failed': ['rate<0.01'],
  },
};

// ── Helpers ───────────────────────────────────────────────────────────────────

function login(credentials, tag) {
  const start = Date.now();
  const res = http.post(
    `${BASE}/api/auth/login`,
    JSON.stringify(credentials),
    {
      headers: { 'Content-Type': 'application/json' },
      tags: { name: 'POST /api/auth/login', ...tag },
    }
  );
  loginDuration.add(Date.now() - start, tag);

  const ok = check(res, {
    'login status 200':  (r) => r.status === 200,
    'login success true': (r) => {
      try { return JSON.parse(r.body).success === true; } catch { return false; }
    },
  });

  if (!ok) {
    authErrors.add(1, tag);
    errorRate.add(1, tag);
  } else {
    errorRate.add(0, tag);
  }

  return res;
}

function getPage(jar, path, checkName, tag) {
  const start = Date.now();
  const res = http.get(`${BASE}${path}`, {
    jar,
    tags: { name: `GET ${path}`, ...tag },
    redirects: 5,
  });
  pageDuration.add(Date.now() - start, tag);

  const ok = check(res, {
    [`${checkName} status 200`]:    (r) => r.status === 200,
    [`${checkName} no 401/403`]:    (r) => r.status !== 401 && r.status !== 403,
    [`${checkName} no 500`]:        (r) => r.status < 500,
  });

  if (!ok) {
    pageErrors.add(1, tag);
    errorRate.add(1, tag);
  } else {
    errorRate.add(0, tag);
  }

  return res;
}

// ── Scenarios ─────────────────────────────────────────────────────────────────

export function studentScenario() {
  const tag = { scenario: 'studentFlow' };
  const jar = http.cookieJar();

  group('Student: login', () => { login(STUDENT_CREDS, tag); });
  group('Student: dashboard', () => { getPage(jar, '/dashboard', 'dashboard', tag); });
  sleep(0.5);
  group('Student: thesis updates', () => { getPage(jar, '/updates', 'updates', tag); });
  sleep(0.5);
  group('Student: thesis topics', () => { getPage(jar, '/thesis-topics', 'thesis-topics', tag); });
  sleep(1);
}

export function professorScenario() {
  const tag = { scenario: 'professorFlow' };
  const jar = http.cookieJar();

  group('Professor: login', () => { login(PROFESSOR_CREDS, tag); });
  group('Professor: home', () => { getPage(jar, '/', 'home', tag); });
  sleep(0.5);
  group('Professor: thesis topics', () => { getPage(jar, '/thesis-topics', 'thesis-topics', tag); });
  sleep(0.5);
  group('Professor: student updates (supervisor)', () => {
    getPage(jar, '/updates', 'updates-supervisor', tag);
  });
  sleep(1);
}
