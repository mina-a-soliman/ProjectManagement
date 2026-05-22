# Project & Task Management API

![.NET 9](https://img.shields.io/badge/.NET-9.0-purple?style=for-the-badge&logo=.net)
![ASP.NET Core Web API](https://img.shields.io/badge/ASP.NET_Core-Web_API-blue?style=for-the-badge&logo=dotnet)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-SQL_Server-orange?style=for-the-badge&logo=microsoftsqlserver)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-success?style=for-the-badge)
![CQRS](https://img.shields.io/badge/Pattern-CQRS-blueviolet?style=for-the-badge)

A production-grade, enterprise-ready RESTful API built for managing projects and tasks. This solution was engineered with a strict adherence to **Clean Architecture**, **SOLID principles**, and modern .NET development best practices to ensure high maintainability, testability, and scalability.

---

## 🏛️ Architecture Overview

The system strictly follows the **Clean Architecture** paradigm, ensuring that the core business logic (Domain) is completely decoupled from UI, databases, and external frameworks. The dependency rule points *inward*—the inner layers know absolutely nothing about the outer layers.

### 📂 Directory Structure Map

```text
src/
├── ProjectManagement.Domain/        # (No Dependencies) Entities, Enums, Exceptions, Base Types
├── ProjectManagement.Application/   # (Depends on Domain) CQRS Handlers, DTOs, FluentValidation, Interfaces
├── ProjectManagement.Infrastructure/# (Depends on Application) EF Core DbContext, Identity, JWT, Services
└── ProjectManagement.API/           # (Depends on Application & Infrastructure) Controllers, Middleware, Web Setup
```

1. **Domain Layer**: The heart of the software. Contains enterprise-wide logic, entities (`Project`, `ProjectTask`, `ProjectUser`), and domain exceptions.
2. **Application Layer**: Contains business use cases implemented via the CQRS pattern using MediatR. Defines interfaces (e.g., `IApplicationDbContext`) implemented by outer layers.
3. **Infrastructure Layer**: Implements external concerns like database access (EF Core SQL Server), ASP.NET Core Identity, and JWT Token generation.
4. **API (Presentation) Layer**: The entry point of the application. Contains minimal logic, focusing on routing, API versioning, and wiring up dependencies.

---

## 🧩 Design Patterns Used

- **CQRS (Command Query Responsibility Segregation) via MediatR**: Read operations (Queries) and write operations (Commands) are completely separated. This allows for optimized data access strategies and prevents complex, bloated service classes.
- **Repository / Unit of Work Pattern**: Database access is abstracted through `IApplicationDbContext`. The Application layer queries `DbSet<T>` without knowing it's coupled to SQL Server, making the system highly testable.
- **Pipeline Behavior Pattern**: Built-in MediatR behaviors automatically intercept requests to run FluentValidation rules *before* the request ever reaches the handler.
- **SOLID Principles**: 
  - *Single Responsibility*: Handlers do one thing. 
  - *Dependency Inversion*: Application layer depends on interfaces, not implementations.
  - *Open/Closed*: Adding new features means adding new MediatR handlers, not modifying existing massive service classes.

---

## ✨ Features Breakdown

### 🔹 Core Features
- **Authentication**: Secure JWT-based user registration and login via ASP.NET Core Identity.
- **Projects Management**: Full CRUD operations for Projects.
- **Task Management**: Full CRUD operations for Tasks nested under specific projects, with priority and status tracking.
- **Relational Data Integrity**: Strict foreign key constraints and domain-driven encapsulation (e.g., read-only collections).

### 🏆 Advanced & Bonus Features Implemented
To elevate the quality of this submission, the following bonus architectural features were engineered:

- **Advanced API Request/Response Logging Middleware**: A custom middleware that buffers, intercepts, truncates, and securely masks sensitive data (passwords, tokens) before persisting complete API lifecycle logs to the database (`ApiLogs` table) for precise auditing and debugging.
- **Role-Based Authorization & Admin Controls**: Implemented a comprehensive administrative suite allowing users with the `Admin` role to assign users to projects, manage task assignments, and control user roles dynamically.
- **Many-to-Many Architecture**: Designed a complex many-to-many relationship between Users and Projects (via the `ProjectUser` join entity) allowing collaborative, team-based access control rather than simple 1-to-1 ownership.
- **API Versioning**: Fully integrated URL-segment API versioning (e.g., `api/v1/projects`) ensuring future-proof contract maintainability.
- **Generic Response Wrapper**: Every single endpoint returns a unified, predictable `Result<T>` or `Result` payload.
- **Global Exception Handling**: A centralized exception middleware that catches unhandled exceptions, validation errors, and unauthorized access attempts, cleanly transforming them into standard HTTP responses without exposing stack traces.

---

## 🛠️ Tech Stack & Dependencies

- **Framework:** .NET 9.0
- **Web API:** ASP.NET Core Web API
- **ORM:** Entity Framework Core 9
- **Database:** Microsoft SQL Server
- **Security:** ASP.NET Core Identity & JWT Bearer Authentication
- **Architecture Tools:** MediatR, FluentValidation, AutoMapper, Asp.Versioning.Mvc

---

## 🚀 Getting Started & Setup Instructions

Follow these instructions to run the application natively on your local environment.

### 1. Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) installed.
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express, Developer, or local instance) running.

### 2. Configure the Database Connection
Navigate to the API project's configuration file:
`src/ProjectManagement.API/appsettings.json`

Update the `DefaultConnection` string to point to your local SQL Server instance. For example:
```json
"Database": {
  "ConnectionString": "Server=localhost;Database=ProjectManagementDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;MultipleActiveResultSets=true"
}
```

### 3. Run Entity Framework Migrations
The application utilizes EF Core Code-First migrations. Open your CLI at the root of the solution and apply the migrations to create the database schema:

```bash
# Navigate to the API project
cd src/ProjectManagement.API

# Run database update (this requires dotnet ef global tool)
dotnet ef database update --project ../ProjectManagement.Infrastructure/ProjectManagement.Infrastructure.csproj
```
*(Note: A custom database seeder runs automatically on API startup to ensure required roles and default configurations exist).*

### 4. Run the Application
Start the API locally using the .NET CLI:

```bash
dotnet run
```
The API will start and typically bind to `http://localhost:5000` or `https://localhost:5001`. 

---


## 📖 API Documentation & Testing

### Swagger UI
When running in the Development environment, the API automatically serves a comprehensive Swagger UI interface.
- Navigate to: `http://localhost:<port>/swagger` (or click the root URL and it will auto-redirect).
- You can authorize directly in Swagger by clicking the **Authorize** button and pasting your `Bearer <JWT_TOKEN>`.

### Postman Collection
For deeper endpoint testing, a pre-configured Postman collection is highly recommended. You can easily import the Swagger `v1/swagger.json` definition directly into Postman to automatically generate all endpoint requests.

---

