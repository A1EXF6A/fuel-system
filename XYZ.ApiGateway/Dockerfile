FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["XYZ.ApiGateway/XYZ.ApiGateway.csproj", "XYZ.ApiGateway/"]
COPY ["XYZ.AuthService/Protos/", "XYZ.AuthService/Protos/"]
COPY ["XYZ.DriversService/Protos/", "XYZ.DriversService/Protos/"]
RUN dotnet restore "XYZ.ApiGateway/XYZ.ApiGateway.csproj"
COPY . .
WORKDIR "/src/XYZ.ApiGateway"
RUN dotnet build "XYZ.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "XYZ.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "XYZ.ApiGateway.dll"]