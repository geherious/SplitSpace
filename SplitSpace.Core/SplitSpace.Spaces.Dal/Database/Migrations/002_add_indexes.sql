-- +goose Up
CREATE INDEX IF NOT EXISTS idx_space_membership_space_id ON space_membership(space_id);
CREATE INDEX IF NOT EXISTS idx_space_membership_user_id ON space_membership(user_id);
CREATE INDEX IF NOT EXISTS idx_invitation_space_id ON invitation(space_id);
CREATE INDEX IF NOT EXISTS idx_invitation_invited_user_id ON invitation(invited_user_id);
CREATE INDEX IF NOT EXISTS idx_invitation_status ON invitation(status);

-- +goose Down
DROP INDEX IF EXISTS idx_invitation_status;
DROP INDEX IF EXISTS idx_invitation_invited_user_id;
DROP INDEX IF EXISTS idx_invitation_space_id;
DROP INDEX IF EXISTS idx_space_membership_user_id;
DROP INDEX IF EXISTS idx_space_membership_space_id;
