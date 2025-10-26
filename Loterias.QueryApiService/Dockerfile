# Etapa 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas o arquivo de projeto e dependências necessárias
COPY ["Loterias.QueryApiService/Loterias.QueryApiService.csproj", "Loterias.QueryApiService/"]
COPY ["Loterias.Shared/Loterias.Shared.csproj", "Loterias.Shared/"]
COPY ["Loterias.RedisCacheService/Loterias.RedisCacheService.csproj", "Loterias.RedisCacheService/"]

# Restaura os pacotes
RUN dotnet restore "Loterias.QueryApiService/Loterias.QueryApiService.csproj"

# Copia o restante do código
COPY . .

WORKDIR "/src/Loterias.QueryApiService"

# Publica apenas o projeto da API (não a solução inteira)
RUN dotnet publish "Loterias.QueryApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false


# Etapa 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Loterias.QueryApiService.dll"]
