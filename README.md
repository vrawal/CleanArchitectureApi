# Clean Architecture API - .NET 9

A production-ready .NET 9 Web API built with Clean Architecture principles, featuring CQRS, comprehensive validation, JWT authentication, caching, message queuing, and more.

## 🏗️ Architecture

This project follows Clean Architecture principles with the following layers:

- **Domain Layer**: Core business logic, entities, value objects, and domain events
- **Application Layer**: Use cases, CQRS commands/queries, DTOs, and application services
- **Infrastructure Layer**: Data access, external services, and infrastructure concerns
- **WebApi Layer**: Controllers, middleware, and API configuration

## 🚀 Features

### Core Features
- ✅ **Clean Architecture** with proper dependency inversion
- ✅ **CQRS** pattern with MediatR
- ✅ **Domain-Driven Design** with entities, value objects, and domain events
- ✅ **Specification Pattern** for flexible queries
- ✅ **Unit of Work** pattern for transaction management
- ✅ **Generic Repository** pattern

### Security & Authentication
- ✅ **JWT Authentication** and Authorization
- ✅ **Role-based Authorization**
- ✅ **Rate Limiting** middleware
- ✅ **CORS** configuration

### Validation & Error Handling
- ✅ **FluentValidation** for request validation
- ✅ **Global Exception Handling** middleware
- ✅ **Structured Error Responses**

### Data & Persistence
- ✅ **Entity Framework Core** with SQL Server
- ✅ **Database Migrations**
- ✅ **Soft Delete** implementation
- ✅ **Audit Fields** (CreatedAt, UpdatedAt, etc.)

### Caching & Performance
- ✅ **Redis** caching with fallback to in-memory cache
- ✅ **Response Caching**
- ✅ **Query Optimization** with specifications

### Messaging & Background Jobs
- ✅ **RabbitMQ** for message queuing
- ✅ **Hangfire** for background job processing
- ✅ **Domain Events** handling

### External Services
- ✅ **Email Service** with MailKit
- ✅ **HTTP Client** with Polly for resilience
- ✅ **Retry Policies** and Circuit Breaker

### Monitoring & Logging
- ✅ **Serilog** for structured logging
- ✅ **Health Checks**
- ✅ **Application Metrics**

### API Documentation
- ✅ **Swagger/OpenAPI** documentation
- ✅ **API Versioning**
- ✅ **XML Documentation**

### Testing
- ✅ **Unit Tests** with xUnit
- ✅ **Integration Tests**
- ✅ **Test Fixtures** and Mocking

### DevOps & Deployment
- ✅ **Docker** containerization
- ✅ **Docker Compose** for local development
- ✅ **Nginx** reverse proxy configuration
- ✅ **Environment-specific** configurations

## 🛠️ Technology Stack

- **.NET 9** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM
- **SQL Server** - Primary database
- **Redis** - Caching layer
- **RabbitMQ** - Message broker
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping
- **Hangfire** - Background job processing
- **Serilog** - Structured logging
- **MailKit** - Email service
- **Polly** - Resilience patterns
- **BCrypt** - Password hashing
- **JWT** - Authentication tokens
- **Swagger** - API documentation
- **xUnit** - Testing framework
- **Docker** - Containerization

## 🚀 Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server (or SQL Server Express)
- Redis (optional, falls back to in-memory cache)
- RabbitMQ (optional)

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd CleanArchitectureApi
   ```

2. **Update connection strings**
   Edit `src/WebApi/CleanArchitectureApi.WebApi/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=CleanArchitectureApiDb_Dev;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }
   ```

3. **Run the application**
   ```bash
   cd src/WebApi/CleanArchitectureApi.WebApi
   dotnet run
   ```

4. **Access the API**
   - Swagger UI: `https://localhost:7000`
   - API Base URL: `https://localhost:7000/api/v1`
   - Health Check: `https://localhost:7000/health`

### Docker Development Setup

1. **Start all services with Docker Compose**
   ```bash
   docker-compose up -d
   ```

2. **Access the services**
   - API: `http://localhost:8080`
   - RabbitMQ Management: `http://localhost:15672` (admin/admin123)
   - Seq Logging: `http://localhost:5341`

## 📚 API Documentation

### Authentication Endpoints

- `POST /api/v1/auth/register` - Register a new user
- `POST /api/v1/auth/login` - Login and get JWT token
- `GET /api/v1/auth/profile` - Get current user profile
- `POST /api/v1/auth/validate-token` - Validate JWT token

### User Management Endpoints

- `GET /api/v1/users` - Get all users (paginated)
- `GET /api/v1/users/{id}` - Get user by ID
- `GET /api/v1/users/me` - Get current user
- `PUT /api/v1/users/{id}` - Update user profile
- `DELETE /api/v1/users/{id}` - Delete user (Admin only)

### Product Management Endpoints

- `GET /api/v1/products` - Get all products (paginated)
- `GET /api/v1/products/{id}` - Get product by ID
- `POST /api/v1/products` - Create new product
- `PUT /api/v1/products/{id}` - Update product
- `PATCH /api/v1/products/{id}/stock` - Update product stock
- `DELETE /api/v1/products/{id}` - Delete product
- `GET /api/v1/products/category/{category}` - Get products by category

## 🔧 Configuration

### Environment Variables (Production)

```bash
# Database
DB_SERVER=your-sql-server
DB_NAME=CleanArchitectureApiDb
DB_USER=your-db-user
DB_PASSWORD=your-db-password

# JWT
JWT_SECRET_KEY=your-super-secret-key-at-least-32-characters
JWT_ISSUER=CleanArchitectureApi
JWT_AUDIENCE=CleanArchitectureApiUsers

# Redis
REDIS_CONNECTION_STRING=your-redis-connection

# RabbitMQ
RABBITMQ_HOST=your-rabbitmq-host
RABBITMQ_PORT=5672
RABBITMQ_USERNAME=your-username
RABBITMQ_PASSWORD=your-password

# Email
SMTP_SERVER=your-smtp-server
SMTP_PORT=587
SMTP_USERNAME=your-email
SMTP_PASSWORD=your-password
FROM_EMAIL=noreply@yourapp.com
FROM_NAME=Your App Name
```

## 🧪 Testing

### Run Unit Tests
```bash
dotnet test tests/UnitTests/CleanArchitectureApi.UnitTests/
```

### Run Integration Tests
```bash
dotnet test tests/IntegrationTests/CleanArchitectureApi.IntegrationTests/
```

### Run All Tests
```bash
dotnet test
```

## 📦 Deployment

### Docker Deployment

1. **Build the Docker image**
   ```bash
   docker build -t cleanarchitectureapi .
   ```

2. **Run with environment variables**
   ```bash
   docker run -d \
     -p 8080:8080 \
     -e DB_SERVER=your-server \
     -e DB_NAME=your-db \
     -e JWT_SECRET_KEY=your-secret \
     cleanarchitectureapi
   ```

### Production Deployment

1. **Set environment variables**
2. **Run database migrations**
3. **Deploy using your preferred method** (Azure, AWS, Kubernetes, etc.)

## 🏗️ Project Structure

```
CleanArchitectureApi/
├── src/
│   ├── Domain/                     # Domain layer
│   │   ├── Common/                 # Base classes and interfaces
│   │   ├── Entities/               # Domain entities
│   │   ├── Events/                 # Domain events
│   │   ├── Specifications/         # Specification pattern
│   │   └── ValueObjects/           # Value objects
│   ├── Application/                # Application layer
│   │   ├── Behaviors/              # MediatR behaviors
│   │   ├── DTOs/                   # Data transfer objects
│   │   ├── Features/               # CQRS commands/queries
│   │   ├── Interfaces/             # Application interfaces
│   │   └── Mappings/               # AutoMapper profiles
│   ├── Infrastructure/             # Infrastructure layer
│   │   ├── Data/                   # EF Core DbContext
│   │   ├── Repositories/           # Repository implementations
│   │   └── Services/               # External service implementations
│   └── WebApi/                     # Presentation layer
│       ├── Controllers/            # API controllers
│       ├── Middleware/             # Custom middleware
│       └── Program.cs              # Application entry point
├── tests/
│   ├── UnitTests/                  # Unit tests
│   └── IntegrationTests/           # Integration tests
├── docker-compose.yml              # Docker Compose configuration
├── Dockerfile                      # Docker configuration
└── README.md                       # This file
```



## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.




