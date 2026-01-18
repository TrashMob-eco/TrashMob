FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["TrashMobDailyJobs/TrashMobDailyJobs.csproj", "TrashMobDailyJobs/"]
COPY ["TrashMob.Shared/TrashMob.Shared.csproj", "TrashMob.Shared/"]
COPY ["TrashMob.Models/TrashMob.Models.csproj", "TrashMob.Models/"]
RUN dotnet restore "TrashMobDailyJobs/TrashMobDailyJobs.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/TrashMobDailyJobs"
RUN dotnet build "TrashMobDailyJobs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrashMobDailyJobs.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrashMobDailyJobs.dll"]
