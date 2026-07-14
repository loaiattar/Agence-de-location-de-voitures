FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/CarAgence.Domain/CarAgence.Domain.csproj src/CarAgence.Domain/
COPY src/CarAgence.Data/CarAgence.Data.csproj src/CarAgence.Data/
COPY src/CarAgence.Web/CarAgence.Web.csproj src/CarAgence.Web/
RUN dotnet restore src/CarAgence.Web/CarAgence.Web.csproj

COPY src/ src/
RUN dotnet publish src/CarAgence.Web/CarAgence.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN groupadd -r appuser && useradd -r -g appuser appuser && \
    mkdir -p /data && chown appuser:appuser /data

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__DefaultConnection="Data Source=/data/caragence.db"

EXPOSE 8080

USER appuser

ENTRYPOINT ["dotnet", "CarAgence.Web.dll"]
