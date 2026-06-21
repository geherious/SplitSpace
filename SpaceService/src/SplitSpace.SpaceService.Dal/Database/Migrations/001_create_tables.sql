-- +goose Up
CREATE TABLE IF NOT EXISTS space (
    id          uuid        PRIMARY KEY,
    name        text        NOT NULL,
    type        text        NOT NULL,
    owner_id    uuid        NOT NULL,
    created_at  timestamptz NOT NULL
);

CREATE TABLE IF NOT EXISTS space_membership (
    id          uuid        PRIMARY KEY,
    space_id    uuid        NOT NULL REFERENCES space(id) ON DELETE CASCADE,
    user_id     uuid        NOT NULL,
    role        text        NOT NULL,
    joined_at   timestamptz NOT NULL
);

CREATE TABLE IF NOT EXISTS invitation (
    id              uuid        PRIMARY KEY,
    space_id        uuid        NOT NULL REFERENCES space(id) ON DELETE CASCADE,
    invited_user_id uuid        NOT NULL,
    invited_by      uuid        NOT NULL,
    status          text        NOT NULL,
    expires_at      timestamptz NOT NULL,
    created_at      timestamptz NOT NULL
);

-- +goose Down
DROP TABLE IF EXISTS invitation;
DROP TABLE IF EXISTS space_membership;
DROP TABLE IF EXISTS space;
