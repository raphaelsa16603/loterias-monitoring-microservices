# 🧩 Loterias Monitoring Microservices (.NET 8)

Sistema distribuído em **microsserviços .NET 8** para **coleta, armazenamento, consulta e monitoramento** de resultados das loterias oficiais da **Caixa Econômica Federal**, com arquitetura moderna baseada em **Kafka, MongoDB, Redis, Graylog e Hangfire**.

---

## 🧱 Estrutura de Microsserviços

| Serviço | Função Principal | Comunicação | Dependências |
|----------|------------------|--------------|---------------|
| **Loterias.JobControlService** | Orquestra e agenda execuções (Hangfire). | HTTP + Kafka | MongoDB, Hangfire |
| **Loterias.CollectorDailyService** | Coleta sorteios diários da Caixa. | HTTP + Kafka | CaixaApi, QueryApi, Redis |
| **Loterias.CollectorHistoricalService** | Atualiza histórico completo. | HTTP + Kafka | CaixaApi, QueryApi |
| **Loterias.CaixaApiService** | API intermediária para dados da Caixa, com cache Redis. | HTTP + Redis | Redis |
| **Loterias.QueryApiService** | API de leitura de sorteios (MongoDB + Redis). | HTTP | MongoDB, Redis |
| **Loterias.WriteApiService** | API de escrita e atualização de sorteios. | HTTP | MongoDB, Redis |
| **Loterias.JobConsumerService** | Consome tópicos Kafka e grava no MongoDB. | Kafka + HTTP | MongoDB, WriteApi |
| **Loterias.LoggingApiCommandService** | Recebe logs de microsserviços e publica no Kafka + Graylog. | HTTP + Kafka | Graylog |
| **Loterias.JobConsumerLoggingService** | Consome logs Kafka e grava no MongoDB. | Kafka | MongoDB |
| **Loterias.LoggingApiQueryService** | Consulta logs técnicos armazenados. | HTTP | MongoDB |
| **Loterias.MonitoringWebApp** | Painel Web (Razor MVC) de monitoramento, logs e jobs. | HTTP | Graylog, Logging APIs |

---

## ⚙️ Fluxo Arquitetural

```
[JobControlService] → agenda execuções (Hangfire)
        │
        ▼
[CollectorDailyService] / [CollectorHistoricalService]
        │
        ▼
[CaixaApiService] → [Kafka Topics]
        │                    │
        ▼                    ▼
[JobConsumerService] → [WriteApiService] → [MongoDB] ↔ [Redis]
        │
        ▼
[QueryApiService] → consultas públicas
        │
        ▼
[MonitoringWebApp] → dashboards + logs + métricas
```

**Logs e Observabilidade:**
```
[Serviços .NET] → [LoggingApiCommandService]
       ├──► Kafka Topic: loterias.logs
       └──► Graylog (GELF HTTP)
            ├──► Elasticsearch (indexação)
            └──► Graylog Web (visualização)
[JobConsumerLoggingService] → MongoDB.LogsApp
[LoggingApiQueryService] → [MonitoringWebApp]
```

---

## 🗄️ Stack de Tecnologias

| Componente | Função |
|-------------|--------|
| **.NET 8 (C#)** | Base de todos os microsserviços |
| **MongoDB 6.0** | Armazenamento principal de sorteios e logs |
| **Redis 7.2** | Cache de resultados recentes |
| **Apache Kafka 3.6** | Mensageria assíncrona e logs distribuídos |
| **Graylog 6.0 + Elasticsearch 8.15** | Central de logs e observabilidade |
| **Hangfire** | Agendador e orquestrador de jobs |
| **Docker Compose 3.9** | Orquestração de containers locais |
| **Kafdrop** | Interface Web para monitorar tópicos Kafka |

---

## 🚀 Execução com Docker Compose

```bash
cd docker/compose
docker-compose up -d
```

A stack completa será iniciada com:
- MongoDB (porta 27017)
- Redis (porta 6379)
- Kafka + Zookeeper + Kafdrop (portas 9092, 2181, 9000)
- Graylog + Elasticsearch (portas 9009, 9200)
- APIs e Services .NET (portas 5000–5012)

Acesse:
- 🌐 **Graylog:** [http://localhost:9009](http://localhost:9009)
- 📊 **Kafdrop:** [http://localhost:9000](http://localhost:9000)
- 🧠 **MonitoringWebApp:** [http://localhost:5012](http://localhost:5012)
- ⚙️ **Hangfire Dashboard:** [http://localhost:5003/hangfire](http://localhost:5003/hangfire)

---

## 🧩 Bibliotecas Internas

- **Loterias.Shared** — DTOs, modelos e enums comuns  
- **Loterias.Messaging** — Abstração genérica para Kafka (producer/consumer)  
- **Loterias.Logging.Common** — Modelo de logs estruturados  
- **Loterias.CaixaClientLib** — Cliente HTTP desacoplado da API da Caixa  

---

## 🧪 Testes Automatizados

```bash
dotnet test Loterias.Tests.Unit
dotnet test Loterias.Tests.Integration
```

Tipos de teste:
- **Unitários:** Validação isolada de métodos e serviços.
- **Integração:** Testes reais com MongoDB, Redis e Kafka (via TestContainers).
- **Contrato:** Validação de schemas JSON de APIs e eventos Kafka.

---

## 🔐 Segurança

- 🔑 Autenticação via `X-API-KEY` para APIs internas.  
- 🔒 Comunicação segura via HTTPS/TLS (em produção).  
- 🧱 Armazenamento de segredos via **User Secrets / KeyVault**.  
- 🪵 Logs estruturados e auditáveis (traceId + correlationId).  
- 🧩 Rate limiting e CORS configuráveis em todas as APIs.

---

## 🧠 Observabilidade

- **Graylog + Elasticsearch:** logs centralizados (loterias.logs)
- **Prometheus + cAdvisor:** métricas de CPU, memória e jobs
- **Alertmanager:** alertas automáticos (Slack / Email)
- **Hangfire Dashboard:** monitoramento de jobs e falhas
- **Kafdrop:** visualização de tópicos e mensagens Kafka

---

## 🧰 Build e CI/CD

```bash
dotnet build LoteriasSolution.sln --configuration Release
```

Pipeline GitHub Actions (incremental):
1. ✅ **00-verify** → valida estrutura
2. 🧩 **01-shared-tests** → testa Shared isoladamente
3. ⚙️ **02-build-apis** → compila todas as APIs
4. 🧪 **03-full-tests** → executa testes unitários e integração
5. 🔍 **04-quality-checks** → análise de código e segurança
6. 🚀 **05-deploy** → build & push Docker (GHCR)

---

## 📦 Estrutura do Repositório

```
LoteriasSolution/
│
├── src/
│   ├── Core/
│   │   ├── Loterias.Shared/
│   │   ├── Loterias.Messaging/
│   │   ├── Loterias.CaixaClientLib/
│   │   └── Loterias.Logging.Common/
│   ├── APIs/
│   │   ├── Loterias.QueryApiService/
│   │   ├── Loterias.WriteApiService/
│   │   ├── Loterias.CaixaApiService/
│   │   ├── Loterias.LoggingApiCommandService/
│   │   └── Loterias.LoggingApiQueryService/
│   ├── Jobs/
│   │   ├── Loterias.CollectorDailyService/
│   │   ├── Loterias.CollectorHistoricalService/
│   │   ├── Loterias.JobConsumerService/
│   │   ├── Loterias.JobConsumerLoggingService/
│   │   └── Loterias.JobControlService/
│   └── Web/
│       └── Loterias.MonitoringWebApp/
│
├── tests/
│   ├── Loterias.Tests.Unit/
│   └── Loterias.Tests.Integration/
│
├── Database/
│   ├── MongoDB/
│   ├── Redis/
│   ├── Kafka/
│   ├── Zookeeper/
│   ├── Elasticsearch/
│   └── Graylog/
│
├── docs/
│   ├── Especificacao_Tecnica.md
│   └── Fluxo_Arquitetura.drawio
│
└── docker-compose.yml
```

---

## 💡 Benefícios da Arquitetura

- 🧩 **Desacoplamento Total** – cada serviço tem responsabilidade única.  
- ⚡ **Alta Escalabilidade** – Kafka e Redis garantem paralelismo.  
- 🧠 **Resiliência Operacional** – mensagens persistentes e reprocessáveis.  
- 💾 **Armazenamento Histórico Unificado** – MongoDB centralizado.  
- 🪵 **Observabilidade Completa** – logs em Graylog + MongoDB.  
- 🕹️ **Automação Total** – jobs diários e históricos via Hangfire.  
- 🐳 **Infraestrutura Reprodutível** – Docker Compose pronto para Dev/QAS/Prod.  

---

**Desenvolvido em .NET 8 com Kafka, MongoDB, Redis, Graylog e Docker.**  
Arquitetura escalável, resiliente e 100% observável — pronta para análise, BI e predição de resultados futuros.
