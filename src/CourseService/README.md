# Course Service API

## Overview
The Course Service manages course information in the Course Platform microservices architecture. It follows Clean Architecture principles with Domain, Application, Infrastructure, and API layers.

## Architecture
- **Domain Layer**: Contains the Course entity and business rules
- **Application Layer**: Contains queries, commands, and DTOs
- **Infrastructure Layer**: Contains database context and EF Core configurations
- **API Layer**: Contains HTTP endpoints

## API Endpoints

### Get Course by ID
Retrieves a specific course by its ID.

```http
GET /api/courses/{id}
```

**Response:**
```json
{
  "id": "01JCZR123ABC456DEF789GHI",
  "title": "Introduction to Microservices",
  "description": "Learn about microservices architecture",
  "isPublished": true,
  "isFeatured": false,
  "imageUrl": "https://example.com/image.jpg",
  "instructorUserId": "instructor-123",
  "price": {
    "amount": 99.99,
    "currency": "ILS"
  },
  "updatedAtUtc": "2025-11-07T12:00:00Z"
}
```

### Get Featured Courses
Retrieves all courses marked as featured and published.

```http
GET /api/courses/featured
```

**Response:**
```json
[
  {
    "id": "01JCZR123ABC456DEF789GHI",
    "title": "Introduction to Microservices",
    "description": "Learn about microservices architecture",
    "isPublished": true,
    "isFeatured": true,
    "imageUrl": "https://example.com/image.jpg",
    "instructorUserId": "instructor-123",
    "price": {
      "amount": 99.99,
      "currency": "ILS"
    },
    "updatedAtUtc": "2025-11-07T12:00:00Z"
  }
]
```

**Features:**
- Returns only published courses that are marked as featured
- Returns an empty array if no featured courses are available
- No authentication required (public endpoint)

## Domain Model

### Course Entity
- `Id` (CourseId): Unique identifier
- `Title` (string): Course title
- `Description` (string): Course description
- `IsPublished` (bool): Whether the course is published
- `IsFeatured` (bool): Whether the course is featured
- `ImageUrl` (string?): Optional course image URL
- `InstructorUserId` (string?): Optional instructor user ID
- `Price` (Money): Course price with amount and currency
- `UpdatedAtUtc` (DateTimeOffset): Last update timestamp

## Error Handling
The API uses the Result pattern for consistent error handling:
- `404 Not Found`: Course not found
- `400 Bad Request`: Invalid request
- `500 Internal Server Error`: Server error

## Testing
Use the `Test.http` file in the project root for testing endpoints with VS Code REST Client or similar tools.
