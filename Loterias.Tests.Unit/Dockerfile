# Etapa de build com o SDK .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia todo o código-fonte da solução
COPY . .

# Restaura dependências
RUN dotnet restore

# Publica o projeto de testes unitários
RUN dotnet publish Loterias.Tests.Unit/Loterias.Tests.Unit.csproj -c Release -o /app/publish

# Etapa final: ambiente de execução para testes
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS testrunner
WORKDIR /app

# Copia os arquivos publicados da etapa anterior
COPY --from=build /app/publish .

# Executa os testes unitários diretamente
ENTRYPOINT ["dotnet", "test", "--logger:trx", "--results-directory", "/testresults", "--no-build"]
