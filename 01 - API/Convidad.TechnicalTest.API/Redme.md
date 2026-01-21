﻿# 🎅 Santa Delivery API Technical Test

A modern .NET 8 API solution for Santa's delivery management system, featuring reindeer assignment to delivery routes with capacity management.

## 📋 Features Implemented

## ✅ Part 1: Fix Existing Endpoints

* ✅ Retrieve all children

* ✅ Filter naughty children (IsNice = false)

* ✅ Get wishlist by child ID

* ✅ Get wishlist ordered by priority (descending)

* ✅ Retrieve all deliveries

* ✅ Filter failed deliveries

## ✅ Part 2: Reindeer Management System

✅ Create Reindeer entity with required properties:

* Id (GUID)

* Name (string)

* PlateNumber (string)

* Weight (double)

* Packets (int)

* Delivery Assignment: POST /delivery/deliveries/{id}/assign-reindeer

## ✅ Part 3: Structural Improvements & Readability

* ✅ Clean architecture with proper separation of concerns
* ✅ Async/await pattern throughout
* ✅ DTO layer for API contracts
* ✅ Comprehensive error handling
* ✅ Performance monitoring middleware
* ✅ Security headers

## 🏗️ Architecture Overview

      Convidad.TechnicalTest.API/
      ├── Controllers/          # RESTful API endpoints
      ├── Services/            # Business logic layer
      ├── Data/
      │   ├── Entities/        # Database entities
      │   ├── Context/         # Entity Framework DbContext
      │   └── Enums/           # Enumerations
      ├── Models/
      │   └── DTOs/           # Data Transfer Objects
      ├── Middlewares/         # Custom middleware
      ├── Tests/              # Unit and integration tests
      └── Extensions/         # Service registration extensions


## 🔧 Key Technical Decisions
   
* Single Responsibility: Each service handles one domain
* Separation of Concerns: Clear layers (Controller → Service → Entity)
* Async-First: All operations are asynchronous
* Immutable DTOs: Using C# records for data transfer objects
* In-Memory Database: SQLite in-memory for testing

## Implementation Details

### 1. Many-to-Many Relationship
* Created RouteReindeer junction table
* Composite primary key: (RouteId, ReindeerId)
* Supports multiple reindeers per route and multiple routes per reindeer

### 2. Capacity Management
 MaxDeliveries: Maximum deliveries a reindeer can handle on a specific route
* urrentDeliveries: Current delivery count for load tracking
* CanHandleNewDelivery(): Method to check route capacity before assignment
* 
### 3. Route-Level Assignment
* ❌ Before: Delivery.ReindeerId (delivery-level assignment)
* ✅ After: RouteReindeer table (route-level assignment)
* 
### 4. API Endpoints
* POST /api/routes/{routeId}/assign-reindeer - Assign reindeer to route
* DELETE /api/routes/{routeId}/reindeers/{reindeerId} - Remove reindeer from route
* GET /api/routes/{routeId}/reindeers - Get all reindeers for a route
* GET /api/routes/{routeId}/can-handle-delivery - Check if route can handle new delivery

## 🌐 API Endpoints

### Children
* GET /api/children - Get all children
* GET /api/children/naughty - Get naughty children only

### Wishlist
* GET /api/wishlist/children/{childId} - Get wishlist by child ID
* GET /api/wishlist/children/{childId}/priority - Get wishlist ordered by priority

### Deliveries
* GET /api/deliveries - Get all deliveries
* GET /api/deliveries/failures - Get failed deliveries only

### Reindeers
* GET /api/reindeers - Get all reindeers
* GET /api/reindeers/{id} - Get specific reindeer by ID
* POST /api/reindeers - Create new reindeer

### Route Reindeer Management
* POST /api/routes/{routeId}/assign-reindeer - Assign reindeer to route
* DELETE /api/routes/{routeId}/reindeers/{reindeerId} - Remove reindeer from route
* GET /api/routes/{routeId}/reindeers - Get all reindeers assigned to route
* GET /api/routes/{routeId}/can-handle-delivery - Check route capacity


## 🚀 Getting Started
### Prerequisites

* .NET 8.0 SDK
* Visual Studio 2022 or VS Code

### Installation

      # Clone the repository
      git clone <your-repository-url>
      cd Convidad.TechnicalTest.API
      
      # Restore dependencies
      dotnet restore
      
      # Build the project
      dotnet build

### Running the Application

      # Start the API server
      dotnet run

      # The application will be available at:
      # http://localhost:5*** (HTTP)
      # https://localhost:7*** (HTTPS)

### API Documentation
* Swagger UI: http://localhost:5***/swagger
* Interactive API documentation with test capabilities

## Testing
### Unit Tests
Comprehensive unit tests covering all controllers and services:

    # Run all tests
    dotnet test
    
    # Run specific test class
      dotnet test --filter "DisplayName~RouteReindeerControllerTest"

### Test Coverage
* ✅ Controllers: All HTTP endpoints tested
* ✅ Services: All business logic covered
* ✅ Error Handling: Proper exception scenarios tested
* ✅ Async Operations: All async methods properly tested

### Key Test Scenarios
* Route-reindeer assignment and capacity management
* Naughty children filtering
* Wishlist priority ordering
* Error handling for invalid requests
* Load capacity validation

## ⚙️ Technologies

### Core Stack
* .NET 8 - Latest LTS version
* ASP.NET Core - Web framework
* Entity Framework Core - ORM
* SQLite - In-memory database for testing

### Development Tools
* Moq - Mocking framework for unit tests
* xUnit - Testing framework
* Swashbuckle.AspNetCore - OpenAPI/Swagger documentation

### Architecture Patterns
* Clean Architecture - Separation of concerns
* Repository Pattern - Data access abstraction
* DTO Pattern - API contract isolation
* Middleware Pipeline - Cross-cutting concerns


## 📊 Performance & Security

### Performance Monitoring
* Request Timing Middleware: Logs slow requests (>500ms)
* Async Operations: Non-blocking I/O throughout
* Efficient Queries: Proper EF Core includes and projections

### Security Features
* Security Headers: XSS protection, MIME type safety, frame protection
* Input Validation: Model validation at API layer
* Error Handling: Generic error messages (no sensitive info exposed)
* HTTPS: Redirect enabled in production

## 🎁 Bonus Features

### Production Ready
* Environment-specific configuration
* Structured logging
* Health monitoring ready
* Scalable architecture

### Developer Experience
* Hot reload support
* Interactive API documentation
* Comprehensive test suite
* Clean, readable codebase