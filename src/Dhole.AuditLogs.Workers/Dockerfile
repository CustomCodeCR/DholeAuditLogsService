# Base image
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
RUN apt-get update && \
  apt-get install -y tzdata apt-utils libgdiplus libc6-dev && \
  ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll && \
  rm -rf /var/lib/apt/lists/*

WORKDIR /app
ENV TZ=America/Costa_Rica

# Build image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Dhole.AuditLogs.Workers/Dhole.AuditLogs.Workers.csproj", "src/Dhole.AuditLogs.Workers/"]
COPY ["src/Dhole.AuditLogs.Api/Dhole.AuditLogs.Api.csproj", "src/Dhole.AuditLogs.Api/"]
COPY ["src/Dhole.AuditLogs.Application/Dhole.AuditLogs.Application.csproj", "src/Dhole.AuditLogs.Application/"]
COPY ["src/Dhole.AuditLogs.Domain/Dhole.AuditLogs.Domain.csproj", "src/Dhole.AuditLogs.Domain/"]
COPY ["src/Dhole.AuditLogs.Persistence/Dhole.AuditLogs.Persistence.csproj", "src/Dhole.AuditLogs.Persistence/"]
COPY ["src/Dhole.AuditLogs.Infrastructure/Dhole.AuditLogs.Infrastructure.csproj", "src/Dhole.AuditLogs.Infrastructure/"]
COPY ["src/Dhole.AuditLogs.Contracts/Dhole.AuditLogs.Contracts.csproj", "src/Dhole.AuditLogs.Contracts/"]

RUN dotnet restore "src/Dhole.AuditLogs.Workers/Dhole.AuditLogs.Workers.csproj"

COPY . .

WORKDIR "/src/src/Dhole.AuditLogs.Workers"
RUN dotnet build "Dhole.AuditLogs.Workers.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

# Publish image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/src/Dhole.AuditLogs.Workers

RUN dotnet publish "Dhole.AuditLogs.Workers.csproj" \
  -c $BUILD_CONFIGURATION \
  -o /app/publish \
  /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN chmod 755 /app

ENTRYPOINT ["dotnet", "Dhole.AuditLogs.Workers.dll"]
