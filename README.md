# VcsDevelop

VcsDevelop - веб-приложение для управления репозиториями/документами в стиле VCS. Проект состоит из ASP.NET Core Web API, React + TypeScript клиентской части и набора инфраструктурных сервисов для PostgreSQL, Redis и MinIO.

## Состав

- `src/VcsDevelop.WebApi` - backend API и хост для SPA.
- `src/VcsDevelop.Client` - frontend на React, TypeScript и Vite.
- `src/VcsDevelop.Application` - application layer с обработчиками команд и запросов.
- `src/VcsDevelop.Domain` - доменные модели и команды.
- `src/VcsDevelop.Infrastructure` - EF Core, репозитории, JWT, Redis, MinIO и сервисы.
- `tests/VcsDevelop.WebApi.UnitTests` - unit-тесты для API и обработчиков.

## Возможности

- регистрация, вход, выход и обновление access token;
- управление профилем пользователя;
- создание репозиториев/документов;
- загрузка файлов и staging;
- коммит, revert, просмотр дерева и blob-объектов;
- OpenAPI/Scalar для API;
- SPA, обслуживаемая через Web API.

## Требования

- .NET SDK 10.0
- Node.js 22+
- Docker и Docker Compose, если нужен полный локальный стек

## Локальный запуск

### 1. Поднять зависимости

Проект использует PostgreSQL, Redis и MinIO.

```bash
docker compose up -d postgres redis minio
```

### 2. Запустить backend

```bash
dotnet run --project src/VcsDevelop.WebApi
```

По умолчанию API доступно по адресу:

- `http://localhost:5050`
- `https://localhost:7031` - если используется HTTPS-профиль

### 3. Запустить frontend отдельно

```bash
cd src/VcsDevelop.Client
npm install
npm run dev -- --host 0.0.0.0
```

Vite по умолчанию использует `http://localhost:5173`.

## Полный запуск через Docker

В корне репозитория есть `docker-compose.yml`, который поднимает:

- PostgreSQL на `5433`
- Redis на `6379`
- MinIO API на `9000`
- MinIO Console на `9001`
- frontend на `5173`
- webapi на `5050`

Запуск:

```bash
docker compose up --build
```

## Конфигурация

Основные настройки лежат в:

- `src/VcsDevelop.WebApi/appsettings.json`
- `src/VcsDevelop.WebApi/appsettings.Development.json`

Ключевые параметры:

- `ConnectionStrings:VCS-X`
- `ConnectionStrings:Redis`
- `Minio:Endpoint`
- `Minio:AccessKey`
- `Minio:SecretKey`
- `Minio:BucketName`
- `JwtToken:*`

## Тесты

```bash
dotnet test
```

## API

После запуска backend доступны:

- Swagger/OpenAPI и Scalar-интерфейс, если они включены в конфигурации окружения;
- контроллеры `api/account` и `api/repos`.

## Примечания

- На старте приложение пытается создать бакет MinIO и применить миграции EF Core.
- В `docker-compose.yml` frontend настроен на режим разработки, backend - на порт `5050`.
