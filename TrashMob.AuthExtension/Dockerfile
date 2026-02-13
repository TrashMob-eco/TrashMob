FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["TrashMob.AuthExtension/TrashMob.AuthExtension.csproj", "TrashMob.AuthExtension/"]
RUN dotnet restore "TrashMob.AuthExtension/TrashMob.AuthExtension.csproj"

COPY . .
WORKDIR "/src/TrashMob.AuthExtension"
RUN dotnet build "TrashMob.AuthExtension.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrashMob.AuthExtension.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "TrashMob.AuthExtension.dll"]
