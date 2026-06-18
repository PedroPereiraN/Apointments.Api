# Appointments API

A REST API for managing users and appointments, built with ASP.NET Core on .NET 10. It includes a background job that automatically sends email reminders to users about their upcoming appointments.

## Tech Stack

- **[ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/) (.NET 10)** — web framework for building the REST API
- **[Entity Framework Core 10](https://learn.microsoft.com/en-us/ef/core/)** — ORM for database access
- **[SQLite](https://www.sqlite.org/)** — lightweight file-based database via `Microsoft.EntityFrameworkCore.Sqlite`
- **[MailKit 4.17](https://github.com/jstedfast/MailKit)** — email sending library (recommended replacement for the deprecated `SmtpClient`)
- **[DotNetEnv 3.2](https://github.com/tonerdo/dotnet-env)** — loads environment variables from a `.env` file

## How it Works

The API exposes two resources: **Users** and **Appointments**. Each appointment belongs to a user and has a scheduled date/time stored as `DateTimeOffset` to preserve timezone information.

A background job (`AppointmentReminderJob`) runs on two schedules:
- **At midnight** — sends an email to each user listing all their appointments for that day
- **Every hour** — checks for appointments happening in the next 3 hours and sends a reminder email

If a user has multiple appointments in the same window, they receive a single email listing all of them. The job is disabled automatically if SMTP environment variables are not configured.

## Getting Started

### 1. Clone the repository

```bash
git clone <your-repo-url>
cd Appointments.Api
```

### 2. Configure environment variables

Create a `.env` file in the project root (never commit this file):

```bash
TZ=America/Cuiaba

Smtp__Host=smtp.gmail.com
Smtp__Port=587
Smtp__Username=your@gmail.com
Smtp__Password=your-app-password
Smtp__From=your@gmail.com
```

> If you use Gmail, `Smtp__Password` must be an **App Password**, not your account password.
> Generate one at: Google Account → Security → 2-Step Verification → App passwords.
>
> If the `.env` file is missing or incomplete, the app will still run but email reminders will be disabled.

### 3. Run the migrations

```bash
dotnet ef database update
```

This creates the `Appointments.db` SQLite file with all the required tables.

### 4. Run the project

```bash
dotnet run
```

The API will be available at `http://localhost:<port>` (check the terminal output for the exact port).

---

## API Routes

### Users

| Method | Route | Description | Body |
|--------|-------|-------------|------|
| `GET` | `/users` | List all users | — |
| `GET` | `/users/{id}` | Get a user by ID | — |
| `POST` | `/users` | Create a user | `{ "name": "string", "email": "string" }` |
| `PUT` | `/users/{id}` | Update a user | `{ "name": "string", "email": "string" }` |
| `DELETE` | `/users/{id}` | Delete a user | — |

### Appointments

| Method | Route | Description | Body |
|--------|-------|-------------|------|
| `GET` | `/appointments` | List all appointments | — |
| `GET` | `/appointments/{id}` | Get an appointment by ID | — |
| `POST` | `/appointments` | Create an appointment | `{ "description": "string", "scheduledAt": "2026-06-20T14:00:00Z", "userId": 1 }` |
| `PUT` | `/appointments/{id}` | Update an appointment | `{ "description": "string", "scheduledAt": "2026-06-20T14:00:00Z" }` |
| `DELETE` | `/appointments/{id}` | Delete an appointment | — |
| `POST` | `/appointments/remind-today` | Manually trigger today's reminders for all users | — |

> **Note:** `scheduledAt` must be a valid ISO 8601 datetime with timezone offset (e.g. `2026-06-20T14:00:00Z` for UTC or `2026-06-20T14:00:00-03:00` for UTC-3).
