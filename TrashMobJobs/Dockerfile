FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["TrashMobJobs/TrashMobJobs.csproj", "TrashMobJobs/"]
COPY ["TrashMob.Shared/TrashMob.Shared.csproj", "TrashMob.Shared/"]
RUN dotnet restore "TrashMobJobs/TrashMobJobs.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/TrashMobJobs"
RUN dotnet build "TrashMobJobs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrashMobJobs.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrashMobJobs.dll"]