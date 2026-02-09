# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["TrashMobMCP/TrashMobMCP.csproj", "TrashMobMCP/"]
COPY ["TrashMob.Shared/TrashMob.Shared.csproj", "TrashMob.Shared/"]
COPY ["TrashMob.Models/TrashMob.Models.csproj", "TrashMob.Models/"]
RUN dotnet restore "TrashMobMCP/TrashMobMCP.csproj"

# Copy source and build
COPY . .
WORKDIR "/src/TrashMobMCP"
RUN dotnet build "TrashMobMCP.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TrashMobMCP.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrashMobMCP.dll"]
