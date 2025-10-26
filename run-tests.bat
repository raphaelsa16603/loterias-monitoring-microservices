@echo off
setlocal enabledelayedexpansion

echo ====================================================
echo   🔍 Verificando se o Docker está em execução...
echo ====================================================

docker info >nul 2>&1
if errorlevel 1 (
    echo ⚠️  Docker não está em execução. Tentando iniciar...
    net start com.docker.service >nul 2>&1
    timeout /t 10 >nul

    docker info >nul 2>&1
    if errorlevel 1 (
        echo ❌ Falha ao iniciar o Docker. Inicie manualmente e tente novamente.
        exit /b 1
    )
)

echo ====================================================
echo   🐋 Subindo containers (MongoDB, Redis, API)
echo ====================================================

docker compose -f docker-compose.tests.yml up -d --build

if errorlevel 1 (
    echo ❌ Falha ao iniciar containers.
    exit /b 1
)

echo ====================================================
echo   🧪 Executando testes unitários...
echo ====================================================
dotnet test Loterias.Tests.Unit --no-build --logger:trx --results-directory ./TestResults/Unit

if errorlevel 1 (
    echo ❌ Testes unitários falharam.
) else (
    echo ✅ Testes unitários concluídos com sucesso.
)

echo ====================================================
echo   🧩 Executando testes de integração (via Docker)...
echo ====================================================
docker compose -f docker-compose.tests.yml run --rm tests

if errorlevel 1 (
    echo ❌ Testes de integração falharam.
) else (
    echo ✅ Testes de integração concluídos com sucesso.
)

echo ====================================================
echo   🧹 Limpando containers...
echo ====================================================
docker compose -f docker-compose.tests.yml down -v

echo ✅ Todos os testes finalizados.
exit /b 0
