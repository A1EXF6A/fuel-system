FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5004

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["XYZ.RoutesService.csproj", "./"]
RUN dotnet restore "XYZ.RoutesService.csproj"
COPY . .
RUN dotnet publish "XYZ.RoutesService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "XYZ.RoutesService.dll"]
