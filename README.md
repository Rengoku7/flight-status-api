# Flight Status API

# Запуск всего стека через Docker (одной командой)

```bash
docker compose up --build
```

Поднимаются:
1. API — http://localhost:5000 (Swagger: http://localhost:5000/swagger)
2. PostgreSQL — порт 5432 
3. Redis — порт 6379

При старте приложение:
1. Применяет миграции к БД
2. Заполняет БД сидером: роли (Moderator, Reader), пользователи и демо-рейсы
3. Прогревает кэш рейсов для демонстрации работы кэша

### Пароли для входа

- Модератор: логин `moderator`, пароль `Moderator1`.
- Читатель: логин `user`, пароль `Reader1`.

Требования к паролю: минимум 8 символов, хотя бы одна буква и одна цифра.

### Эндпоинты

- GET /health — проверка состояния (БД и др.).
- POST /api/auth/login — вход. Тело: username, password. Ответ: JWT в data.token и data.expiresAt.
- GET /api/flights** — список рейсов с пагинацией. Параметры: origin, destination, page, pageSize (по умолчанию 1 и 20). Сортировка по времени прилёта.
- GET /api/flights/{id}** — рейс по id.
- POST /api/flights** — добавить рейс. Тело: origin, destination, departure, arrival, status (InTime, Delayed, Cancelled). Роль Moderator.
- PATCH /api/flights/{id}/status** — изменить статус рейса. Тело: status. Роль Moderator.

Ответы в формате ApiResult (success, data, errors)

