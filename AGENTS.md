# AGENTS.md

Backend for **SplitSpace** (personal/group expense sharing). .NET 10 (net10.0, `LangVersion latest`, nullable, implicit usings, central package management). Git repo root is the workspace root; current work lives in `SplitSpace.Core` on branch `dev`.

## Repo layout — three independent parts

- `SplitSpace.Core/` — the **active backend**: a modular monolith (`SplitSpace.Core.sln`, 17 projects). gRPC + Dapper + Mediator.
- `ExternalFacade/` — separate REST gateway solution (`ExternalFacade.sln`) that calls Core over gRPC. **Different stack**: EF Core packages, Mapperly, xunit 2.x.
- `Environment/` — stale `docker-compose.yml` (Postgres x3, Kafka, kafka-ui, external-facade), `kafka-init.sh`

## SplitSpace.Core — modular monolith

- Single host: `SplitSpace.Core/SplitSpace.Core/Program.cs` composes 3 modules via `XModule.Add(...)`: Auth, Spaces, Finances. Each module = 5 projects: `.{Module}.Domain`, `.{Module}.Dal`, `.{Module}.Logic`, `.{Module}.Grpc`, plus the host module project (e.g. `SplitSpace.Auth`) with `AuthModule.cs` exposing `Add` / `MapGrpc` / `RunMigrations`. `SharedKernel` holds `Result<T>`, DDD primitives, `DatabaseMigrator`.
- Kestrel: port **5002 = HTTP/2 (gRPC)**, **5004 = HTTP/1 (gRPC JSON transcoding + Swagger)** — see `Program.cs:26-31`.
- Persistence is **Dapper + Npgsql, NOT EF Core**. (EF Core appears only in ExternalFacade.)
- Migrations are `.sql` files in `*.Dal/Database/Migrations/`, numbered `00N_name.sql`, goose-style markers (`-- +goose Up` / `-- +goose Down`), auto-embedded by csproj glob and auto-applied at startup by `RunMigrations` → `SharedKernel/Database/DatabaseMigrator.cs` (tracks a `goose_db_version` table). To migrate: add a numbered `.sql` file to a Dal project's `Database/Migrations/` folder; restart the host.
- Connection strings come from **env vars** `AUTH_SERVICE_DB_CONNECTION_STRING`, `SPACE_SERVICE_DB_CONNECTION_STRING`, `FINANCE_SERVICE_DB_CONNECTION_STRING`, each with a `localhost`/`postgres` fallback in `Database/Connections/DbConnectionFactory.cs`. The `ConnectionStrings:AuthServiceDb` key in appsettings is **not read by any code**.
- Mediator (Mediator.SourceGenerator 3.x; the generator is referenced only by the host project, lifetime **scoped**). Handlers live in `*.Logic/Features/<Area>/<Operation>/{Command|Query|Handler|ResultData}.cs`. Queries must be `XQuery : IQuery<...>` even though several reads are misleadingly named `...Command` (`GetExpensesCommand.cs`, `GetDebtsCommand.cs`, etc.). Domain events implement `INotification` and are published after persist via `_mediator.Publish` (see `AddExpenseHandler.cs`).
- Cross-module calls happen **in-process** through `*.Dal/ClientFacades/Implementations/*` (e.g. `AuthServiceClientFacade` → `UserExistCommand`, `SpacesServiceClientFacade` → `GetSpacesQuery`/`GetSpaceMembersQuery`). Hence `Spaces.Dal` → `Auth.Logic` and `Finances.Dal` → `Spaces.Logic` project references are intentional.
- DDD style: aggregates are `sealed record` with private ctor, static `Create(...)` returning `Result<T>` and `Rehydrate(...)` for persistence; **all IDs are UUIDv7** via `Guid.CreateVersion7()`. Handlers return `Result<T>`/`Result` with `Error{Type,Message}`; throw `DomainException` only for true invariants. `TimeProvider.System` is registered (`Program.cs:8`) and injected instead of `DateTime.UtcNow`.
- gRPC: `.proto` files in `*.Grpc/Protos/SplitSpace/<Module>/` (Google `api`/`type` protos are vendored per project); generated code is **not committed** — built into `obj/`. Server impls are hand-written in the host module's `Services/*Grpc.cs` and mapped by `Module.MapGrpc`. ExternalFacade keeps its **own copies** of these protos with `GrpcServices="Client"` — keep them in sync manually.
- Auth: Core's gRPC surface has **no auth middleware**; requests carry `user_id` as a string field that handlers `Guid.Parse`. JWT is only validated on demand via the `AuthService.ValidateToken` RPC (this is how ExternalFacade authenticates).

## ExternalFacade

- REST gateway on **port 8000** (HTTP/1, forced in `Program.cs`; `launchSettings.json`'s 5082 is stale). Controllers pass requests to Core gRPC clients via Dal facades and map responses with Mapperly (`Mappers/ToGrpcMapper.cs`, `FromGrpcMapper.cs` — note they use opposite `RequiredEnumMappingStrategy`).
- gRPC client endpoints are docker hostnames (`http://auth-service:5002`, `http://space-service:5002`, `http://finance-service:5002`) registered in `src/SplitSpace.ExternalFacade.Dal/ServiceCollectionExtensions.cs`.
- Auth: `Middlewares/ExternalAuthenticationHandler.cs` forwards the Bearer token to Core's `ValidateToken` RPC, then sets `HttpContext.Items["UserId"]` (controllers read it via `HttpContextExtensions.GetUserId()`).

## Known broken / gotchas

- `ExternalFacade/tests/SplitSpace.ExternalFacade.UnitTests/` has **zero test files** and a broken `ProjectReference` (points at non-existent `SplitSpace.ExternalFacade.csproj`, actual file is `...Logic.csproj`) → `dotnet restore ExternalFacade.sln` and the ExternalFacade Dockerfile build **fail**. `SplitSpace.Core` has **no test projects** (xunit.v3 packages are declared in `Directory.Packages.props` but unused).
- `Environment/docker-compose.yml` still builds legacy sibling repos (`../AuthService`, `../SpaceService`, `../FinanceService`) that no longer exist after the "refactor into modular monolith" commit; it will not build Core as-is. Kafka is provisioned (topic `space-events`) but **no C# code produces/consumes Kafka events**.
- Route bug in `FinanceController.cs`/`SearchController.cs`: `"spaces/{spaceId::guid}"` (double colon) won't match; `SpaceController.cs` uses the correct `{spaceId:guid}`.
- `ExternalFacade` declares EF Core / JWT / BCrypt packages that are unused (JWT validation is delegated to Core).

## Commands

```powershell
# build / run Core (workdir: SplitSpace.Core)
dotnet build SplitSpace.Core.sln
dotnet run --project SplitSpace.Core   # gRPC 5002, REST 5004

# build Facade (workdir: ExternalFacade) — fails on the broken test project
dotnet build ExternalFacade.sln

# infra (workdir: Environment)
docker compose up -d    # postgres 5432/5433/5434, kafka 9092, kafka-ui 8080
```
