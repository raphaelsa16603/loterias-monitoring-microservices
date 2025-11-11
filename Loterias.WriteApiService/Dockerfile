FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Loterias.WriteApiService/Loterias.WriteApiService.csproj", "Loterias.WriteApiService/"]
COPY ["Loterias.Shared/Loterias.Shared.csproj", "Loterias.Shared/"]
COPY ["Loterias.Messaging/Loterias.Messaging.csproj", "Loterias.Messaging/"]
COPY ["Loterias.Logging.Common/Loterias.Logging.Common.csproj", "Loterias.Logging.Common/"]

RUN dotnet restore "Loterias.WriteApiService/Loterias.WriteApiService.csproj"

COPY . .
WORKDIR "/src/Loterias.WriteApiService"
RUN dotnet publish "Loterias.WriteApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Loterias.WriteApiService.dll"]
