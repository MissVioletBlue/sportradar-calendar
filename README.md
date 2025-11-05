# Sportradar Calendar

## Overview
This project is a small sports event calendar that I build. It uses ASP.NET Core 8 with Razor Components to show the official feed of upcoming sports. Users can see the schedule, check a spotlight event, and submit their own custom matches that stay in the list during the current session.

## Getting started
### Requirements
- .NET SDK 8.0 (see `global.json`)
- PostgreSQL 16 (local install or Docker)
- Node **not** required because the UI is plain Razor.

### Running with Docker Compose
1. Copy the `.env.sample` file to `.env` (or create one) and fill the values for Postgres and pgAdmin. The compose file expects `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`, `PGADMIN_DEFAULT_EMAIL`, and `PGADMIN_DEFAULT_PASSWORD`.
2. From the repo root run:
   ```bash
   docker compose up -d --build
   ```
3. The web site will be available at http://localhost:8080. PostgreSQL runs on port 5433 and pgAdmin on port 5050 by default.

### API quick view
- `GET /api/events?from=...` returns the filtered list of upcoming events.
- `GET /api/events/{id}` loads a single event by identifier.
- `POST /api/events` accepts `{ sportId, startsAt, title }` JSON. Validation happens server side and returns standard HTTP problem responses.

## Assumptions and decisions
- I assumed one shared tenant, so there is no user account system or personal storage. When a user submits a custom event from the UI it saves into the official event table immediately.
- Every event stores timestamps in UTC. On the client I convert to local time using `ToLocalTime()` for display.
- I let the backend sort events by start date, and the UI just renders the order it receives.
- The form success message and personal list link still show TODO comments. I kept them there so I remember the future improvements I want to do.
- Database migrations run on application startup. For bigger systems I would run them separately, but for this small project it keeps setup simple.