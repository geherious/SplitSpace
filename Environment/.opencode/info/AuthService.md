# AuthService Architecture Documentation

## 📌 Overview

AuthService is a .NET microservice responsible for user authentication and authorization in the SplitSpace platform. It implements JWT-based authentication with access/refresh token rotation pattern.

---

## 🏗️ Project Structure

```
AuthService/
├── src/
│   ├── SplitSpace.AuthService/           # Main (API layer)
│   │   └── SplitSpace.AuthService.csproj
│   ├── SplitSpace.AuthService.Logic/     # Business logic layer
│   │   ├── Services/
│   │   │   ├── IAuthService.cs
│   │   │   ├── ITokenService.cs
│   │   │   └── Implementations/
│   │   │       ├── AuthService.cs
│   │   │       └── TokenService.cs
│   │   ├── Options/JwtOptions.cs
│   │   └── SplitSpace.AuthService.Logic.csproj
│   ├── SplitSpace.AuthService.Dal/       # Data access layer
│   │   ├── Models/
│   │   │   └── Entities/
│   │   │       ├── User.cs
│   │   │       └── RefreshToken.cs
│   │   ├── Repositories/
│   │   │   ├── IUserRepository.cs
│   │   │   ├── IRefreshTokenRepository.cs
│   │   │   └── Implementations/
│   │   │       ├── UserRepository.cs
│   │   │       └── RefreshTokenRepository.cs
│   │   ├── AuthServiceDbContext.cs
│   │   └── SplitSpace.AuthService.Dal.csproj
│   ├── SplitSpace.AuthService.Common/    # Shared models
│   │   ├── Constants/
│   │   │   ├── JwtClaims.cs
│   │   │   └── RegisteredJwtClaims.cs
│   │   ├── Models/
│   │   │   ├── Commands/
│   │   │   │   ├── LoginCommand.cs
│   │   │   │   ├── RegisterCommand.cs
│   │   │   │   ├── RefreshTokensCommand.cs
│   │   │   │   └── ValidateTokenCommand.cs
│   │   │   ├── Results/
│   │   │   │   ├── LoginResultData.cs
│   │   │   │   ├── RegisterResultData.cs
│   │   │   │   ├── RefreshTokensResultData.cs
│   │   │   │   └── ValidateTokenResultData.cs
│   │   │   ├── Error.cs
│   │   │   ├── ErrorType.cs
│   │   │   └── Result.cs
│   │   └── SplitSpace.AuthService.Common.csproj
│   └── SplitSpace.AuthService.Grpc/      # gRPC contracts
│       ├── Protos/
│       │   └── SplitSpace/AuthService/AuthService.proto
│       └── SplitSpace.AuthService.Grpc.csproj
└── tests/
    └── SplitSpace.AuthService.UnitTests/  # Unit tests
        └── SplitSpace.AuthService.UnitTests.csproj
```

---

## 📦 Project Dependencies

### **SplitSpace.AuthService.Common**
- **SDK:** Microsoft.NET.Sdk
- **NuGet Packages:**
  - Microsoft.Extensions.DependencyInjection.Abstractions

### **SplitSpace.AuthService.Dal**
- **SDK:** Microsoft.NET.Sdk
- **NuGet Packages:**
  - Microsoft.EntityFrameworkCore
  - Npgsql.EntityFrameworkCore.PostgreSQL
- **References:** Common

### **SplitSpace.AuthService.Logic**
- **SDK:** Microsoft.NET.Sdk
- **NuGet Packages:**
  - BCrypt.Net-Next (password hashing)
  - Microsoft.IdentityModel.Tokens
  - System.IdentityModel.Tokens.Jwt
- **References:** Dal, Common, Grpc

### **SplitSpace.AuthService** (Main)
- **SDK:** Microsoft.NET.Sdk.Web
- **NuGet Packages:**
  - Grpc.AspNetCore
  - Google.Protobuf
  - Grpc.AspNetCore.Server.Reflection
  - Grpc.Core.Api
  - Microsoft.AspNetCore.Grpc.JsonTranscoding
  - Microsoft.AspNetCore.Grpc.Swagger
  - Swashbuckle.AspNetCore
  - Microsoft.EntityFrameworkCore.Design
  - Microsoft.EntityFrameworkCore.Tools
- **References:** Logic, Dal

### **SplitSpace.AuthService.Grpc**
- **SDK:** Microsoft.NET.Sdk
- **NuGet Packages:**
  - Grpc.AspNetCore
  - Google.Api.CommonProtos
- **Configuration:** Generates C# from proto files

### **SplitSpace.AuthService.UnitTests**
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
Contains shared models and constants used across all layers:
- `JwtClaims` - JWT claim type constants (`sub`, `jti`)
- `RegisteredJwtClaims` - Registered JWT claims
- `Commands` - Command DTOs (LoginCommand, RegisterCommand, etc.)
- `Results` - Result DTOs (LoginResultData, RegisterResultData, etc.)
- `Error` - Error model with Type, Code, Message
- `ErrorType` - Enum: Validation, FailedPrecondition, Unauthenticated
- `Result<T>` - Generic result pattern for success/failure responses

### **2. Dal Layer**
Data access layer using Entity Framework Core:
- `AuthServiceDbContext` - EF Core DbContext with Users and RefreshTokens
- `User` entity - User model with Id, Email, PasswordHash, CreatedAt, LastLogin
- `RefreshToken` entity - Token model with Id, UserId, Token, ExpiresAt, CreatedAt
- `IUserRepository` / `UserRepository` - User operations (GetById, FindByEmail, Create, UpdateLastLogin)
- `IRefreshTokenRepository` / `RefreshTokenRepository` - Token operations (GetByToken, Create, Revoke, GetAllUserTokens)

### **3. Logic Layer**
Business logic and service implementations:
- `AuthService` - Authentication service implementing IAuthService
  - `RegisterAsync()` - User registration with password hashing
  - `LoginAsync()` - User login with session revocation
  - `RefreshTokenAsync()` - Token refresh with rotation pattern
  - `ValidateTokenAsync()` - Access token validation
- `TokenService` - JWT token creation and validation
  - `CreateAccessToken()` - Creates JWT access token
  - `CreateRefreshToken()` - Creates random base64 refresh token
  - `ValidateAccessToken()` - Validates JWT tokens
- `PasswordHasher` - BCrypt password hashing utility

### **4. Grpc Layer**
gRPC service contracts:
- `AuthService.proto` - Service definition with Register, Login, RefreshToken, ValidateToken methods
- Generated C# code in `/generated/` directory

### **5. Main Layer**
API endpoints and application entry point:
- HTTP/gRPC endpoints (Controllers)
- Request routing
- Middleware configuration
- Application startup

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
    public string? Code { get; set; }            // "INVALID_EMAIL", "TOKEN_EXPIRED" etc.
    public required string Message { get; set; }
}
```

---

## 📋 Command/Result Models (Record Pattern)

### **Commands:**
```csharp
public record LoginCommand(string Email, string Password);
public record RegisterCommand(string Email, string Password);
public record RefreshTokensCommand(string RefreshToken);
public record ValidateTokenCommand(string AccessToken);
```

### **Results:**
```csharp
public record LoginResultData(Guid UserId, string AccessToken, string RefreshToken);
public record RegisterResultData(Guid UserId, string AccessToken, string RefreshToken);
public record RefreshTokensResultData(string AccessToken, string RefreshToken);
public record ValidateTokenResultData(Guid UserId);
```

---

## 🔐 JWT Token Configuration

### **JwtOptions:**
```csharp
public class JwtOptions
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpiryMinutes { get; set; }
    public int RefreshTokenExpiryDays { get; set; }
}
```

### **JWT Claims:**
```csharp
public static class JwtClaims
{
    public const string Sub = "sub";      // User ID
    public const string Jti = "jti";      // Token ID
}
```

### **JWT Payload Structure:**
- `sub`: User ID (Guid.Version7)
- `jti`: Token ID (Guid)
- `exp`: Expiration timestamp
- `iss`: Issuer (e.g., "SplitSpace.AuthService")
- `aud`: Audience (e.g., "SplitSpace.Clients")

---

## 🗄️ Database Schema

### **User Table:**
```sql
CREATE TABLE user (
    Id UUID PRIMARY KEY,              -- Guid.Version7
    Email VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL,
    LastLogin TIMESTAMP WITH TIME ZONE
);
```

### **RefreshToken Table:**
```sql
CREATE TABLE refresh_token (
    Id UUID PRIMARY KEY,              -- Guid.Version7
    UserId UUID NOT NULL,             -- Foreign key to User
    Token VARCHAR(512) NOT NULL,      -- Base64 encoded random bytes (512 bits)
    ExpiresAt TIMESTAMP WITH TIME ZONE NOT NULL,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL,
    UNIQUE (Token),                   -- Unique index on token for rotation
    FOREIGN KEY (UserId) REFERENCES user(Id) ON DELETE CASCADE
);
```

### **DbContext Configuration:**
- Connection string from environment variable `AUTH_SERVICE_DB_CONNECTION_STRING`
- Default: `Host=localhost;Database=auth_db;Username=postgres;Password=postgres`
- Uses PostgreSQL via Npgsql.EntityFrameworkCore.PostgreSQL

---

## 🔒 Security Pattern

### **Access Token (JWT):**
- Short-lived JWT with signature (HS256)
- Contains claims: sub, jti, exp, iss, aud
- Expiration configured via JwtOptions.AccessTokenExpiryMinutes

### **Refresh Token:**
- Base64 encoded random 512-bit bytes
- Stored in database with userId and expiration
- Rotating pattern: old token revoked when new one issued

### **Token Revocation:**
- On login: all existing refresh tokens for user are revoked
- On token refresh: used token is revoked (rotating pattern)
- Prevents session hijacking and unauthorized access

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

## 🛠️ Entity Framework Migrations

Migrations are located in `SplitSpace.AuthService.Dal/Migrations/`:
- InitialCreate.cs - First migration creating base schema
- AddForeignKeys.cs - Subsequent migrations adding foreign keys and indexes
- Snapshot file tracks schema state

---

## 📊 Service Dependencies Summary

| Project | Dependencies | Purpose |
|---------|-------------|---------|
| **Common** | None (leaf layer) | Shared models, constants, DTOs |
| **Dal** | Common | Data access, repositories, DbContext |
| **Logic** | Dal, Common, Grpc | Business logic, services, password hashing |
| **Main** | Logic, Dal | API endpoints, routing, middleware |
| **Grpc** | None (standalone) | Service contracts for inter-service communication |
| **UnitTests** | Logic (or Main) | Unit tests with xunit framework |

---

## 🧪 Testing Strategy

- All tests located in `tests/SplitSpace.AuthService.UnitTests/`
- Uses xunit testing framework
- Code coverage measured by coverlet.collector
- Tests reference Logic or Main project for integration testing

---

## 🚀 Deployment Considerations

1. **Environment Variables:**
   - `AUTH_SERVICE_DB_CONNECTION_STRING` - Database connection string

2. **JWT Configuration:**
   - Configure JwtOptions in appsettings.json or environment variables
   - Set Secret, Issuer, Audience appropriately

3. **Database:**
   - PostgreSQL database required
   - Run migrations before deployment

4. **Token Expiry:**
   - Adjust AccessTokenExpiryMinutes and RefreshTokenExpiryDays based on security requirements

---

## 🔍 Key Files Reference

| File | Purpose |
|------|---------|
| `AuthService.cs` | Main authentication service implementation |
| `TokenService.cs` | JWT token creation and validation |
| `PasswordHasher.cs` | BCrypt password hashing utility |
| `AuthServiceDbContext.cs` | EF Core DbContext configuration |
| `AuthService.proto` | gRPC service contract definition |
| `JwtOptions.cs` | JWT configuration options |

---

## 📚 Related Documentation

- [ARCHITECTURE.md](./ARCHITECTURE.md) - Overall microservices architecture
- [database.md](./database.md) - Database schema documentation
- [overview.md](./overview.md) - Project overview and setup
