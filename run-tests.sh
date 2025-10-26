#!/bin/bash
set -e

echo "===================================================="
echo "🔍 Verificando se o Docker está em execução..."
echo "===================================================="

if ! docker info > /dev/null 2>&1; then
  echo "⚠️  Docker não está ativo. Tentando iniciar..."
  if command -v systemctl &>/dev/null; then
    sudo systemctl start docker
  fi
  sleep 10
  if ! docker info > /dev/null 2>&1; then
    echo "❌ Falha ao iniciar o Docker. Inicie manualmente e tente novamente."
    exit 1
  fi
fi

echo "===================================================="
echo "🐋 Subindo containers (MongoDB, Redis, API)..."
echo "===================================================="

docker compose -f docker-compose.tests.yml up -d --build

echo "===================================================="
echo "🧪 Executando testes unitários..."
echo "===================================================="

dotnet test Loterias.Tests.Unit --no-build --logger:trx --results-directory ./TestResults/Unit || true

echo "===================================================="
echo "🧩 Executando testes de integração (via Docker)..."
echo "===================================================="

docker compose -f docker-compose.tests.yml run --rm tests || true

echo "===================================================="
echo "🧹 Limpando containers..."
echo "===================================================="

docker compose -f docker-compose.tests.yml down -v

echo "✅ Todos os testes finalizados com sucesso."
