FROM mcr.microsoft. com/dotnet/aspnet: 8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/ProductCatalog.API/ProductCatalog.API.csproj", "ProductCatalog.API/"]
COPY ["src/ProductCatalog.Application/ProductCatalog.Application. csproj", "ProductCatalog.Application/"]
COPY ["src/ProductCatalog.Domain/ProductCatalog.Domain. csproj", "ProductCatalog.Domain/"]
COPY ["src/ProductCatalog.Infrastructure/ProductCatalog.Infrastructure.csproj", "ProductCatalog.Infrastructure/"]

RUN dotnet restore "ProductCatalog.API/ProductCatalog.API.csproj"

COPY src/ . 

WORKDIR "/src/ProductCatalog.API"
RUN dotnet build "ProductCatalog.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductCatalog.API. csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductCatalog.API.dll"]