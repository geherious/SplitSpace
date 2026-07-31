-- +goose Up
CREATE TABLE IF NOT EXISTS "user" (
    id            uuid        PRIMARY KEY,
    email         text        NOT NULL UNIQUE,
    password_hash text        NOT NULL,
    created_at    timestamptz NOT NULL,
    last_login    timestamptz NULL
);

CREATE TABLE IF NOT EXISTS refresh_token (
    id         uuid        PRIMARY KEY,
    user_id    uuid        NOT NULL REFERENCES "user"(id) ON DELETE CASCADE,
    token      text        NOT NULL UNIQUE,
    expires_at timestamptz NOT NULL,
    revoked_at timestamptz NULL,
    created_at timestamptz NOT NULL
);

-- +goose Down
DROP TABLE IF EXISTS refresh_token;
DROP TABLE IF EXISTS "user";
