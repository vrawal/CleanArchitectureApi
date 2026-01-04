# Use the official .NET 9 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY CleanArchitectureApi.sln ./

# Copy project files
COPY src/Domain/CleanArchitectureApi.Domain/CleanArchitectureApi.Domain.csproj src/Domain/CleanArchitectureApi.Domain/
COPY src/Application/CleanArchitectureApi.Application/CleanArchitectureApi.Application.csproj src/Application/CleanArchitectureApi.Application/
COPY src/Infrastructure/CleanArchitectureApi.Infrastructure/CleanArchitectureApi.Infrastructure.csproj src/Infrastructure/CleanArchitectureApi.Infrastructure/
COPY src/WebApi/CleanArchitectureApi.WebApi/CleanArchitectureApi.WebApi.csproj src/WebApi/CleanArchitectureApi.WebApi/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR /src/src/WebApi/CleanArchitectureApi.WebApi
RUN dotnet build -c Release -o /app/build

# Publish the application
RUN dotnet publish -c Release -o /app/publish --no-restore

# Use the official .NET 9 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create a non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Create logs directory and set permissions
RUN mkdir -p /app/logs && chown -R appuser:appuser /app/logs

# Copy the published application
COPY --from=build /app/publish .

# Change ownership of the app directory
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose the port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "CleanArchitectureApi.WebApi.dll"]

