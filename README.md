# LoteriasSolution (.NET Microservices)

Sistema de coleta, armazenamento, consulta e publicação de resultados de jogos da Caixa Econômica Federal.

## 🧱 Estrutura de Microsserviços

- `Loterias.CollectorDailyService` - Coleta sorteios do dia.
- `Loterias.CollectorHistoricalService` - Coleta histórica controlada.
- `Loterias.QueryApiService` - API REST para consulta de sorteios.
- `Loterias.RedisCacheService` - Cache distribuído com Redis.
- `Loterias.LoggingService` - Logs centralizados com interface web (Seq).
- `Loterias.JobControlService` - Controle e agendamento de jobs (Hangfire).
- `Loterias.Shared` - Interfaces, modelos, enums e helpers.
- `Loterias.Tests.Unit` / `Loterias.Tests.Integration` - Testes automatizados.

## 🚀 Execução com Docker Compose

```bash
cd docker/compose
docker-compose up -d
```

## 🔧 Dependências Externas

- MongoDB - Armazenamento de sorteios
- Redis - Cache de dados
- Seq - Visualização de logs (http://localhost:5341)

## 🛠️ Build dos Serviços

Use Visual Studio 2022 ou CLI:

```bash
dotnet build LoteriasSolution.sln
```

## 🧪 Executar Testes

```bash
dotnet test Loterias.Tests.Unit
dotnet test Loterias.Tests.Integration
```

## 📅 Agendamentos e Jobs

- Jobs diários e históricos definidos em `CollectorDailyJob` e `CollectorHistoricalJob`
- Integração futura com Hangfire em `JobControlService`

## 🔐 Segurança

Para ambientes reais, utilize:
- Autenticação OAuth2
- Limites de requisição (Rate Limiting)
- Armazenamento seguro de chaves (KeyVault)

---

Desenvolvido com .NET 8, MongoDB, Redis, Serilog, Docker e boas práticas de DDD, Clean Architecture e SOLID.
