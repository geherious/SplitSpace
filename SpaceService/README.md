# SpaceService

SpaceService is a microservice in the SplitSpace platform responsible for space management operations. It provides CRUD functionality for spaces and follows the same architecture pattern as AuthService.

## 📌 Overview

- **Purpose**: Manage spaces (create, read, update, delete)
- **Database**: Dedicated PostgreSQL instance
- **Authentication**: External auth via AuthService (accepts userId in requests)
- **Communication**: HTTP/gRPC endpoints
- **Result Pattern**: Uses `Result<T>` for all operations

---

## 🏗️ Project Structure

```
SpaceService/
├── src/
│   ├── SplitSpace.SpaceService/           # Main (API layer)
│   │   ├── Controllers/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── SplitSpace.SpaceService.csproj
│   ├── SplitSpace.SpaceService.Logic/     # Business logic layer
│   │   ├── Services/
│   │   │   ├── ISpaceService.cs
│   │   │   └── Implementations/
│   │   │       └── SpaceService.cs
│   │   ├── Options/
│   │   └── SplitSpace.SpaceService.Logic.csproj
│   ├── SplitSpace.SpaceService.Dal/       # Data access layer
│   │   ├── Models/
│   │   │   └── Entities/
│   │   │       └── Space.cs
│   │   ├── Repositories/
│   │   │   ├── ISpaceRepository.cs
│   │   │   └── Implementations/
│   │   │       └── SpaceRepository.cs
│   │   ├── ServiceCollectionExtensions.cs
│   │   └── SplitSpace.SpaceService.Dal.csproj
│   ├── SplitSpace.SpaceService.Common/    # Shared models
│   │   ├── Models/
│   │   │   ├── Result.cs
│   │   │   ├── Error.cs
│   │   │   ├── ErrorType.cs
│   │   │   ├── Commands/
│   │   │   │   ├── GetSpaceCommand.cs
│   │   │   │   ├── CreateSpaceCommand.cs
│   │   │   │   ├── UpdateSpaceCommand.cs
│   │   │   │   └── DeleteSpaceCommand.cs
│   │   │   └── Results/
│   │   │       ├── GetSpaceResultData.cs
│   │   │       ├── CreateSpaceResultData.cs
│   │   │       ├── UpdateSpaceResultData.cs
│   │   │       └── DeleteSpaceResultData.cs
│   │   └── SplitSpace.SpaceService.Common.csproj
│   └── SplitSpace.SpaceService.Grpc/      # gRPC contracts
│       ├── Protos/
│       │   └── SplitSpace/SpaceService/SpaceService.proto
│       └── SplitSpace.SpaceService.Grpc.csproj
└── tests/
    └── SplitSpace.SpaceService.UnitTests/  # Unit tests
        └── SplitSpace.SpaceService.UnitTests.csproj
```

---

## 📦 Dependencies

### **SplitSpace.SpaceService.Common**
- **SDK:** Microsoft.NET.Sdk
- **NuGet Packages:** None (leaf layer)

### **SplitSpace.SpaceService.Dal**
- **SDK:** Microsoft.NET.Sdk
- **NuGet Packages:**
  - Microsoft.EntityFrameworkCore
  - Npgsql.EntityFrameworkCore.PostgreSQL
- **References:** Common

### **SplitSpace.SpaceService.Logic**
- **SDK:** Microsoft.NET.Sdk
- **NuGet Packages:** None (uses packages from other layers)
- **References:** Dal

### **SplitSpace.SpaceService.Grpc**
- **SDK:** Microsoft.NET.Sdk
- **NuGet Packages:**
  - Grpc.AspNetCore
  - Google.Api.CommonProtos
- **Configuration:** Generates C# from proto files

### **SplitSpace.SpaceService** (Main)
- **SDK:** Microsoft.NET.Sdk.Web
- **NuGet Packages:**
  - Grpc.AspNetCore
  - Google.Protobuf
  - Grpc.AspNetCore.Server.Reflection
  - Grpc.Core.Api
  - Grpc.Net.Client
  - Microsoft.AspNetCore.Grpc.JsonTranscoding
  - Microsoft.AspNetCore.Grpc.Swagger
  - Swashbuckle.AspNetCore
  - Microsoft.EntityFrameworkCore.Design
  - Microsoft.EntityFrameworkCore.Tools
- **References:** Logic, Dal

### **SplitSpace.SpaceService.UnitTests**
- **SDK:** Microsoft.NET.Sdk
- **NuGet Packages:**
  - coverlet.collector (code coverage)
  - Microsoft.NET.Test.Sdk
  - xunit
  - xunit.runner.visualstudio
- **References:** Logic (or Main)

---

## 🧱 Architecture Layers

### **1. Common Layer**
Contains shared models and constants:
- `Result<T>` - Generic result pattern for success/failure responses
- `Error` - Error model with Type, Code, Message
- `ErrorType` - Enum: Validation, FailedPrecondition, Unauthenticated
- Commands: GetSpaceCommand, CreateSpaceCommand, UpdateSpaceCommand, DeleteSpaceCommand
- Results: GetSpaceResultData, CreateSpaceResultData, UpdateSpaceResultData, DeleteSpaceResultData

### **2. Dal Layer**
Data access layer using Entity Framework Core:
- `SpaceServiceDbContext` - EF Core DbContext with Spaces DbSet
- `Space` entity - Space model with Id, Name, Description, CreatedAt
- `ISpaceRepository` / `SpaceRepository` - Space operations (GetById, Create, Update, Delete)

### **3. Logic Layer**
Business logic and service implementations:
- `ISpaceService` - Service interface defining operations
- `SpaceService` - Service implementation with CRUD operations

### **4. Grpc Layer**
gRPC service contracts:
- `SpaceService.proto` - Service definition with GetSpace, CreateSpace, UpdateSpace, DeleteSpace methods

### **5. Main Layer**
API endpoints and application entry point:
- HTTP/gRPC endpoints (Controllers)
- Request routing
- Middleware configuration
- Application startup (Program.cs)

---

## 🔄 Result Pattern

All service operations return `Result<T>`:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public List<Error> Errors { get; }
    public T? Value { get; private set; }

    public static Result<T> Success(T value) => new(true, [], value);
    public static Result<T> Failure(List<Error> errors) => new(false, errors, default);
    public static Result<T> Failure(Error error) => new(false, [error], default);
}
```

**Error Structure:**
```csharp
public record Error
{
    public required ErrorType Type { get; set; }  // Validation, FailedPrecondition, Unauthenticated
    public string? Code { get; set; }            // "SPACE_NOT_FOUND" etc.
    public required string Message { get; set; }
}
```

---

## 📋 Command/Result Models (Record Pattern)

### **Commands:**
```csharp
public record GetSpaceCommand(Guid SpaceId);
public record CreateSpaceCommand(string Name, string Description, Guid? OwnerUserId = null);
public record UpdateSpaceCommand(Guid SpaceId, string? Name = null, string? Description = null);
public record DeleteSpaceCommand(Guid SpaceId);
```

### **Results:**
```csharp
public record Space
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public record GetSpaceResultData(Space space);
public record CreateSpaceResultData(Space space, string? AccessToken = null);
public record UpdateSpaceResultData(Space space, string? AccessToken = null);
public record DeleteSpaceResultData(bool success);
```

---

## 🗄️ Database Schema

### **Space Table:**
```sql
CREATE TABLE space (
    Id UUID PRIMARY KEY,              -- Guid.Version7
    Name VARCHAR(255) UNIQUE NOT NULL,
    Description TEXT NOT NULL,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL
);
```

### **DbContext Configuration:**
- Connection string from environment variable `SPACE_SERVICE_DB_CONNECTION_STRING`
- Default: `Host=localhost;Database=space_db;Username=postgres;Password=postgres`
- Uses PostgreSQL via Npgsql.EntityFrameworkCore.PostgreSQL
- Unique index on Name field

---

## 🔐 Authentication Strategy

SpaceService accepts **userId** in requests for authorization (external auth via AuthService). The service does not handle authentication itself - it trusts that the caller has been authenticated by AuthService.

**Request Pattern:**
```csharp
public record GetSpaceCommand(Guid SpaceId);  // Can include userId for authorization checks
```

---

## 📝 Directory.Build.props

Global settings for all projects:
```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>latest</LangVersion>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  </PropertyGroup>
</Project>
```

---

## 🛠️ EF Core Migrations

Migrations are located in `SplitSpace.SpaceService.Dal/Migrations/`:
- Run migrations: `dotnet ef migrations add <name>`
- Apply migrations: `dotnet ef database update`
- Snapshot file tracks schema state

---

## 📊 Service Dependencies Summary

| Project | Dependencies | Purpose |
|---------|-------------|---------|
| **Common** | None (leaf layer) | Shared models, commands, results |
| **Dal** | Common | Data access, repositories, DbContext |
| **Logic** | Dal | Business logic, services |
| **Grpc** | None (standalone) | Service contracts for inter-service communication |
| **Main** | Logic, Dal, Grpc | API endpoints, routing, middleware |
| **UnitTests** | Logic (or Main) | Unit tests with xunit framework |

---

## 🚀 Deployment Considerations

1. **Environment Variables:**
   - `SPACE_SERVICE_DB_CONNECTION_STRING` - Database connection string

2. **Database:**
   - PostgreSQL database required
   - Run migrations before deployment

3. **gRPC Contracts:**
   - Proto files in `Protos/SplitSpace/SpaceService/`
   - Generated C# code in `generated/` directory

---

## 📚 Related Documentation

- [AUTHITECTURE.md](../Environment/.opencode/info/ARCHITECTURE.md) - Overall microservices architecture
- [AuthService.md](../Environment/.opencode/info/AuthService.md) - AuthService architecture reference
