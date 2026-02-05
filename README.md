# 📚 Course Platform – Microservices Architecture

> **A learning management system (LMS) demonstrating production-ready microservices patterns with .NET 9, React, RabbitMQ, and PostgreSQL.**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19-61DAFB?logo=react)](https://react.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.13-FF6600?logo=rabbitmq&logoColor=white)](https://www.rabbitmq.com/)
[![License](https://img.shields.io/badge/License-Educational-green)](LICENSE)

A full-featured **Course Platform** built with a **microservices architecture** using **.NET 9** and **React**. This project demonstrates scalable system design, distributed communication patterns, and modern software engineering practices including DDD, Clean Architecture, CQRS, and event-driven messaging.

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
| **Enrollments** | Enroll users in courses with validation (part of CourseService) |
| **Order Processing** | Handle course purchases and payment flows |
| **File Storage** | S3-compatible object storage for course materials and videos |
| **Video Transcoding** | Automatic video transcoding for streaming |
| **API Gateway** | Centralized entry point with Yarp reverse proxy |
| **Identity Management** | Keycloak integration for OAuth2/OIDC authentication |
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
│                    (React 19 + TypeScript + Vite 7)                     │
└─────────────────────────────────┬───────────────────────────────────────┘
                                  │ HTTP/REST
┌─────────────────────────────────▼───────────────────────────────────────┐
│                          API Gateway (Yarp)                             │
│                    + Redis (Session/Caching)                            │
└───────┬─────────┬─────────┬──────────────┬─────────────┬───────────────┘
        │         │         │              │             │
   ┌────▼───┐ ┌───▼────┐ ┌──▼─────┐  ┌─────▼─────┐  ┌─────▼─────┐
   │  Auth  │ │  User  │ │ Course │  │  Storage  │  │   Order   │
   │Service │ │Service │ │Service │  │  Service  │  │  Service  │
   └────┬───┘ └───┬────┘ └───┬────┘  └─────┬─────┘  └─────┬─────┘
        │         │          │             │             │
   ┌────▼───┐ ┌───▼────┐ ┌───▼────┐  ┌─────▼──────┐ ┌─────▼─────┐
   │ AuthDB │ │UserDB  │ │CourseDB│  │   Garage   │ │ OrderDB   │
   │(Postgres)│(Postgres)│(Postgres)│ │ (S3 Store) │ │(Postgres) │
   └────────┘ └────────┘ └────────┘  └────────────┘ └───────────┘
                                  │
                    ┌─────────────▼─────────────┐
                    │        RabbitMQ           │
                    │   (Message Broker)        │
                    └───────────────────────────┘
                                  │
                    ┌─────────────▼─────────────┐
                    │       Keycloak            │
                    │   (Identity Provider)     │
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
| **API Gateway Pattern** | Yarp reverse proxy for routing and authentication |
| **Minimal APIs** | Lightweight endpoints for Gateway and StorageService |

> **Note**: Course enrollments are managed within the CourseService domain, not as a separate microservice. This follows the DDD principle of keeping related aggregates within the same bounded context.

### Inter-Service Communication

- **Synchronous**: REST APIs via API Gateway for client-service communication
- **Asynchronous**: RabbitMQ message broker for event-driven communication between services
- **Integration Events**: Events defined in `Contracts/` assemblies (e.g., `UserUpserted`, `EnrollmentCreated`, `OrderCompleted`)
- **Message Bus**: MassTransit provides abstraction over RabbitMQ with automatic retry and error handling
- **Outbox Pattern**: Ensures reliable event publishing using transactional outbox table

---

## 🔧 Microservices

| Service | Description | Architecture | Port (Default) |
|---------|-------------|--------------|----------------|
| **AuthService** | JWT authentication, token refresh, password management, Keycloak integration | Clean Architecture + DDD | Dynamic (Aspire) |
| **UserService** | User registration, profile management, user queries | Clean Architecture + DDD + CQRS | Dynamic (Aspire) |
| **CourseService** | Course CRUD, catalog management, enrollment management | Clean Architecture + DDD | Dynamic (Aspire) |
| **StorageService** | File storage, S3-compatible object storage, video transcoding | Minimal API | Dynamic (Aspire) |
| **OrderService** | Order processing, payment workflows | Clean Architecture + DDD | Dynamic (Aspire) |
| **Gateway** | API Gateway with reverse proxy (Yarp), authentication, routing | Minimal API + Yarp | Dynamic (Aspire) |

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
| **Keycloak 26.4.7** | Identity and Access Management |
| **Redis** | Caching and session management |
| **Garage v2.1.0** | S3-compatible object storage |
| **Yarp** | Reverse proxy for API Gateway |

### Frontend

| Technology | Purpose |
|------------|---------|
| **React 19** | UI framework |
| **TypeScript** | Type-safe JavaScript |
| **Vite 7** | Build tool and dev server |
| **React Router 7.9** | Client-side routing |
| **TanStack Query** | Data fetching and caching |
| **Radix UI** | Accessible component primitives |
| **Tailwind CSS 4** | Utility-first CSS framework |
| **i18next** | Internationalization |
| **OIDC Client** | OpenID Connect authentication |
| **Vidstack** | Video player components |

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
│   ├── CourseService/         # Course catalog & enrollment microservice
│   ├── StorageService/        # File storage & video transcoding
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
├── Gateway.Api/               # API Gateway with Yarp
├── Frontend/                  # React TypeScript frontend
├── Tests/                     # Additional test projects
├── infrastructure/            # Keycloak themes and configuration
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

### Platform Notes

⚠️ **Windows**: The Aspire configuration uses Windows-specific volume paths (`C:\AspireVolumes\*`).

🐧 **Linux/Mac**: You'll need to modify volume paths in `CoursePlatform.AppHost/AppHost.cs` to use appropriate paths for your OS (e.g., `/var/lib/aspire/*` or `~/aspire-volumes/*`).

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/dekel5030/CoursePlatformMicroServices.git
cd CoursePlatformMicroServices
```

### 2. Install .NET Aspire Workload (Recommended)

```bash
# Install Aspire workload for orchestration
dotnet workload install aspire
```

### 3. Run the Application with .NET Aspire

.NET Aspire orchestrates all services, databases, and infrastructure automatically:

```bash
# Run from repository root
dotnet run --project CoursePlatform.AppHost
```

This will start:
- **All backend services** (Auth, User, Course, Storage, Order)
- **API Gateway** with Yarp
- **Frontend** React application
- **PostgreSQL databases** (separate database per service)
- **RabbitMQ** message broker
- **Redis** for caching and sessions
- **Keycloak** for identity management
- **Garage** S3-compatible object storage
- **Aspire Dashboard** at http://localhost:15000

### 4. Access the Application

Once started, you can access:

| Resource | URL | Description |
|----------|-----|-------------|
| **Aspire Dashboard** | http://localhost:15000 | Service orchestration and monitoring |
| **Frontend** | Dynamic (check Aspire Dashboard) | React application |
| **API Gateway** | Dynamic (check Aspire Dashboard) | Gateway endpoints |
| **Keycloak Admin** | http://localhost:8080 | Identity management (admin/admin) |
| **RabbitMQ Management** | http://localhost:15672 | Message broker UI (guest/guest) |

> **Note**: Aspire assigns dynamic ports to services. Use the Aspire Dashboard to find the exact URLs.

---

## 📖 Usage

### Accessing Services

All service URLs are dynamically assigned by .NET Aspire. Access the **Aspire Dashboard** at http://localhost:15000 to view:
- Service endpoints and health status
- Logs and traces
- Metrics and performance data
- Resource dependencies

### Key Components

| Component | Purpose |
|-----------|---------|
| **API Gateway** | Single entry point for all frontend requests, handles routing, authentication |
| **Keycloak** | Identity provider for OAuth2/OIDC authentication |
| **Redis** | Session storage and caching layer |
| **Garage** | S3-compatible storage for course materials and videos |
| **RabbitMQ** | Event-driven communication between services |

### API Documentation

Each service exposes Swagger/OpenAPI documentation at the `/swagger` endpoint when running in Development mode. Access them through the Aspire Dashboard.

### Frontend Features

The React frontend includes:
- User authentication with Keycloak (OIDC)
- Course browsing and enrollment
- Video player with HLS streaming
- Multi-language support (i18next)
- Responsive design with Tailwind CSS
- Dark/light theme support

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

#### Aspire services won't start

```bash
# Update Aspire workload
dotnet workload update

# Clean and rebuild
dotnet clean
dotnet build

# Check Aspire Dashboard logs at http://localhost:15000
```

#### Port conflicts

If you encounter port conflicts:
1. Stop any services using the required ports
2. Let Aspire assign dynamic ports automatically
3. Check the Aspire Dashboard for actual port assignments

#### Volume mount issues (Windows)

The AppHost uses Windows-specific volume paths:
```
C:\AspireVolumes\*
```

If you're on Linux/Mac, you'll need to modify `CoursePlatform.AppHost/AppHost.cs` to use appropriate paths.

#### Keycloak connection issues

1. Verify Keycloak is running via Aspire Dashboard
2. Default admin credentials: `admin` / `admin`
3. Keycloak health endpoint should be accessible at `/health/ready`

#### Frontend build errors

```bash
cd Frontend
rm -rf node_modules package-lock.json
npm install
npm run build
```

#### Database migration issues

Migrations are applied automatically on service startup. If issues occur:
```bash
# Example: Apply UserService migrations manually
cd src/UserService
dotnet ef database update --project Infrastructure --startup-project Web.Api
```

### Environment-Specific Configuration

Services support configuration via:
- `appsettings.json` – Default settings
- `appsettings.Development.json` – Development overrides
- `appsettings.Production.json` – Production settings
- Environment variables – Override any setting at runtime
- .NET Aspire – Service configuration in `AppHost.cs`

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

- **Architecture comparison** – Clean Architecture vs DDD vs Minimal APIs in real codebases
- **CQRS implementation** – Separate read/write contexts and projections
- **MassTransit Outbox** – Idempotent message handling and reliable publishing
- **Integration testing** – Using ephemeral containers with Testcontainers
- **CI/CD pipelines** – GitHub Actions for automated builds and tests
- **Monorepo structure** – Managing multiple services in a single repository
- **.NET Aspire** – Cloud-native service orchestration with dynamic resource allocation
- **API Gateway Pattern** – Implementing Yarp reverse proxy for unified entry point
- **Identity Management** – Integrating Keycloak for OAuth2/OIDC authentication
- **S3-compatible storage** – Using Garage for distributed object storage
- **Video streaming** – HLS video transcoding and delivery
- **Event-driven architecture** – Asynchronous inter-service communication with RabbitMQ

---

## 📬 Contact

**Author**: Dekel  
**GitHub**: [@dekel5030](https://github.com/dekel5030)

---

> ⭐ If you find this project helpful, please consider giving it a star!
