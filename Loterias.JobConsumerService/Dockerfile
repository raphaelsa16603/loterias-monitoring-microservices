FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Loterias.JobConsumerService/Loterias.JobConsumerService.csproj", "Loterias.JobConsumerService/"]
COPY ["Loterias.Shared/Loterias.Shared.csproj", "Loterias.Shared/"]
COPY ["Loterias.Messaging/Loterias.Messaging.csproj", "Loterias.Messaging/"]
COPY ["Loterias.Logging.Common/Loterias.Logging.Common.csproj", "Loterias.Logging.Common/"]
RUN dotnet restore "Loterias.JobConsumerService/Loterias.JobConsumerService.csproj"
COPY . .
WORKDIR "/src/Loterias.JobConsumerService"
RUN dotnet publish "Loterias.JobConsumerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Loterias.JobConsumerService.dll"]
