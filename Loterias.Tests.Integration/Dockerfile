# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os projetos necessários para restaurar dependências
COPY ["Loterias.Tests.Integration/Loterias.Tests.Integration.csproj", "Loterias.Tests.Integration/"]
COPY ["Loterias.QueryApiService/Loterias.QueryApiService.csproj", "Loterias.QueryApiService/"]
COPY ["Loterias.Shared/Loterias.Shared.csproj", "Loterias.Shared/"]
COPY ["Loterias.RedisCacheService/Loterias.RedisCacheService.csproj", "Loterias.RedisCacheService/"]

# Restaura os pacotes
RUN dotnet restore "Loterias.Tests.Integration/Loterias.Tests.Integration.csproj"

# Copia o código completo
COPY . .

# Publica os testes já compilados (sem precisar compilar na hora do run)
RUN dotnet publish "Loterias.Tests.Integration/Loterias.Tests.Integration.csproj" -c Release -o /app/publish /p:UseAppHost=false


# Etapa final: runner dos testes
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS testrunner
WORKDIR /app

# Copia os testes publicados
COPY --from=build /app/publish .

# Define a URL da API dentro da rede do Compose
ENV API_URL=http://loterias-api:8080

# Executa o assembly de testes (sem precisar de .csproj)
ENTRYPOINT ["dotnet", "vstest", "Loterias.Tests.Integration.dll", "--ResultsDirectory:/testresults", "--logger:trx;LogFileName=test_results.trx"]
