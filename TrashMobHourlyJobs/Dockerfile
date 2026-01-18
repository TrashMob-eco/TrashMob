FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["TrashMobHourlyJobs/TrashMobHourlyJobs.csproj", "TrashMobHourlyJobs/"]
COPY ["TrashMob.Shared/TrashMob.Shared.csproj", "TrashMob.Shared/"]
COPY ["TrashMob.Models/TrashMob.Models.csproj", "TrashMob.Models/"]
RUN dotnet restore "TrashMobHourlyJobs/TrashMobHourlyJobs.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/TrashMobHourlyJobs"
RUN dotnet build "TrashMobHourlyJobs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrashMobHourlyJobs.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrashMobHourlyJobs.dll"]
