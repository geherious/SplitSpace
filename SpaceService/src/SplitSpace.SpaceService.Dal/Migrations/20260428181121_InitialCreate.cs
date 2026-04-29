using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SplitSpace.SpaceService.Dal.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "space",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_space", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "invitation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    space_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invited_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invited_by = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invitation", x => x.id);
                    table.ForeignKey(
                        name: "fk_invitation_space_space_id",
                        column: x => x.space_id,
                        principalTable: "space",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "space_membership",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    space_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    joined_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_space_membership", x => x.id);
                    table.ForeignKey(
                        name: "fk_space_membership_space_space_id",
                        column: x => x.space_id,
                        principalTable: "space",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_invitation_invited_user_id",
                table: "invitation",
                column: "invited_user_id");

            migrationBuilder.CreateIndex(
                name: "idx_invitation_space_id",
                table: "invitation",
                column: "space_id");

            migrationBuilder.CreateIndex(
                name: "idx_invitation_space_user_status",
                table: "invitation",
                columns: new[] { "space_id", "invited_user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "idx_space_membership_space_id",
                table: "space_membership",
                column: "space_id");

            migrationBuilder.CreateIndex(
                name: "idx_space_membership_space_user_unique",
                table: "space_membership",
                columns: new[] { "space_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_space_membership_user_id",
                table: "space_membership",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invitation");

            migrationBuilder.DropTable(
                name: "space_membership");

            migrationBuilder.DropTable(
                name: "space");
        }
    }
}
