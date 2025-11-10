FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine3.22 AS base
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine3.22 AS build
ARG TARGETARCH
WORKDIR /src

# Install protobuf compiler and glibc compatibility for gRPC
RUN apk add --no-cache protobuf-dev gcompat

COPY --link *.csproj .
RUN dotnet restore "XYZ.RoutesService.csproj" -a $TARGETARCH

COPY --link . .
RUN dotnet publish "XYZ.RoutesService.csproj" -a $TARGETARCH -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --link --from=build /app/publish .
ENTRYPOINT ["dotnet", "XYZ.RoutesService.dll"]
