# Etapa de build (compila os testes)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os csproj para restaurar dependências
COPY ["Loterias.Tests.Integration/Loterias.Tests.Integration.csproj", "Loterias.Tests.Integration/"]
COPY ["Loterias.QueryApiService/Loterias.QueryApiService.csproj", "Loterias.QueryApiService/"]
COPY ["Loterias.Shared/Loterias.Shared.csproj", "Loterias.Shared/"]
COPY ["Loterias.RedisCacheService/Loterias.RedisCacheService.csproj", "Loterias.RedisCacheService/"]

# Restaura pacotes
RUN dotnet restore "Loterias.Tests.Integration/Loterias.Tests.Integration.csproj"

# Copia o restante do código
COPY . .

# Publica os testes
RUN dotnet publish "Loterias.Tests.Integration/Loterias.Tests.Integration.csproj" -c Release -o /app/publish /p:UseAppHost=false


# Etapa final: runner dos testes
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS testrunner
WORKDIR /app

# Copia os artefatos publicados
COPY --from=build /app/publish .

# Variável de ambiente para a URL da API
ENV API_URL=http://loterias-api:8080

# Executa os testes e salva resultado no /testresults
ENTRYPOINT ["dotnet", "test", "--logger:trx;LogFileName=test_results.trx", "--results-directory", "/testresults", "--no-build"]
