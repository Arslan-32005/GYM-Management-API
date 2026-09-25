# Gym Management Web API

A RESTful Gym Management API built with ASP.NET Core Web API, Entity Framework Core (Code First), and SQL Server.

# Tech Stack
ASP.NET Core Web API / C#
Entity Framework Core (Code First, Migrations)
SQL Server
LINQ, Fluent API, async/await
# Project Structure
GYM Management API
├── Controllers/
│   ├── MemberController.cs
│   ├── TrainerController.cs
│   ├── MembershipController.cs
│   ├── WorkoutPlanController.cs
│   └── MemberWorkoutPlanController.cs
├── Models/
│   ├── Member.cs
│   ├── Trainer.cs
│   ├── Membership.cs
│   ├── WorkoutPlan.cs
│   └── MemberWorkoutPlan.cs
├── Data/
│   └── GymContext.cs
└── Migrations/
# Entities & Relationships
Trainer 1 → * Member (optional assignment, delete restricted while members exist)
Member 1 → * Membership (cascade delete)
Member 1 → * MemberWorkoutPlan (cascade delete)
WorkoutPlan 1 → * MemberWorkoutPlan (delete restricted while assignments exist)
Member * ↔ * WorkoutPlan via the MemberWorkoutPlan junction entity
Fluent API Configuration (GymContext.OnModelCreating)
Relationship + delete-behavior setup for all four relationships above
Unique composite index on MemberWorkoutPlan(MemberId, WorkoutPlanId) — prevents duplicate assignment at the database level
Membership.Fee precision set to decimal(8,2)
Max-length constraints on string fields (FullName, Email, PhoneNumber, PlanName, etc.)
Validation
Data Annotations on models: [Required], [EmailAddress], [Range]
Business rules enforced in controllers (not achievable via annotations alone):
Trainer must exist and be available before assignment
Membership EndDate must be after StartDate
No overlapping active memberships for the same member
Inactive member cannot be assigned a workout plan
No duplicate Member + WorkoutPlan assignment
Trainer / WorkoutPlan cannot be deleted while referenced
Endpoints (per controller: Member, Trainer, Membership, WorkoutPlan, MemberWorkoutPlan)
GET    /api/{Resource}            (search / filter / sort / pagination)
GET    /api/{Resource}/{id}
POST   /api/{Resource}
PUT    /api/{Resource}/{id}
DELETE /api/{Resource}/{id}

# Example:

GET /api/Member?search=Ali&isActive=true&trainerId=2&page=1&pageSize=5
Async Data Access
All EF Core calls use async methods: ToListAsync(), FirstOrDefaultAsync(), FindAsync(), AnyAsync(), CountAsync(), SaveChangesAsync().
Pagination
Skip = (page - 1) × pageSize
Response includes items, page, pageSize, totalItems, totalPages.

# Projections (DTO-style)

GET endpoints project entities into anonymous objects (e.g. Select(m => new { ... })) instead of returning full entities — avoids circular-reference issues from navigation properties and controls exactly what fields the client sees.

HTTP Status Codes
Code	Usage
200 OK	Successful GET/PUT
201 Created	Successful POST
204 No Content	Successful DELETE
400 Bad Request	Validation / business-rule violation
404 Not Found	Resource does not exist
