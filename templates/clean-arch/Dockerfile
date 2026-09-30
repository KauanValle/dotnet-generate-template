FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json Directory.Build.props ./
COPY src/Api/ApiTemplate.Api.csproj src/Api/
COPY src/Application/ApiTemplate.Application.csproj src/Application/
COPY src/Domain/ApiTemplate.Domain.csproj src/Domain/
COPY src/Infrastructure/ApiTemplate.Infrastructure.csproj src/Infrastructure/
RUN dotnet restore src/Api/ApiTemplate.Api.csproj

COPY . .
RUN dotnet publish src/Api/ApiTemplate.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
USER app
ENTRYPOINT ["dotnet", "ApiTemplate.Api.dll"]
