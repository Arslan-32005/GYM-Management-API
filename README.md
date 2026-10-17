# Gym Management API

RESTful API for managing gym members, trainers, memberships and workout plans.
Built with ASP.NET Core 10 Web API, EF Core (Code First) and SQL Server.

## Features
- Identity registration/login with JWT Bearer authentication (2-hour tokens)
- Role-based authorization: Member (read) and Trainer (create/update/delete)
- Fluent API relationships, delete rules, and a unique index on MemberWorkoutPlan(MemberId, WorkoutPlanId)
- Business rules in controllers: no overlapping memberships, inactive members can't get plans, trainer/plan can't be deleted while referenced
- Pagination, search, filtering and sorting on list endpoints

## Tech Stack
ASP.NET Core Web API · EF Core · SQL Server · ASP.NET Core Identity · JWT Bearer · OpenAPI

## Setup
1. Install .NET 10 SDK and SQL Server (or SQL Server Express).
2. Set the connection string in `appsettings.json` (`ConnectionStrings:Default`).
3. Set the JWT secret (never commit it):
   `dotnet user-secrets set "Jwt:Key" "<at least 32 characters>"`
4. Apply migrations: `dotnet ef database update`
5. Run: `dotnet run` and open `/openapi/v1.json` in Development.

## Authentication
- `POST /api/auth/register` creates a user with the Member role.
- `POST /api/auth/login` returns `{ token, expiry }`.
- Send `Authorization: Bearer <token>` on every endpoint except register and login.

## Roles
| Role | Can |
|---|---|
| Member | Read all resources |
| Trainer | Read, create, update, delete |

### Creating a Trainer account
Trainer accounts are seeded at startup from user-secrets. Set them once:

dotnet user-secrets set "SeedTrainer:Email" "<email>"
dotnet user-secrets set "SeedTrainer:Password" "<password>"

The seed runs only if the email is not already registered.

## Endpoints (Member, Trainer, Membership, WorkoutPlan, MemberWorkoutPlan)
| Method | Route | Role |
|---|---|---|
| GET | /api/{resource} | Any authenticated user |
| GET | /api/{resource}/{id} | Any authenticated user |
| POST | /api/{resource} | Trainer |
| PUT | /api/{resource}/{id} | Trainer |
| DELETE | /api/{resource}/{id} | Trainer |

Example: GET /api/Member?search=Ali&isActive=true&trainerId=2&page=1&pageSize=5

## Status Codes
200 OK (GET/PUT) · 201 Created (POST) · 204 No Content (DELETE) · 400 Bad Request (validation/business rule) · 401 Unauthorized · 403 Forbidden (wrong role) · 404 Not Found

## Project Structure
Controllers/ · Models/ · DTOs/ · Data/GymContext.cs · Migrations/ · Auth.http (sample requests)

## Known Limitations
- Members can read all members and workout plans. There is no ownership check yet, because a Member account is not linked to its own Member record.
- Public registration always creates a Member account. Trainers are created only through the seed.
