-- +goose Up
CREATE INDEX IF NOT EXISTS idx_refresh_token_user_id ON refresh_token(user_id);
CREATE INDEX IF NOT EXISTS idx_refresh_token_token ON refresh_token(token);

-- +goose Down
DROP INDEX IF EXISTS idx_refresh_token_token;
DROP INDEX IF EXISTS idx_refresh_token_user_id;
