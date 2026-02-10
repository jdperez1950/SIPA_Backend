# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and csproj files
COPY Pavis.sln ./
COPY src/Pavis.WebApi/Pavis.WebApi.csproj src/Pavis.WebApi/
COPY src/Pavis.Application/Pavis.Application.csproj src/Pavis.Application/
COPY src/Pavis.Domain/Pavis.Domain.csproj src/Pavis.Domain/
COPY src/Pavis.Infrastructure/Pavis.Infrastructure.csproj src/Pavis.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Clean any previous build artifacts
RUN rm -rf bin/ obj/ out/

# Copy rest of code
COPY . .

# Build and publish
WORKDIR /app/src/Pavis.WebApi
RUN dotnet publish -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Expose port
EXPOSE 8080

# Entry point
ENTRYPOINT ["dotnet", "Pavis.WebApi.dll"]
