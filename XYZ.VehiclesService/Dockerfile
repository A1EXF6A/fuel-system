FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["XYZ.VehiclesService.csproj", "./"]
RUN dotnet restore "XYZ.VehiclesService.csproj"
COPY . .
RUN dotnet publish "XYZ.VehiclesService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "XYZ.VehiclesService.dll"]
