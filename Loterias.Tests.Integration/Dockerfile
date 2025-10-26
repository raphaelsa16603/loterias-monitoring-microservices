# Etapa base com o SDK .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia tudo e restaura dependências
COPY . .
RUN dotnet restore

# Publica os testes
RUN dotnet publish Loterias.Tests.Integration/Loterias.Tests.Integration.csproj -c Release -o /app/publish

# Etapa final para execução dos testes
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS testrunner
WORKDIR /app
COPY --from=build /app/publish .

# Comando direto para rodar os testes
ENTRYPOINT ["dotnet", "test", "--logger:trx", "--results-directory", "/testresults", "--no-build"]
