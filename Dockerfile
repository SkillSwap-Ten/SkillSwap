# syntax=docker/dockerfile:1.7
# ──── Build stage ────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution-level config first (best cache layer)
COPY ["global.json", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["SkillSwap.sln", "./"]

# Copy csproj files for restore (preserves layer cache when only sources change)
COPY ["src/SkillSwap.Domain/SkillSwap.Domain.csproj", "src/SkillSwap.Domain/"]
COPY ["src/SkillSwap.Application/SkillSwap.Application.csproj", "src/SkillSwap.Application/"]
COPY ["src/SkillSwap.Infrastructure/SkillSwap.Infrastructure.csproj", "src/SkillSwap.Infrastructure/"]
COPY ["src/SkillSwap.API/SkillSwap.API.csproj", "src/SkillSwap.API/"]
COPY ["src/SkillSwap.Shared/SkillSwap.Shared.csproj", "src/SkillSwap.Shared/"]

# Restore only the API closure (Application + Domain + Infrastructure + Shared come transitively)
RUN dotnet restore "src/SkillSwap.API/SkillSwap.API.csproj"

# Copy sources and publish
COPY src/ src/
RUN dotnet publish "src/SkillSwap.API/SkillSwap.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ──── Runtime stage ────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# wget is required by HEALTHCHECK below; the slim base image does not include it.
RUN apt-get update \
    && apt-get install -y --no-install-recommends wget \
    && rm -rf /var/lib/apt/lists/*

# Non-root user
RUN groupadd -r app && useradd -r -g app app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

USER app
COPY --from=build --chown=app:app /app/publish .

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "SkillSwap.API.dll"]
