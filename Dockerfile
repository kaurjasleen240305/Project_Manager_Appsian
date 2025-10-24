# Use .NET 9.0 SDK for build and runtime
FROM mcr.microsoft.com/dotnet/sdk

# Set working directory
WORKDIR /app

# Copy all source files
COPY . .

# Expose application port
EXPOSE 8080

# Command: build the project every time container starts, then run it
CMD dotnet build MiniProjectManager.csproj -c Release && dotnet run --project MiniProjectManager.csproj --urls "http://0.0.0.0:8080"
