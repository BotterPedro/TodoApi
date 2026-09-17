FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/TodoApi.Domain/TodoApi.Domain.csproj src/TodoApi.Domain/
COPY src/TodoApi.Application/TodoApi.Application.csproj src/TodoApi.Application/
COPY src/TodoApi.Infrastructure/TodoApi.Infrastructure.csproj src/TodoApi.Infrastructure/
COPY src/TodoApi.Api/TodoApi.Api.csproj src/TodoApi.Api/

RUN dotnet restore src/TodoApi.Api/TodoApi.Api.csproj

COPY src/ src/

RUN dotnet publish src/TodoApi.Api/TodoApi.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TodoApi.Api.dll"]