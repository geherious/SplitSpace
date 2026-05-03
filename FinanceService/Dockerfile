# syntax=docker/dockerfile:1.4

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

ENV NUGET_FALLBACK_PACKAGES=""
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
ENV DOTNET_NOLOGO=true
ENV NUGET_PACKAGES=/root/.nuget/packages

COPY FinanceService.sln ./
COPY Directory.Packages.props ./
COPY Directory.Build.props ./
COPY Directory.Build.targets ./
COPY . .

RUN dotnet restore FinanceService.sln

RUN dotnet publish src/SplitSpace.FinanceService/SplitSpace.FinanceService.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SplitSpace.FinanceService.dll"]