# 1. Base image for execution
FROM mcr.microsoft.com/dotnet/aspnet:10 AS base
WORKDIR /app
EXPOSE 8080

# 2. Base image for compilation
FROM mcr.microsoft.com/dotnet/sdk:10 AS build
WORKDIR /src

# 3. Copy projects and restore packages
COPY ["NotificationsChallenge.sln", "./"]
COPY ["src/NotificationsChallenge.Domain/NotificationsChallenge.Domain.csproj", "src/NotificationsChallenge.Domain/"]
COPY ["src/NotificationsChallenge.Application/NotificationsChallenge.Application.csproj", "src/NotificationsChallenge.Application/"]
COPY ["src/NotificationsChallenge.Infrastructure/NotificationsChallenge.Infrastructure.csproj", "src/NotificationsChallenge.Infrastructure/"]
COPY ["src/NotificationsChallenge.WebApi/NotificationsChallenge.WebApi.csproj", "src/NotificationsChallenge.WebApi/"]
RUN dotnet restore "NotificationsChallenge.sln"

# 4. Copy the rest of the code and build
COPY . .
WORKDIR "/src/src/NotificationsChallenge.WebApi"
RUN dotnet build "NotificationsChallenge.WebApi.csproj" -c Release -o /app/build

# 5. Publish optimized version
FROM build AS publish
RUN dotnet publish "NotificationsChallenge.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 6. Final production image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationsChallenge.WebApi.dll"]