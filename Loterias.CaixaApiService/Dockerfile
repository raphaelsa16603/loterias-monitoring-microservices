FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Loterias.CaixaApiService/Loterias.CaixaApiService.csproj", "Loterias.CaixaApiService/"]
COPY ["Loterias.Shared/Loterias.Shared.csproj", "Loterias.Shared/"]
COPY ["Loterias.CaixaClientLib/Loterias.CaixaClientLib.csproj", "Loterias.CaixaClientLib/"]
COPY ["Loterias.Logging.Common/Loterias.Logging.Common.csproj", "Loterias.Logging.Common/"]
RUN dotnet restore "Loterias.CaixaApiService/Loterias.CaixaApiService.csproj"

COPY . .
WORKDIR "/src/Loterias.CaixaApiService"
RUN dotnet publish "Loterias.CaixaApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Loterias.CaixaApiService.dll"]
