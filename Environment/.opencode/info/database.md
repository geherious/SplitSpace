# SplitSpace — Database Structure (Grouped by Microservices)

## 📌 Общий принцип
База данных разделена логически по микросервисам. Каждый сервис владеет собственной схемой данных и не использует внешние ключи между сервисами. Связи реализуются через бизнес-логику и события.

---

# 🔐 Auth Service

## user
- id (uuid) — PK, идентификатор пользователя
- email (string) — уникальный email
- password_hash (string) — хеш пароля
- created_at (datetime) — дата регистрации
- last_login (datetime) — последний вход

## refresh_token
- id (uuid) — PK
- user_id (uuid) — идентификатор пользователя
- token (string) — refresh token
- expires_at (datetime) — срок действия
- created_at (datetime) — дата создания

---

# 🏠 Space Service

## space
- id (uuid) — PK
- name (string) — название пространства
- type (string) — personal / group
- owner_id (uuid) — владелец
- created_at (datetime) — дата создания

## space_membership
- id (uuid) — PK
- space_id (uuid) — пространство
- user_id (uuid) — пользователь
- role (string) — роль (owner/member)
- joined_at (datetime) — дата вступления

## invitation
- id (uuid) — PK
- space_id (uuid) — пространство
- invited_user_id (uuid) — приглашённый пользователь
- invited_by (uuid) — кто пригласил
- status (string) — pending/accepted/rejected
- expires_at (datetime) — срок действия
- created_at (datetime) — дата создания

---

# 💰 Finance Service

## account
- id (uuid) — PK
- name (string) — название счёта
- balance (decimal) — баланс
- owner_type (string) — user / space
- owner_id (uuid) — владелец

## expense
- id (uuid) — PK
- space_id (uuid) — пространство
- created_by (uuid) — автор расхода
- category_id (uuid) — категория
- account_id (uuid) — счёт списания
- amount (decimal) — сумма
- description (string) — описание
- created_at (datetime) — дата создания

## expense_split
- id (uuid) — PK
- expense_id (uuid) — расход
- space_id (uuid) — пространство
- user_id (uuid) — участник
- amount_to_pay (decimal) — сумма к оплате
- paid_amount (decimal) — оплачено
- pay_account (uuid) — счёт оплаты
- paid_at (datetime) — дата оплаты

## debt
- id (uuid) — PK
- space_id (uuid) — пространство
- from_user_id (uuid) — должник
- to_user_id (uuid) — кредитор
- amount (decimal) — сумма долга

## category
- id (uuid) — PK
- space_id (uuid) — пространство
- name (string) — название
- parent_id (uuid) — родительская категория
- limit (decimal) — лимит расходов

## tag
- id (uuid) — PK
- space_id (uuid) — пространство
- name (string) — название

## expense_tag
- id (uuid) — PK
- expense_id (uuid) — расход
- tag_id (uuid) — тег

---

# 📌 Общие принципы
- UUID используется как основной тип идентификаторов
- отсутствуют внешние ключи между сервисами
- целостность данных обеспечивается бизнес-логикой
- межсервисное взаимодействие через Kafka и gRPC
