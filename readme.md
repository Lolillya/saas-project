# InventorySaaS

A multi-tenant SaaS platform for inventory management with out-going and in-going stocks management, invoice and restocks management, and employee management built with .NET 9 microservices, Clean Architecture, and CQRS.

## Architecture Overview

```
Client
  └── API Gateway (YARP)
        ├── Identity Service      → Auth, tenancy, JWT
        ├── Inventory Service     → Stock & products
        ├── Invoicing Service     → Invoices & billing
        ├── Customers Service     → Customer management
        ├── Suppliers Service     → Supplier management
        ├── Analytics Service     → Reporting & insights
        └── Notifications Service → Alerts & messaging
```

Each service follows **Clean Architecture** with four layers:

```
ServiceName/
  ServiceName.Api/            → HTTP endpoints, DI setup
  ServiceName.Application/    → CQRS commands/queries (MediatR)
  ServiceName.Domain/         → Entities, value objects
  ServiceName.Infrastructure/ → EF Core, repositories, external services
```

### Shared Libraries

| Library               | Purpose                                          |
| --------------------- | ------------------------------------------------ |
| `Shared.Kernel`       | `JwtSettings` and other cross-cutting primitives |
| `Shared.Messaging`    | Inter-service messaging (in progress)            |
| `Shared.MultiTenancy` | Tenant resolution utilities (in progress)        |

---

## Tech Stack

| Concern        | Technology                           |
| -------------- | ------------------------------------ |
| Runtime        | .NET 9                               |
| API Gateway    | YARP Reverse Proxy                   |
| Authentication | JWT Bearer + BCrypt                  |
| ORM            | Entity Framework Core 9 (SQL Server) |
| CQRS           | MediatR 14                           |
| Validation     | FluentValidation 12                  |
| Containers     | Docker + Docker Compose              |
| Orchestration  | Kubernetes (manifests in progress)   |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- SQL Server instance (local or Azure)

### Environment Setup

Copy the example environment file and fill in your values:

```bash
cp .env.example .env
```

Required variables:

```env
ConnectionStrings__IdentityDb="Server=...;Database=IdentityDb;..."
JwtSettings__Key="Your_Super_Secret_High_Security_Key_123456!"
```

> **Never commit `.env` to source control.** It is already in `.gitignore`.

### Run with Docker Compose

```bash
cd docker
docker compose up --build
```

| Service      | URL                   |
| ------------ | --------------------- |
| API Gateway  | http://localhost:5013 |
| Identity API | http://localhost:5001 |

### Run Locally (without Docker)

```bash
# Restore dependencies
dotnet restore

# Run the Identity service
dotnet run --project src/Services/Identity/Identity.Api

# Run the Gateway (separate terminal)
dotnet run --project src/ApiGateway/InventorySaas.Gateway
```

---

## API Reference

All requests go through the **API Gateway** at `/identity/*`.

### Register

```http
POST /identity/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "StrongPass123!",
  "companyName": "Acme Corp",
  "firstName": "Jane",
  "lastName": "Doe",
  "tenantSlug": "acme-corp"
}
```

### Login

```http
POST /identity/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "StrongPass123!"
}
```

**Response:**

```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<token>",
  "expiresAt": "2026-05-04T01:00:00Z"
}
```

Use the `accessToken` as a Bearer token on all subsequent authenticated requests.

---

## Multi-Tenancy Model

Each registered user creates a **Tenant**. The JWT issued on login includes:

- `tenant_id`
- `tenant_slug`
- `role`

The Gateway validates the JWT and forwards these claims downstream. Services use them to scope data per tenant.

```
Plan ──< Tenants ──< UserTenantRoles >── Roles
                └──< AspNetUsers
```

---

## Gateway Routing

Routes are configured in `appsettings.json` under `ReverseProxy`:

| Path                | Auth         | Rate Limit |
| ------------------- | ------------ | ---------- |
| `/identity/auth/**` | Anonymous    | 10 req/min |
| `/identity/**`      | JWT required | —          |

---

## Project Status

| Service              | Status         |
| -------------------- | -------------- |
| API Gateway          | ✅ Complete    |
| Identity             | ✅ Complete    |
| Inventory            | 🚧 Scaffolded  |
| Invoicing            | 🚧 Scaffolded  |
| Customers            | 🚧 Scaffolded  |
| Suppliers            | 🚧 Scaffolded  |
| Analytics            | 🚧 Scaffolded  |
| Notifications        | 🚧 Scaffolded  |
| Kubernetes manifests | 🚧 In progress |
| Shared.Messaging     | 🚧 In progress |
| Shared.MultiTenancy  | 🚧 In progress |

---

## Running Tests

```bash
dotnet test
```

Test projects are located under `tests/`:

- `Identity.Tests` — Identity service unit tests
- `Inventory.Tests` — Inventory service unit tests
- `Invoicing.Tests` — Invoicing service unit tests
- `Integration.Tests` — Cross-service integration tests

---

## License

MIT
