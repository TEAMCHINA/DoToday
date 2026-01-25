# Stage 1: Build frontend
FROM node:22-alpine AS frontend-build
WORKDIR /app/client
COPY dotoday.client/package*.json ./
RUN npm ci
COPY dotoday.client/ ./
RUN npm run build

# Stage 2: Build backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /src

# Copy csproj and restore dependencies
COPY DoToday.Server/DoToday.Server.csproj ./DoToday.Server/
WORKDIR /src/DoToday.Server
RUN dotnet restore

# Copy source code
WORKDIR /src
COPY DoToday.Server/ ./DoToday.Server/

# Copy frontend build to wwwroot
COPY --from=frontend-build /app/client/dist ./DoToday.Server/wwwroot/

# Publish
WORKDIR /src/DoToday.Server
RUN dotnet publish -c Release -o /app --no-restore

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create data directory for SQLite database
RUN mkdir -p /data

COPY --from=backend-build /app ./

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DB_PATH=/data/dotoday.db

EXPOSE 8080

ENTRYPOINT ["dotnet", "DoToday.Server.dll"]
