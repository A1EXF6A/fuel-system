# Use the official .NET 8.0 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["XYZ.DriversService.csproj", "./"]
RUN dotnet restore "XYZ.DriversService.csproj"

# Copy the entire project and build
COPY . .
RUN dotnet build "XYZ.DriversService.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "XYZ.DriversService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

ENTRYPOINT ["dotnet", "XYZ.DriversService.dll"]