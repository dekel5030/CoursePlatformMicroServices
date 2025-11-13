# Custom Agents for Course Platform Microservices

This directory contains custom AI agents specialized for different aspects of the Course Platform microservices architecture. Each agent provides expert guidance and assistance for specific domains within the project.

## Available Agents

### 1. **CleanCodeAgent** 
   - **Files**: `my-agent.agent.md`, `clean-code-agent.yml`
   - **Purpose**: Maintains code clarity and architectural consistency
   - **Focus Areas**:
     - Clean Architecture principles
     - Event-Driven design patterns
     - Meaningful naming conventions
     - Readable and maintainable code

### 2. **MicroservicesTestingAgent** 
   - **File**: `microservices-testing-agent.md`
   - **Purpose**: Comprehensive testing for microservices architecture
   - **Focus Areas**:
     - Domain unit tests with xUnit and FluentAssertions
     - Integration tests with Testcontainers (PostgreSQL, RabbitMQ)
     - API tests with WebApplicationFactory
     - Event-driven messaging tests with MassTransit
     - Test isolation and AAA pattern

### 3. **ApiDesignAgent** 
   - **File**: `api-design-agent.md`
   - **Purpose**: RESTful API design and implementation
   - **Focus Areas**:
     - REST conventions and HTTP methods
     - Request/response contracts
     - HTTP status codes
     - API versioning
     - Input validation
     - OpenAPI/Swagger documentation
     - Minimal APIs with ASP.NET Core

### 4. **EventDrivenAgent** 
   - **File**: `event-driven-agent.md`
   - **Purpose**: Event-driven architecture patterns
   - **Focus Areas**:
     - MassTransit integration
     - Event publishing and consuming
     - Outbox/Inbox patterns for reliability
     - Saga patterns for workflows
     - Idempotent message handling
     - RabbitMQ/Kafka configuration
     - Message contracts and versioning

### 5. **DomainDrivenDesignAgent** 
   - **File**: `ddd-agent.md`
   - **Purpose**: Domain-Driven Design tactical patterns
   - **Focus Areas**:
     - Aggregates and aggregate roots
     - Entities with strongly-typed IDs (ULID)
     - Value objects and immutability
     - Domain events
     - Repository patterns
     - Domain services
     - Ubiquitous language
     - Result pattern for error handling

## How to Use These Agents

These custom agents are designed to provide specialized assistance when working on different parts of the system:

- **Writing Tests?** → Use **MicroservicesTestingAgent**
- **Creating APIs?** → Use **ApiDesignAgent**
- **Implementing Events?** → Use **EventDrivenAgent**
- **Modeling Domain?** → Use **DomainDrivenDesignAgent**
- **Code Review?** → Use **CleanCodeAgent**

## Agent Capabilities

Each agent can help with:
- ✅ Code reviews and suggestions
- ✅ Implementation guidance
- ✅ Best practices enforcement
- ✅ Pattern recommendations
- ✅ Example code snippets
- ✅ Architecture decisions

## Technology Stack Context

These agents are tailored for this project's technology stack:
- **.NET 9** with C#
- **ASP.NET Core** Minimal APIs
- **Entity Framework Core** with PostgreSQL
- **MassTransit** with RabbitMQ
- **xUnit**, FluentAssertions, Moq
- **Testcontainers**
- **Docker** and Kubernetes

## Contributing

When adding new custom agents:
1. Create a new `.md` file in this directory
2. Follow the existing structure (name, description, expertise areas, examples)
3. Update this README with the new agent information
4. Ensure the agent aligns with the project's architecture and patterns

## Related Documentation

- [Main README](../../README.md) - Project overview and architecture
- [Clean Architecture Docs](../../docs/clean-architecture.md) (if exists)
- [Testing Strategy](../../docs/testing-strategy.md) (if exists)
- [API Guidelines](../../docs/api-guidelines.md) (if exists)
