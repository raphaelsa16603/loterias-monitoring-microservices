# ---------- BASE ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas os csproj para cachear o restore
COPY ["Loterias.CollectorDailyService/Loterias.CollectorDailyService.csproj", "Loterias.CollectorDailyService/"]
COPY ["Loterias.Shared/Loterias.Shared.csproj", "Loterias.Shared/"]
COPY ["Loterias.Logging.Common/Loterias.Logging.Common.csproj", "Loterias.Logging.Common/"]
COPY ["Loterias.Messaging/Loterias.Messaging.csproj", "Loterias.Messaging/"]
COPY ["Loterias.CaixaApiService/Loterias.CaixaApiService.csproj", "Loterias.CaixaApiService/"]
COPY ["Loterias.CaixaClientLib/Loterias.CaixaClientLib.csproj", "Loterias.CaixaClientLib/"]

RUN dotnet restore "Loterias.CollectorDailyService/Loterias.CollectorDailyService.csproj"

# Copia o código-fonte dos projetos necessários
COPY Loterias.CollectorDailyService/. ./Loterias.CollectorDailyService/
COPY Loterias.Shared/. ./Loterias.Shared/
COPY Loterias.Logging.Common/. ./Loterias.Logging.Common/
COPY Loterias.Messaging/. ./Loterias.Messaging/
COPY Loterias.CaixaApiService/. ./Loterias.CaixaApiService/
COPY Loterias.CaixaClientLib/. ./Loterias.CaixaClientLib/

# Compila e publica
WORKDIR "/src/Loterias.CollectorDailyService"
RUN dotnet publish "Loterias.CollectorDailyService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------- FINAL ----------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Loterias.CollectorDailyService.dll"]

