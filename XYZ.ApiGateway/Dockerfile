FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine3.22 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine3.22 AS build
ARG TARGETARCH
WORKDIR /src

# Install protobuf compiler and glibc compatibility for gRPC
RUN apk add --no-cache protobuf-dev gcompat

COPY --link ["XYZ.ApiGateway.csproj", "XYZ.ApiGateway/"]
RUN dotnet restore "XYZ.ApiGateway/XYZ.ApiGateway.csproj" -a $TARGETARCH

COPY --link ["../XYZ.AuthService/Protos/", "XYZ.AuthService/Protos/"]
COPY --link ["../XYZ.DriversService/Protos/", "XYZ.DriversService/Protos/"]

# COPY --link . .
# WORKDIR "/src/XYZ.ApiGateway"
# RUN dotnet build "XYZ.ApiGateway.csproj" -a $TARGETARCH -c Release -o /app/build
#
# FROM build AS publish

COPY --link . .
RUN dotnet publish "XYZ.ApiGateway.csproj" -a $TARGETARCH -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --link --from=build /app/publish .
# Set default port but allow override via --urls parameter
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "XYZ.ApiGateway.dll"]
