# 📚 Course Platform – Microservices Architecture

## 👋 Overview
This project is a **Course Platform** built with a **microservices architecture** using **.NET 9**.  
It was designed as a learning playground to practice **scalable system design**, **distributed communication**, and multiple architectural patterns.

The platform manages **Users**, **Authentication**, **Courses**, **Enrollments**, and **Orders** as independent services. Each service owns its own database and communicates via messaging (RabbitMQ / Kafka).

---

## 🏗️ Architecture
During the project I experimented with multiple architecture styles:

- **🧹 Clean Architecture** – layering by Domain / Application / Infrastructure / WebAPI.  
- **🏛️ Domain-Driven Design (DDD)** – aggregates, value objects, domain events, repositories.  
- **📡 Event-Driven Architecture (EDA)** – asynchronous communication with RabbitMQ / Kafka, integration events, Outbox/Inbox patterns.  
- **📂 N-Layer (traditional)** – Controllers → Services → Repositories (classic separation).  

> This gave me hands-on experience comparing **traditional layered approaches** with **modern domain-driven and event-driven designs**.

---

## ⚙️ Tech Stack

### Core Technologies
- **.NET 9** (C#)  
- **ASP.NET Core WebAPI**  
- **PostgreSQL** (per service, isolated database)  
- **Entity Framework Core**  
- **MassTransit** with **RabbitMQ** (for async messaging)  
- **Docker & Docker Compose**  
- **Kubernetes** (deployment experiments)  
- **AWS** (S3, EC2, IAM – cloud integration exploration)  

### Frontend
- **React** (TypeScript) – basic UI for interaction  

### Testing & Quality
- **xUnit** + **FluentAssertions** (unit testing)  
- **Moq** (application layer testing with mocks)  
- **Testcontainers** (integration tests with PostgreSQL + RabbitMQ)  
- **WebApplicationFactory** (end-to-end API testing)  

---

## 🔥 Key Features
- Independent services for **Users**, **Auth**, **Courses**, **Enrollments**, **Orders**.  
- **Event-driven communication** (e.g., `UserUpserted`, `EnrollmentCreated`).  
- **Outbox pattern** for reliable event publishing.  
- **Result<T> pattern** for consistent error handling.  
- **Strongly-typed IDs** (using ULID) for entities.  
- **Optimistic concurrency control** with versioning.  

---

## 🧪 What I Learned
- Comparing **N-Layer vs Clean vs DDD vs EDA** in real codebases.  
- How to apply **CQRS** with separate read/write contexts.  
- Using **MassTransit Outbox** for idempotent message handling.  
- Writing **integration tests** with ephemeral containers.  
- Setting up **CI/CD pipelines** (GitHub Actions, Jenkins experiments).  
- Structuring a **monorepo with per-service branches**.  

---
