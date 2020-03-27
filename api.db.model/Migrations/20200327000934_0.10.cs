using Microsoft.EntityFrameworkCore.Migrations;

namespace Apex.Api.Db.Model.Migrations
{
    public partial class _010 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserPrivileges",
                newName: "UserPrivilegeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Snapshots",
                newName: "SnapshotId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Sessions",
                newName: "SessionId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Privileges",
                newName: "PrivilegeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PlayerStatuses",
                newName: "PlayerStatusId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Players",
                newName: "PlayerId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PlayerKinds",
                newName: "PlayerKindId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "NeutralPlayerNames",
                newName: "NeutralPlayerNameId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "GameStatuses",
                newName: "GameStatusId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Games",
                newName: "GameId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Events",
                newName: "EventId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EventKinds",
                newName: "EventKindId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UserPrivilegeId",
                table: "UserPrivileges",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "SnapshotId",
                table: "Snapshots",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "Sessions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PrivilegeId",
                table: "Privileges",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PlayerStatusId",
                table: "PlayerStatuses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "Players",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PlayerKindId",
                table: "PlayerKinds",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "NeutralPlayerNameId",
                table: "NeutralPlayerNames",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GameStatusId",
                table: "GameStatuses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "Games",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "Events",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EventKindId",
                table: "EventKinds",
                newName: "Id");
        }
    }
}
