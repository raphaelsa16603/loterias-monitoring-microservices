# Etapa 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas os arquivos de projeto para restaurar as dependências
COPY ["Loterias.QueryApiService/Loterias.QueryApiService.csproj", "Loterias.QueryApiService/"]
COPY ["Loterias.Shared/Loterias.Shared.csproj", "Loterias.Shared/"]
COPY ["Loterias.RedisCacheService/Loterias.RedisCacheService.csproj", "Loterias.RedisCacheService/"]

# Restaura os pacotes necessários
RUN dotnet restore "Loterias.QueryApiService/Loterias.QueryApiService.csproj"

# Copia o restante do código-fonte
COPY . .

WORKDIR "/src/Loterias.QueryApiService"

# Publica apenas o projeto da API
RUN dotnet publish "Loterias.QueryApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false


# Etapa 2 - Runtime (única base com curl instalado)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalar curl para healthcheck do Docker
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copia os artefatos publicados
COPY --from=build /app/publish .

# Expõe a porta da aplicação (definida em ASPNETCORE_URLS)
EXPOSE 8080

# Define o ponto de entrada
ENTRYPOINT ["dotnet", "Loterias.QueryApiService.dll"]
