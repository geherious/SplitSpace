# AuthService

Authentication service for SplitSpace microservices architecture.

## 📋 Overview

This service handles user registration, login, and token refresh operations using gRPC synchronous endpoints.

## 🏗 Architecture

- **gRPC Service**: Exposes synchronous authentication endpoints
- **PostgreSQL**: Stores users and refresh tokens
- **JWT Tokens**: Access tokens (15 min) and refresh tokens (7 days)
- **BCrypt**: Password hashing with 12 salt rounds

## 🔐 Endpoints

### Register
```protobuf
rpc Register(RegisterRequest) returns (RegisterResponse);
```

**Request:**
```protobuf
message RegisterRequest {
  string email = 1;
  string password = 2;
}
```

**Response:**
```protobuf
message RegisterResponse {
  uint64 user_id = 1;
  string token_id = 2;
  string access_token = 3;
  string refresh_token = 4;
}
```

### Login
```protobuf
rpc Login(LoginRequest) returns (LoginResponse);
```

**Request:**
```protobuf
message LoginRequest {
  string email = 1;
  string password = 2;
}
```

**Response:**
```protobuf
message LoginResponse {
  uint64 user_id = 1;
  string token_id = 2;
  string access_token = 3;
  string refresh_token = 4;
}
```

### Refresh Tokens
```protobuf
rpc RefreshTokens(RefreshTokenRequest) returns (RefreshTokenResponse);
```

**Request:**
```protobuf
message RefreshTokenRequest {
  string refresh_token = 1;
}
```

**Response:**
```protobuf
message RefreshTokenResponse {
  string access_token = 1;
  string refresh_token = 2;
}
```

## ⚙️ Configuration

### appsettings.json
```json
{
  "Jwt": {
    "Secret": "CHANGE_THIS_SECRET_KEY_IN_PRODUCTION",
    "Issuer": "SplitSpace.AuthService",
    "Audience": "SplitSpace.Clients",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "ConnectionStrings": {
    "AuthServiceDb": "Host=localhost;Database=auth_db;Username=postgres;Password=postgres"
  }
}
```

## 🚀 Running Locally

```bash
# Restore dependencies
dotnet restore

# Run the service
dotnet run --project src/SplitSpace.AuthService
```

## 🐳 Docker

```bash
cd Environment
docker-compose up --build
```

## 🔒 Security Features

- Password hashing with BCrypt (12 salt rounds)
- JWT tokens with short-lived access tokens (15 min)
- Rotating refresh token pattern
- Token validation and expiration checks
- Email format validation
- Minimum password length (8 characters)

## 📦 Dependencies

- **Grpc.AspNetCore**: gRPC server implementation
- **Microsoft.EntityFrameworkCore**: Data access layer
- **Npgsql.EntityFrameworkCore.PostgreSQL**: PostgreSQL provider
- **System.IdentityModel.Tokens.Jwt**: JWT token handling
- **BCrypt.Net-Next**: Password hashing

## 🔧 Error Handling

All errors are returned with:
- HTTP Status Code
- gRPC Status Code
- Service Code (01 for Auth Service)
- Business Error Code
- Descriptive Message

Error types:
- `Validation`: Invalid input data (400 Bad Request)
- `FailedPrecondition`: User already exists, etc. (409 Conflict)
- `Unauthenticated`: Invalid credentials or expired token (401 Unauthorized)

## 📝 Notes

- Replace the JWT secret key with a strong random string in production
- Use environment variables for connection strings in production
- All services are registered as scoped dependencies
- No HTTP controllers - only gRPC endpoints
