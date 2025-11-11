# Etapa 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia apenas os arquivos de projeto para restaurar as dependências
COPY Loterias.QueryApiService/Loterias.QueryApiService.csproj Loterias.QueryApiService/
COPY Loterias.Shared/Loterias.Shared.csproj Loterias.Shared/
COPY Loterias.RedisCacheService/Loterias.RedisCacheService.csproj Loterias.RedisCacheService/

# Copia o appsettings.json (precisa estar visível pro MSBuild)
COPY Loterias.QueryApiService/appsettings.json Loterias.QueryApiService/

# Restaura as dependências
RUN dotnet restore Loterias.QueryApiService/Loterias.QueryApiService.csproj

# Copia todo o código-fonte
COPY . .

WORKDIR /app/Loterias.QueryApiService

# Publica o projeto
RUN dotnet publish Loterias.QueryApiService.csproj -c Release -o /out /p:UseAppHost=false

# Etapa 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Loterias.QueryApiService.dll"]
