-- +goose Up
CREATE TABLE IF NOT EXISTS balance (
    id          uuid            PRIMARY KEY,
    name        text            NOT NULL,
    balance     numeric(18,2)   NOT NULL,
    owner_type  text            NOT NULL,
    owner_id    uuid            NOT NULL
);

CREATE TABLE IF NOT EXISTS category (
    id          uuid            PRIMARY KEY,
    space_id    uuid            NOT NULL,
    name        text            NOT NULL,
    parent_id   uuid            REFERENCES category(id) ON DELETE CASCADE,
    limit       numeric(18,2)
);

CREATE TABLE IF NOT EXISTS debt (
    id            uuid            PRIMARY KEY,
    space_id      uuid            NOT NULL,
    from_user_id  uuid            NOT NULL,
    to_user_id    uuid            NOT NULL,
    amount        numeric(18,2)   NOT NULL,
    CONSTRAINT ux_debt_space_from_to UNIQUE (space_id, from_user_id, to_user_id)
);

CREATE TABLE IF NOT EXISTS settlement (
    id              uuid            PRIMARY KEY,
    from_user_id    uuid            NOT NULL,
    from_balance_id uuid            NOT NULL REFERENCES balance(id) ON DELETE RESTRICT,
    to_user_id      uuid            NOT NULL,
    amount          numeric(18,2)   NOT NULL,
    created_at      timestamptz     NOT NULL
);

CREATE TABLE IF NOT EXISTS expense (
    id          uuid            PRIMARY KEY,
    space_id    uuid            NOT NULL,
    created_by  uuid            NOT NULL,
    category_id uuid            NOT NULL REFERENCES category(id) ON DELETE RESTRICT,
    balance_id  uuid            NOT NULL REFERENCES balance(id) ON DELETE RESTRICT,
    amount      numeric(18,2)   NOT NULL,
    description text            NOT NULL,
    created_at  timestamptz     NOT NULL
);

CREATE TABLE IF NOT EXISTS expense_split (
    id            uuid            PRIMARY KEY,
    expense_id    uuid            NOT NULL REFERENCES expense(id) ON DELETE CASCADE,
    space_id      uuid            NOT NULL,
    user_id       uuid            NOT NULL,
    amount_to_pay numeric(18,2)   NOT NULL,
    CONSTRAINT ux_expense_split_expense_user UNIQUE (expense_id, user_id)
);

CREATE INDEX idx_balance_owner_id ON balance (owner_id);
CREATE INDEX idx_category_parent_id ON category (parent_id);
CREATE INDEX idx_category_space_id ON category (space_id);
CREATE INDEX idx_expense_balance_id ON expense (balance_id);
CREATE INDEX idx_expense_category_id ON expense (category_id);
CREATE INDEX idx_expense_created_by ON expense (created_by);
CREATE INDEX idx_expense_space_id ON expense (space_id);
CREATE INDEX idx_expense_split_space_id ON expense_split (space_id);
CREATE INDEX idx_expense_split_user_id ON expense_split (user_id);
CREATE INDEX ix_settlement_from_balance_id ON settlement (from_balance_id);

-- +goose Down
DROP TABLE IF EXISTS expense_split;
DROP TABLE IF EXISTS expense;
DROP TABLE IF EXISTS settlement;
DROP TABLE IF EXISTS debt;
DROP TABLE IF EXISTS category;
DROP TABLE IF EXISTS balance;
