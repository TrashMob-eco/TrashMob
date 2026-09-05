# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Build argument for environment (dev, pr)
# dev = development/test environment (blocks search crawlers)
# pr = production environment (allows search crawlers)
ARG BUILD_ENVIRONMENT=pr

WORKDIR /src

# Install Node.js for the React build by copying the official Node.js Docker
# image's runtime, instead of NodeSource's apt-repo installer script. That
# script (`setup_20.x | bash`) started silently failing to add its apt repo
# starting 2026-09-04/05 (every deploy since 22:11 UTC 2026-09-04 failed
# identically) — `apt-get install nodejs` then fell back to Ubuntu's own
# bundled `nodejs` package, which doesn't include npm ("npm: not found").
# Copying straight from the official image sidesteps any apt-repo /
# distro-packaging path entirely; kept on Node 20 to match the rest of the
# toolchain (this is a build-time-only dependency, not a runtime one).
COPY --from=node:20-slim /usr/local/bin/node /usr/local/bin/node
COPY --from=node:20-slim /usr/local/bin/npm /usr/local/bin/npm
COPY --from=node:20-slim /usr/local/bin/npx /usr/local/bin/npx
COPY --from=node:20-slim /usr/local/lib/node_modules /usr/local/lib/node_modules

# Copy csproj files and restore dependencies
COPY ["TrashMob/TrashMob.csproj", "TrashMob/"]
COPY ["TrashMob.Shared/TrashMob.Shared.csproj", "TrashMob.Shared/"]
RUN dotnet restore "TrashMob/TrashMob.csproj"

# Copy everything else
COPY . .

# Build the React app
WORKDIR "/src/TrashMob/client-app"
# Remove lockfile to regenerate for Linux platform (handles cross-platform native deps like rollup)
# --ignore-scripts prevents lifecycle scripts (e.g. husky install) from failing in containers
RUN rm -f package-lock.json && npm install --ignore-scripts
RUN npm run build

# Use dev robots.txt for non-production environments (blocks all crawlers)
RUN if [ "$BUILD_ENVIRONMENT" != "pr" ]; then \
        echo "Using dev robots.txt to block crawlers"; \
        cp public/robots-dev.txt build/robots.txt; \
    else \
        echo "Using production robots.txt"; \
    fi

# Build .NET app
WORKDIR "/src/TrashMob"
RUN dotnet build "TrashMob.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TrashMob.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl --fail http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "TrashMob.dll"]
