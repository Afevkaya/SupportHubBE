# AGENTS.md

# SupportHub — Automated Coding Agent Guide

This document explains how coding agents should discover, build, extend, and maintain the SupportHub backend project.
The goal is to keep the architecture clean, scalable, production-oriented, and consistent as the project evolves.

---

# 1. Project Vision

SupportHub is a modern .NET backend project built to practice and apply:

* Clean / Onion Architecture
* CQRS pattern
* MediatR
* Repository pattern
* PostgreSQL
* Docker & containerization
* Logging & observability
* Redis caching
* RabbitMQ messaging
* SignalR realtime communication
* CI/CD pipelines
* Kubernetes deployment
* Production-grade backend practices

The project is intentionally developed incrementally.
Infrastructure and advanced tooling are added gradually as real needs appear.

Agents should avoid premature optimization or unnecessary abstractions.

---

# 2. Architecture Principles

The project follows a layered architecture inspired by Clean Architecture and Onion Architecture.

## Dependency Rule

Dependencies always point inward.

Outer layers may depend on inner layers.

Inner layers must NEVER depend on outer layers.

Example:

```text
Presentation -> Application -> Domain
Infrastructure -> Application
Persistence -> Application
```

Domain must remain independent from:

* EF Core
* ASP.NET Core
* Infrastructure concerns
* External services

---

# 3. Planned Solution Structure

```text
SupportHub.sln

src/
 ├── Core/
 │    ├── SupportHub.Domain
 │    └── SupportHub.Application
 │
 ├── Infrastructure/
 │    ├── SupportHub.Persistence
 │    ├── SupportHub.Infrastructure
 │    └── SupportHub.SignalR
 │
 └── Presentation/
      └── SupportHub.Api
```

> Note: The repository currently implements `SupportHub.Persistence` and `SupportHub.Infrastructure` under `src/Infrastructure/`. The optional `SupportHub.SignalR` project is planned in the architecture but is not present in this workspace. Agents should not assume SignalR artifacts exist unless you add them intentionally.
---

# 4. Layer Responsibilities

## SupportHub.Domain

Contains:

* Entities
* Value Objects
* Domain Enums
* Domain Rules
* BaseEntity definitions

Must NOT contain:

* EF Core
* HTTP logic
* Controllers
* Database implementations
* External dependencies

Example folders:

```text
Entities/
Enums/
Common/
```

---

## SupportHub.Application

Contains application business logic.

Includes:

* CQRS
* MediatR handlers
* DTOs
* Interfaces / abstractions
* Validation
* Use cases

Current feature areas in this repository are `Features/Auths` and `Features/Tickets`; auth commands live under `Commands/LoginUser` and `Commands/RegisterUser`, and ticket features currently include `Commands/CreateTicket`, `Commands/UpdateTicketStatus`, `Commands/AssignTicket`, `Commands/CreateTicketComment`, plus `Queries/Tickets/GetAllTickets`, `GetOpenTickets`, and `GetTicketDetail`.
Request DTOs live under `DTOs/Requests`, and response DTOs are grouped under `DTOs/Responses/Auths`, `DTOs/Responses/Tickets`, and `DTOs/Responses/Tokens`.

Example folders:

```text
Features/
Abstractions/
Behaviors/
Exceptions/
DTOs/
```

Feature structure example:

```text
Features/
 └── Tickets/
      ├── Commands/
      │    ├── CreateTicket/
      │    ├── UpdateTicketStatus/
      │    ├── AssignTicket/
      │    └── CreateTicketComment/
      └── Queries/
           └── Tickets/
                ├── GetAllTickets/
                ├── GetOpenTickets/
                └── GetTicketDetail/
```

CQRS naming conventions:

```text
CreateTicketCommand
CreateTicketCommandHandler
CreateTicketCommandResponse

GetAllTicketsQuery
GetAllTicketsQueryHandler
GetAllTicketsQueryResponse
```

---

## SupportHub.Persistence

Contains:

* EF Core
* DbContext
* Entity configurations
* Repository implementations
* Migrations

Example folders:

```text
Contexts/
Configurations/
Repositories/
Migrations/
```

---

## SupportHub.Infrastructure

Contains external service integrations.

Examples:

* Email services
* Redis
* RabbitMQ
* File storage
* JWT generation
* Logging services

Example folders:

```text
Services/
Logging/
Caching/
Messaging/
Authentication/
```

---

## SupportHub.Api

Application entry point.

Contains:

* Controllers
* Middleware
* Filters
* Dependency Injection registration
* API configuration

Must remain thin.

Business logic should NEVER live inside controllers.

---

# 5. CQRS & MediatR Rules

SupportHub uses CQRS with MediatR.

## Commands

Commands modify state.

Examples:

* CreateTicket
* UpdateTicketStatus
* DeleteTicket

Commands should:

* Return minimal responses
* Contain validation
* Avoid query responsibilities

---

## Queries

Queries only read data.

Queries:

* Must not modify state
* Should use read repositories
* Prefer projection instead of loading full entities

Cacheable queries implement `ICacheableQuery`; `CachingBehavior<TRequest, TResponse>` scopes the cache key with `ICurrentService.UserId` and uses the query's `Expiration`.
Application registration wires MediatR pipeline behaviors for validation, transactions, query performance, and caching through `AddApplicationServices()`. Command handlers run inside `TransactionBehavior<TRequest, TResponse>` and commit through `IUnitOfWork`; query handlers are timed by `QueryPerformanceBehavior<TRequest, TResponse>`.

---

## Handler Rules

Handlers live inside Application layer.

Handlers:

* Coordinate business flow
* Use abstractions/interfaces
* Must NOT directly use DbContext

Bad:

```csharp
public class CreateTicketHandler
{
    private readonly SupportHubDbContext _context;
}
```

Good:

```csharp
public class CreateTicketHandler
{
    private readonly ITicketWriteRepository _repository;
}
```

---

# 6. Repository Conventions

Repositories are separated into:

* Read repositories
* Write repositories

Base abstractions:

```text
IReadRepository<T>
IWriteRepository<T>
```

Entity-specific repositories:

```text
ITicketReadRepository
ITicketWriteRepository
```

Rules:

* Read operations -> Read repository
* Write operations -> Write repository
* Avoid massive generic repositories
* Keep repositories focused

In this repository, read repositories use `Dapper` + `Npgsql` for ticket read models, while write repositories use EF Core through `SupportHubDbContext`.
Current repository interfaces are `ITicketReadRepository`, `ITicketWriteRepository`, `ITicketCommentWriteRepository`, and `ITicketActivityWriteRepository`.

---

# 7. Entity Conventions

BaseEntity example:

```csharp
public class BaseEntity
{
    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }
}
```

Rules:

* Use Guid as primary key
* Use UTC time
* Prefer explicit enums

In the current domain model, `Ticket` carries its own optional `UpdatedDate` field; `BaseEntity` only supplies `Id` and `CreatedDate`.

Enum example:

```csharp
public enum TicketStatusType
{
    Open = 0,
    InProgress = 1,
    WaitingForResponse = 2,
    Closed = 3
}
```

Database enum strategy:

* Store enums as integers by default
* Use string storage only if business readability is critical

Repository-specific note: `SupportHubDbContext` currently maps `Ticket.Status`, `Ticket.Priority`, and `TicketActivity.ActivityType` to strings with `HasConversion<string>()`, so the read-side SQL in `TicketReadRepository` filters against string enum values.

---

# 8. Database & Persistence

Primary database:

* PostgreSQL

ORM:

* EF Core

Current implementation:

* Dapper is already used for read-heavy ticket queries in `src/Infrastructure/SupportHub.Persistence/Repositories/Tickets/TicketReadRepository.cs`

## Migration Commands

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Implementation notes for this repository:

- Migrations are already present under `src/Infrastructure/SupportHub.Persistence/Migrations/` (multiple timestamped migration files including Identity and ticket-related migrations). Prefer using the existing migrations unless you intentionally create a new migration.
- The persistence registration uses `AddPersistenceServices(this IServiceCollection, IConfiguration)` and reads the connection string `PostgresSql` from configuration. See `src/Infrastructure/SupportHub.Persistence/Extensions/ServiceRegistrationExtension.cs` for details.
- The development connection string is set in `src/Presentation/SupportHub.Api/appsettings.Development.json` (Host=localhost;Port=5433;Database=SupportHubDb;Username=postgres;Password=...). Update the host/port to match your Docker or local Postgres instance.
- Read-heavy ticket queries already use Dapper in `src/Infrastructure/SupportHub.Persistence/Repositories/Tickets/TicketReadRepository.cs`; EF Core remains the write side for tickets, comments, and activities.
- `src/Infrastructure/SupportHub.Persistence/UnitOfWorks/UnitOfWork.cs` is the transaction boundary used by `TransactionBehavior`; command handlers should rely on repositories plus `IUnitOfWork` instead of directly using `DbContext`.
- `Program.cs` registers a PostgreSQL health check with `AddNpgSql` against `PostgresSql`, and `/health` is the readiness endpoint for database connectivity.
---

# 9. Docker Conventions

PostgreSQL will run inside Docker.

Expected goals:

* Persistent volumes
* Environment-based configuration
* Docker Compose support
* Multi-container development

Agents should:

* Prefer containerized infrastructure
* Avoid machine-specific paths
* Use environment variables

---

# 10. Logging & Observability

Planned logging stack:

* Serilog
* Elasticsearch
* Kibana

Goals:

* Structured logging
* Correlation IDs
* Request tracing
* Exception tracking

Agents should:

* Avoid Console.WriteLine
* Prefer ILogger<T>
* Produce structured logs

Repository-specific note: `SupportHub.Api` boots Serilog in `Program.cs`, enriches logs with `CorrelationId`, and wires `CorrelationIdMiddleware`, `RequestLoggingMiddleware`, and `GlobalExceptionMiddleware` into the pipeline. Rolling file logs are written to `logs/log-.txt`.

Good:

```csharp
_logger.LogInformation("Ticket {TicketId} created", ticket.Id);
```

Bad:

```csharp
Console.WriteLine("Ticket created");
```

---

# 11. API Design Conventions

Base route convention:

```text
/api/[resource]
```

Examples:

```text
/api/tickets
/api/users
/api/auths
```

REST conventions:

* GET -> read
* POST -> create
* PUT/PATCH -> update
* DELETE -> remove

Controllers should:

* Stay thin
* Only orchestrate requests
* Delegate business logic to MediatR

Current controllers use pluralized routes like `/api/auths` and `/api/tickets`; follow the existing resource naming when adding new controllers.
Development startup also maps OpenAPI/Swagger (`AddOpenApi`, `app.MapOpenApi()`, `app.UseSwagger()`, `app.UseSwaggerUI()`) and `/health`.
API error responses are modeled with `src/Presentation/SupportHub.Api/Models/Responses/ErrorResponse.cs` and `ValidationErrorResponse.cs`; the global exception and validation paths should preserve those shapes.

---

# 12. Validation Strategy

Preferred validation library:

* FluentValidation

Validation belongs in:

* Application layer

Avoid:

* Massive validation inside controllers

# 12.5 Testing Strategy

Planned test structure:

tests/
├── SupportHub.UnitTests
├── SupportHub.IntegrationTests
└── SupportHub.ArchitectureTests

Guidelines:

- Business rules should be covered by unit tests.
- Critical API endpoints should have integration tests.
- Architecture tests should verify Clean Architecture dependency rules.
- New features should include corresponding tests whenever practical.

Future tooling:

- xUnit
- FluentAssertions
- NetArchTest
- Testcontainers

---

# 13. Authentication & Authorization

Planned:

* JWT Authentication
* ASP.NET Core Identity
* Role/Policy-based authorization

Future integrations may include:

* Refresh Tokens
* Email confirmation
* Password reset flows

Repository-specific notes:

- The project includes a `TokenService` implementation at `src/Infrastructure/SupportHub.Infrastructure/Services/TokenService.cs` that reads `Jwt:SecretKey`, `Jwt:Issuer`, `Jwt:Audience` and `Jwt:ExpirationInMinutes` from configuration. The development JWT values are present in `src/Presentation/SupportHub.Api/appsettings.Development.json`.
- Identity is configured when `AddPersistenceServices` is called (see `src/Infrastructure/SupportHub.Persistence/Extensions/ServiceRegistrationExtension.cs`) and uses `AppUser` + `IdentityRole<Guid>` with Entity Framework stores.
- Program.cs configures JWT bearer authentication and calls `SeedDefaultRolesAsync()` at startup. The role seeds are implemented in `src/Infrastructure/SupportHub.Persistence/Extensions/IdentityRoleSeedExtensions.cs` (default roles: `Admin`, `SupportAgent`, `Customer`). Agents can rely on these seed steps during local runs.
- `CurrentService` in `src/Infrastructure/SupportHub.Infrastructure/Services/CurrentService.cs` exposes `IsAuthenticated`, `Email`, `UserId`, and `FullName` from the authenticated principal, and `AuthsController` includes a protected `GET /api/auths/test` endpoint for verifying JWT claims during local development.
---

# 14. Caching Strategy

Planned:

* Redis

Use cases:

* Frequently accessed queries
* Rate limiting
* Session/token storage

Agents should:

* Cache only expensive reads
* Never cache mutable critical business operations carelessly

Repository-specific notes:

- The current implementation uses an in-memory cache service: `src/Infrastructure/SupportHub.Infrastructure/Caching/MemoryCacheService.cs`. It is registered as `ICacheService` in `src/Infrastructure/SupportHub.Infrastructure/Extensions/ServiceRegistrationExtension.cs` and `AddMemoryCache()` is invoked in `Program.cs`.
- Redis is a planned future enhancement; do not add Redis-specific code unless there is a clear need and you update DI registration and configuration docs.
- `CachingBehavior<TRequest, TResponse>` only applies to `ICacheableQuery` implementations, uses `ICurrentService.UserId` in the cache key, and `MemoryCacheService` keeps an internal key registry so prefix-based invalidation works.

# 14.5 Logging Rules

Logging should be structured and meaningful.

Command Handlers:

- Log operation start
- Log successful completion
- Log business validation failures
- Log unexpected exceptions

Query Handlers:

- Log slow queries
- Log unexpected exceptions

Avoid:

- Logging inside large loops
- Logging sensitive information
- Excessive Information logs

Preferred format:

_logger.LogInformation(
"Ticket {TicketId} assigned to {UserId}",
ticketId,
userId);


# 14.6 Ticket Ownership Rules

Authorization rules:

Customer:
- Can create tickets
- Can view only their own tickets
- Can add comments to their own tickets

SupportAgent:
- Can view all tickets
- Can assign tickets
- Can update ticket status
- Can add comments

Admin:
- Full system access

Ownership checks belong in the Application layer.

Controllers should not implement ownership logic.

# 14.7 Ticket Activity Tracking

SupportHub maintains an activity history for tickets.

Important actions should create TicketActivity records.

Examples:

- Ticket Created
- Ticket Assigned
- Ticket Status Changed
- Comment Added
- Ticket Closed

Activity creation should happen within command handlers.

Activity history should be queryable without affecting ticket write operations.

# 14.8 Authorization Standards

Authentication:
- JWT Bearer

Roles:

- Admin
- SupportAgent
- Customer

Guidelines:

- Prefer policy-based authorization.
- Avoid role checks inside controllers.
- Authorization logic should remain centralized.
- Business ownership checks belong in Application layer.

---

# 15. Messaging & Async Processing

Planned:

* RabbitMQ

Use cases:

* Email queue
* Notifications
* Event-driven communication

Future pattern:

* Domain Events
* Integration Events

---

# 16. Realtime Features

Planned:

* SignalR

Potential features:

* Live ticket updates
* Notification systems
* Realtime dashboards

---

# 17. CI/CD & Deployment Goals

Future goals:

* GitHub Actions
* Docker image pipelines
* Kubernetes deployment
* Environment promotion
* Production readiness

Agents should:

* Keep configurations environment-aware
* Avoid hardcoded secrets
* Prefer appsettings + environment variables

---

# 18. Git & Branch Strategy

Recommended approach:

```text
main
develop
feature/*
bugfix/*
```

Examples:

```text
feature/ticket-create-endpoint
feature/redis-caching
bugfix/ticket-status-update
```

Daily branch naming may evolve over time.

Agents should:

* Avoid unrelated modifications
* Keep commits focused
* Preserve clean history

---

# 19. .gitignore Expectations

The repository should ignore:

```text
bin/
obj/
/packages/
/_ReSharper.Caches/
.idea/
*.user
*.suo
appsettings.Development.json
secrets.json
```

---

# 20. Discovery Checklist For Agents

Before editing:

## Inspect solution structure

```powershell
dotnet sln list
```

## Find projects

```powershell
Get-ChildItem -Recurse -Filter *.csproj
```

Target framework versions should be determined from the project files.

Agents should inspect *.csproj files rather than assuming a specific .NET version.

## Build solution

```powershell
dotnet restore
dotnet build
```

## Run tests

```powershell
dotnet test
```

There are currently no test projects in the solution; `dotnet test` becomes meaningful once test projects are added.

Quick run / migration commands for this repository (PowerShell):

```powershell
dotnet restore; dotnet build
dotnet run --project src/Presentation/SupportHub.Api/SupportHub.Api.csproj

# Add migration (if you must create a new one):
dotnet ef migrations add <Name> --project src/Infrastructure/SupportHub.Persistence/SupportHub.Persistence.csproj --startup-project src/Presentation/SupportHub.Api/SupportHub.Api.csproj

# Apply migrations to the configured database (startup project determines configuration):
dotnet ef database update --project src/Infrastructure/SupportHub.Persistence/SupportHub.Persistence.csproj --startup-project src/Presentation/SupportHub.Api/SupportHub.Api.csproj
```

---

# 21. Safety Rules

Agents MUST NOT:

* Introduce unnecessary abstractions
* Add infrastructure before it is needed
* Place business logic in controllers
* Make Domain depend on EF Core
* Modify solution structure without reason
* Add packages without explaining why

Agents SHOULD:

* Explain architectural decisions
* Keep code readable
* Prefer explicit naming
* Think about scalability
* Preserve clean separation of concerns

---

# 22. Long-Term Vision

This repository is not only a demo project.

It is intended to evolve into a production-style backend system used for:

* Architecture learning
* Interview preparation
* Advanced backend experimentation
* DevOps practice
* Distributed systems concepts
* Performance optimization exercises

All additions should support those learning goals.

# 22.5 Current Repository Status

Implemented:

✓ Clean Architecture structure
✓ CQRS + MediatR
✓ EF Core
✓ PostgreSQL
✓ Identity
✓ JWT Authentication
✓ Role Seeding
✓ Dapper Read Side
✓ Ticket CRUD
✓ Ticket Assignment
✓ Ticket Comments
✓ Ticket Activities
✓ Ticket Ownership
✓ Caching Pipeline
✓ Validation Pipeline
✓ Transaction Pipeline
✓ Structured Logging

Planned:

□ Redis
□ RabbitMQ
□ SignalR
□ Email Notifications
□ Dashboard Queries
□ Pagination
□ Filtering
□ Sorting
□ Docker Compose
□ CI/CD
□ Kubernetes

# 22.6 Roadmap

Near-Term

- Ticket Activity Feed Endpoint
- Pagination
- Filtering
- Sorting
- Dashboard Queries
- Unit Tests

Mid-Term

- Redis Cache
- RabbitMQ
- Email Notifications
- SignalR Notifications
- Integration Tests

Long-Term

- Docker Compose
- GitHub Actions
- Kubernetes
- Distributed System Experiments
- Performance Optimization

---
