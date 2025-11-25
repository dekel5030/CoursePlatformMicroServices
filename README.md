# 📚 Course Platform – Microservices Architecture

A full-featured **Course Platform** built with a **microservices architecture** using **.NET 9** and **React**. This project demonstrates scalable system design, distributed communication patterns, and modern software engineering practices.

---

## Table of Contents

- [Overview](#-overview)
- [Key Features](#-key-features)
- [Architecture](#-architecture)
- [Microservices](#-microservices)
- [Tech Stack](#-tech-stack)
- [Project Structure](#-project-structure)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Usage](#-usage)
- [Testing](#-testing)
- [Troubleshooting](#-troubleshooting)
- [Contributing](#-contributing)
- [License](#-license)

---

## 👋 Overview

The **Course Platform** is a learning management system that enables users to register, browse courses, enroll in courses, and place orders. The platform is designed as a **microservices architecture**, where each bounded context is implemented as an independent service with its own database, communicating asynchronously via message queues.

### Why Microservices?

This project serves as a practical playground for experimenting with:

- **Scalable system design** – Independent services that can scale individually
- **Distributed communication** – Event-driven architecture with message queues
- **Modern architectural patterns** – DDD, Clean Architecture, CQRS, and more
- **Cloud-native development** – Docker, Kubernetes, and .NET Aspire orchestration

---

## 🔥 Key Features

| Feature | Description |
|---------|-------------|
| **User Management** | Register, authenticate, and manage user profiles |
| **Course Catalog** | Browse, create, and manage course offerings |
| **Enrollments** | Enroll users in courses with validation |
| **Order Processing** | Handle course purchases and payment flows |
| **Event-Driven Communication** | Services communicate via RabbitMQ integration events |
| **Outbox Pattern** | Reliable event publishing with transactional guarantees |
| **Result Pattern** | Consistent error handling across all services |
| **Strongly-Typed IDs** | ULID-based identifiers for type safety |
| **Optimistic Concurrency** | Version-based conflict detection |

---

## 🏗️ Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                              Frontend                                    │
│                         (React + TypeScript)                            │
└─────────────────────────────────┬───────────────────────────────────────┘
                                  │ HTTP/REST
┌─────────────────────────────────▼───────────────────────────────────────┐
│                           API Gateway / Direct                          │
└───────┬─────────┬─────────┬─────────────┬─────────────┬─────────────────┘
        │         │         │             │             │
   ┌────▼───┐ ┌───▼────┐ ┌──▼─────┐ ┌─────▼─────┐ ┌─────▼─────┐
   │  Auth  │ │  User  │ │ Course │ │Enrollment │ │   Order   │
   │Service │ │Service │ │Service │ │  Service  │ │  Service  │
   └────┬───┘ └───┬────┘ └───┬────┘ └─────┬─────┘ └─────┬─────┘
        │         │          │            │             │
   ┌────▼───┐ ┌───▼────┐ ┌───▼────┐ ┌─────▼─────┐ ┌─────▼─────┐
   │ AuthDB │ │UserDB  │ │CourseDB│ │EnrollDB   │ │ OrderDB   │
   │(Postgres)│(Postgres)│(Postgres)│(Postgres) │ │(Postgres) │
   └────────┘ └────────┘ └────────┘ └───────────┘ └───────────┘
                                  │
                    ┌─────────────▼─────────────┐
                    │        RabbitMQ           │
                    │   (Message Broker)        │
                    └───────────────────────────┘
```

### Architectural Patterns

| Pattern | Implementation |
|---------|---------------|
| **Clean Architecture** | Layered structure: Domain → Application → Infrastructure → Web.Api |
| **Domain-Driven Design (DDD)** | Aggregates, Value Objects, Domain Events, Repositories |
| **Event-Driven Architecture (EDA)** | Asynchronous messaging via RabbitMQ with MassTransit |
| **CQRS** | Separate read/write database connections where applicable |
| **Outbox Pattern** | Transactional event publishing for reliability |
| **N-Layer** | Traditional Controllers → Services → Repositories (EnrollmentService) |

### Inter-Service Communication

- **Synchronous**: REST APIs for direct client-service communication
- **Asynchronous**: RabbitMQ message broker for event-driven communication between services
- **Integration Events**: `UserUpserted`, `EnrollmentCreated`, `OrderCompleted`, etc.

---

## 🔧 Microservices

| Service | Description | Architecture | Port (Default) |
|---------|-------------|--------------|----------------|
| **AuthService** | JWT authentication, token refresh, password management | Clean Architecture + DDD | 5433 |
| **UserService** | User registration, profile management, user queries | Clean Architecture + DDD + CQRS | 5125 |
| **CourseService** | Course CRUD, catalog management, course queries | Clean Architecture + DDD | 5434 |
| **EnrollmentService** | Course enrollment, enrollment validation | N-Layer (Traditional) | 5435 |
| **OrderService** | Order processing, payment workflows | Clean Architecture + DDD | 5436 |

### Shared Libraries

| Library | Purpose |
|---------|---------|
| **Common** | Shared utilities, Result pattern, Auth helpers |
| **Common.Web** | Web-specific extensions, Swagger configuration |
| **Contracts** | Shared DTOs and integration event definitions |
| **Shared/Kernel** | Domain primitives (Error, Money, Result) |
| **Shared/Messaging** | MassTransit messaging configuration |
| **CoursePlatform.ServiceDefaults** | .NET Aspire service configuration defaults |

---

## ⚙️ Tech Stack

### Backend

| Technology | Purpose |
|------------|---------|
| **.NET 9** | Core framework (C#) |
| **ASP.NET Core** | Web API framework |
| **Entity Framework Core** | ORM for database access |
| **MassTransit** | Message bus abstraction |
| **PostgreSQL 16** | Relational database (per service) |
| **RabbitMQ 3.13** | Message broker |
| **.NET Aspire** | Cloud-native orchestration |

### Frontend

| Technology | Purpose |
|------------|---------|
| **React 19** | UI framework |
| **TypeScript** | Type-safe JavaScript |
| **Vite 7** | Build tool and dev server |
| **React Router 7** | Client-side routing |

### Infrastructure & DevOps

| Technology | Purpose |
|------------|---------|
| **Docker** | Containerization |
| **Docker Compose** | Multi-container orchestration |
| **Kubernetes** | Container orchestration (deployment experiments) |
| **GitHub Actions** | CI/CD pipelines |

### Testing

| Technology | Purpose |
|------------|---------|
| **xUnit** | Unit testing framework |
| **FluentAssertions** | Assertion library |
| **Moq** | Mocking framework |
| **Testcontainers** | Integration testing with real databases |
| **WebApplicationFactory** | End-to-end API testing |

---

## 📁 Project Structure

```
CoursePlatformMicroServices/
├── src/
│   ├── AuthService/           # Authentication microservice
│   │   ├── Application/       # Use cases, commands, queries
│   │   ├── Domain/            # Entities, value objects, events
│   │   ├── Infrastructure/    # EF Core, messaging, external services
│   │   └── Web.Api/           # API endpoints, middleware
│   ├── UserService/           # User management microservice
│   ├── CourseService/         # Course catalog microservice
│   ├── EnrollmentService/     # Enrollment microservice (N-Layer)
│   ├── OrderService/          # Order processing microservice
│   └── Shared/
│       ├── Kernel/            # Domain primitives
│       └── Messaging/         # Messaging infrastructure
├── Contracts/                 # Shared contracts and DTOs
│   ├── Auth.Contracts/
│   ├── Users.Contracts/
│   ├── Courses.Contracts/
│   ├── Enrollments.Contracts/
│   ├── Order.Contracts/
│   └── Products.Contracts/
├── Common/                    # Shared utilities
├── Common.Web/                # Web-specific utilities
├── Frontend/                  # React TypeScript frontend
├── Tests/                     # Additional test projects
├── infra/                     # Docker Compose files
├── CoursePlatform.AppHost/    # .NET Aspire orchestrator
└── CoursePlatform.ServiceDefaults/  # Aspire service defaults
```

---

## 📋 Prerequisites

Before running the project, ensure you have the following installed:

| Requirement | Version | Notes |
|-------------|---------|-------|
| **.NET SDK** | 9.0+ | [Download](https://dotnet.microsoft.com/download) |
| **Node.js** | 18+ | [Download](https://nodejs.org/) |
| **Docker** | Latest | [Download](https://www.docker.com/get-started) |
| **Docker Compose** | v2.0+ | Usually included with Docker Desktop |

### Optional

- **Visual Studio 2022** (17.8+) or **VS Code** with C# extension
- **JetBrains Rider** for .NET development
- **.NET Aspire workload** for orchestration: `dotnet workload install aspire`

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/dekel5030/CoursePlatformMicroServices.git
cd CoursePlatformMicroServices
```

### 2. Start Infrastructure (Databases + RabbitMQ)

#### Option A: Using Docker Compose

```bash
# Start all PostgreSQL databases
cd infra
docker-compose up -d

# Start RabbitMQ separately
docker-compose -f docker-compose.rabbitmq.yml up -d
```

#### Option B: Using .NET Aspire (Recommended)

```bash
# Install Aspire workload if not already installed
dotnet workload install aspire

# Run from repository root
dotnet run --project CoursePlatform.AppHost
```

### 3. Configure Environment Variables

Copy the sample environment file and adjust values if needed:

```bash
cd infra
cp .env .env.local
```

#### Database Connection Strings

Each service requires its own PostgreSQL database. The default configuration in `infra/.env`:

| Service | Database | Default Port |
|---------|----------|--------------|
| UserService | usersdb | 5432 |
| AuthService | authdb | 5433 |
| CourseService | coursesdb | 5434 |
| EnrollmentService | enrollmentdb | 5435 |
| OrderService | ordersdb | 5436 |

### 4. Run Database Migrations

Each service manages its own database schema. Migrations are typically applied on startup via Entity Framework Core.

```bash
# Example: Apply UserService migrations manually
cd src/UserService
dotnet ef database update --project Infrastructure --startup-project Web.Api
```

### 5. Run Backend Services

#### Run All Services with Aspire

```bash
dotnet run --project CoursePlatform.AppHost
```

#### Run Individual Services

```bash
# UserService
cd src/UserService/Web.Api
dotnet run

# AuthService
cd src/AuthService/Web.Api
dotnet run

# CourseService
cd src/CourseService
dotnet run

# EnrollmentService
cd src/EnrollmentService
dotnet run

# OrderService
cd src/OrderService/Web.Api
dotnet run
```

### 6. Run Frontend

```bash
cd Frontend
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173` (default Vite port).

---

## 📖 Usage

### Accessing Services

| Service | URL | Description |
|---------|-----|-------------|
| **Frontend** | http://localhost:5173 | React application |
| **UserService API** | http://localhost:5125/swagger | User management endpoints |
| **AuthService API** | http://localhost:5433/swagger | Authentication endpoints |
| **CourseService API** | http://localhost:5434/swagger | Course management endpoints |
| **RabbitMQ Management** | http://localhost:15672 | Message broker UI (guest/guest) |
| **Aspire Dashboard** | http://localhost:15000 | Service orchestration dashboard |

### API Documentation

Each service exposes Swagger/OpenAPI documentation at the `/swagger` endpoint when running in Development mode.

### Example API Calls

#### Register a New User

```bash
curl -X POST http://localhost:5125/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

#### Authenticate

```bash
curl -X POST http://localhost:5433/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "your-password"
  }'
```

---

## 🧪 Testing

### Run All Tests

```bash
dotnet test CoursePlatform.sln
```

### Run Specific Test Projects

```bash
# UserService Unit Tests
dotnet test src/UserService/Tests/Domain.UnitTests

# UserService Integration Tests
dotnet test src/UserService/Tests/UserService.IntegrationTests

# OrderService Tests
dotnet test src/OrderService/Tests/Domain.UnitTests
dotnet test src/OrderService/Tests/Application.UnitTests
dotnet test src/OrderService/Tests/Architecture.Tests
```

### Frontend Tests

```bash
cd Frontend
npm run lint    # Run ESLint
npm run build   # Type-check and build
```

### Test Types

| Type | Location | Description |
|------|----------|-------------|
| **Unit Tests** | `*/Tests/Domain.UnitTests` | Domain logic testing |
| **Application Tests** | `*/Tests/Application.UnitTests` | Use case testing with mocks |
| **Integration Tests** | `*/Tests/*IntegrationTests` | Tests with real databases (Testcontainers) |
| **Architecture Tests** | `*/Tests/Architecture.Tests` | Verify layering and dependencies |

---

## 🔧 Troubleshooting

### Common Issues

#### Docker containers won't start

```bash
# Check if ports are in use
netstat -an | grep 5432  # PostgreSQL
netstat -an | grep 5672  # RabbitMQ

# Reset containers
cd infra
docker-compose down -v
docker-compose up -d
```

#### Database connection errors

1. Ensure PostgreSQL containers are running: `docker ps`
2. Verify connection strings in `appsettings.json` match your environment
3. Check that the database has been created

#### RabbitMQ connection refused

1. Verify RabbitMQ is running: `docker ps | grep rabbitmq`
2. Check the management UI at http://localhost:15672
3. Default credentials: `guest` / `guest`

#### .NET Aspire issues

```bash
# Update Aspire workload
dotnet workload update

# Clean and rebuild
dotnet clean
dotnet build
```

#### Frontend build errors

```bash
cd Frontend
rm -rf node_modules package-lock.json
npm install
npm run build
```

### Environment-Specific Configuration

Services support configuration via:
- `appsettings.json` – Default settings
- `appsettings.Development.json` – Development overrides
- `appsettings.Production.json` – Production settings
- Environment variables – Override any setting at runtime

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m 'Add your feature'`
4. Push to the branch: `git push origin feature/your-feature`
5. Open a Pull Request

---

## 📄 License

This project is for educational purposes. Feel free to use it as a reference for learning microservices architecture patterns.

---

## 🧪 What I Learned

Building this project provided hands-on experience with:

- **Architecture comparison** – N-Layer vs Clean vs DDD vs EDA in real codebases
- **CQRS implementation** – Separate read/write contexts and projections
- **MassTransit Outbox** – Idempotent message handling and reliable publishing
- **Integration testing** – Using ephemeral containers with Testcontainers
- **CI/CD pipelines** – GitHub Actions for automated builds and tests
- **Monorepo structure** – Managing multiple services in a single repository
- **.NET Aspire** – Cloud-native service orchestration

---

## 📬 Contact

**Author**: Dekel  
**GitHub**: [@dekel5030](https://github.com/dekel5030)

---

> ⭐ If you find this project helpful, please consider giving it a star!
