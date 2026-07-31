-- +goose Up
ALTER TABLE balance ADD COLUMN IF NOT EXISTS created_by uuid;

CREATE TABLE IF NOT EXISTS balance_entry (
    id          uuid            PRIMARY KEY,
    balance_id  uuid            NOT NULL REFERENCES balance(id) ON DELETE CASCADE,
    total       numeric(18,2)   NOT NULL,
    source_type text            NOT NULL,
    expense_id  uuid,
    CONSTRAINT ux_balance_entry_balance_source UNIQUE (balance_id, source_type, expense_id)
);

CREATE TABLE IF NOT EXISTS debt_entry (
    id          uuid            PRIMARY KEY,
    debt_id     uuid            NOT NULL REFERENCES debt(id) ON DELETE CASCADE,
    owned_by    uuid            NOT NULL,
    owned_to    uuid            NOT NULL,
    total       numeric(18,2)   NOT NULL,
    source_type text            NOT NULL,
    expense_id  uuid,
    split_id    uuid,
    CONSTRAINT ux_debt_entry_source UNIQUE (debt_id, source_type, expense_id, split_id)
);

CREATE INDEX idx_balance_entry_balance_id ON balance_entry (balance_id);
CREATE INDEX idx_debt_entry_debt_id ON debt_entry (debt_id);

DROP TABLE IF EXISTS tag;
DROP TABLE IF EXISTS expense_tag;

-- +goose Down
DROP TABLE IF EXISTS debt_entry;
DROP TABLE IF EXISTS balance_entry;
ALTER TABLE balance DROP COLUMN IF EXISTS created_by;
