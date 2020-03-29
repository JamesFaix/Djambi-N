using Microsoft.EntityFrameworkCore.Migrations;

namespace Apex.Api.Db.Model.Migrations
{
    public partial class _017 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventKinds_EventKindId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameStatuses_GameStatusId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_PlayerKinds_PlayerKindId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_PlayerStatuses_PlayerStatusId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPrivileges_Privileges_PrivilegeId",
                table: "UserPrivileges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPrivileges_Users_UserId",
                table: "UserPrivileges");

            migrationBuilder.DropIndex(
                name: "IX_UserPrivileges_PrivilegeId",
                table: "UserPrivileges");

            migrationBuilder.DropIndex(
                name: "IX_UserPrivileges_UserId",
                table: "UserPrivileges");

            migrationBuilder.DropIndex(
                name: "IX_Players_PlayerKindId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_PlayerStatusId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Games_GameStatusId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Events_EventKindId",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "UserSqlModelUserId",
                table: "UserPrivileges",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EventKinds",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.InsertData(
                table: "EventKinds",
                columns: new[] { "EventKindId", "Name" },
                values: new object[,]
                {
                    { (byte)1, "GameParametersChanged" },
                    { (byte)2, "GameCanceled" },
                    { (byte)3, "PlayerJoined" },
                    { (byte)4, "PlayerRemoved" },
                    { (byte)5, "GameStarted" },
                    { (byte)6, "TurnCommitted" },
                    { (byte)7, "TurnReset" },
                    { (byte)8, "CellSelected" },
                    { (byte)9, "PlayerStatusChanged" }
                });

            migrationBuilder.InsertData(
                table: "GameStatuses",
                columns: new[] { "GameStatusId", "Name" },
                values: new object[,]
                {
                    { (byte)1, "Pending" },
                    { (byte)2, "InProgress" },
                    { (byte)3, "Canceled" },
                    { (byte)4, "Over" }
                });

            migrationBuilder.InsertData(
                table: "PlayerKinds",
                columns: new[] { "PlayerKindId", "Name" },
                values: new object[,]
                {
                    { (byte)3, "Neutral" },
                    { (byte)1, "User" },
                    { (byte)2, "Guest" }
                });

            migrationBuilder.InsertData(
                table: "PlayerStatuses",
                columns: new[] { "PlayerStatusId", "Name" },
                values: new object[,]
                {
                    { (byte)1, "Pending" },
                    { (byte)2, "Alive" },
                    { (byte)3, "Eliminated" },
                    { (byte)4, "Conceded" },
                    { (byte)5, "WillConcede" },
                    { (byte)6, "AcceptsDraw" },
                    { (byte)7, "Victorious" }
                });

            migrationBuilder.InsertData(
                table: "Privileges",
                columns: new[] { "PrivilegeId", "Name" },
                values: new object[,]
                {
                    { (byte)4, "ViewGames" },
                    { (byte)1, "EditUsers" },
                    { (byte)2, "EditPendingGames" },
                    { (byte)3, "OpenParticipation" },
                    { (byte)5, "Snapshots" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivileges_UserSqlModelUserId",
                table: "UserPrivileges",
                column: "UserSqlModelUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPrivileges_Users_UserSqlModelUserId",
                table: "UserPrivileges",
                column: "UserSqlModelUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPrivileges_Users_UserSqlModelUserId",
                table: "UserPrivileges");

            migrationBuilder.DropIndex(
                name: "IX_UserPrivileges_UserSqlModelUserId",
                table: "UserPrivileges");

            migrationBuilder.DeleteData(
                table: "EventKinds",
                keyColumn: "EventKindId",
                keyValue: (byte)1);

            migrationBuilder.DeleteData(
                table: "EventKinds",
                keyColumn: "EventKindId",
                keyValue: (byte)2);

            migrationBuilder.DeleteData(
                table: "EventKinds",
                keyColumn: "EventKindId",
                keyValue: (byte)3);

            migrationBuilder.DeleteData(
                table: "EventKinds",
                keyColumn: "EventKindId",
                keyValue: (byte)4);

            migrationBuilder.DeleteData(
                table: "EventKinds",
                keyColumn: "EventKindId",
                keyValue: (byte)5);

            migrationBuilder.DeleteData(
                table: "EventKinds",
                keyColumn: "EventKindId",
                keyValue: (byte)6);

            migrationBuilder.DeleteData(
                table: "EventKinds",
                keyColumn: "EventKindId",
                keyValue: (byte)7);

            migrationBuilder.DeleteData(
                table: "EventKinds",
                keyColumn: "EventKindId",
                keyValue: (byte)8);

            migrationBuilder.DeleteData(
                table: "EventKinds",
                keyColumn: "EventKindId",
                keyValue: (byte)9);

            migrationBuilder.DeleteData(
                table: "GameStatuses",
                keyColumn: "GameStatusId",
                keyValue: (byte)1);

            migrationBuilder.DeleteData(
                table: "GameStatuses",
                keyColumn: "GameStatusId",
                keyValue: (byte)2);

            migrationBuilder.DeleteData(
                table: "GameStatuses",
                keyColumn: "GameStatusId",
                keyValue: (byte)3);

            migrationBuilder.DeleteData(
                table: "GameStatuses",
                keyColumn: "GameStatusId",
                keyValue: (byte)4);

            migrationBuilder.DeleteData(
                table: "PlayerKinds",
                keyColumn: "PlayerKindId",
                keyValue: (byte)1);

            migrationBuilder.DeleteData(
                table: "PlayerKinds",
                keyColumn: "PlayerKindId",
                keyValue: (byte)2);

            migrationBuilder.DeleteData(
                table: "PlayerKinds",
                keyColumn: "PlayerKindId",
                keyValue: (byte)3);

            migrationBuilder.DeleteData(
                table: "PlayerStatuses",
                keyColumn: "PlayerStatusId",
                keyValue: (byte)1);

            migrationBuilder.DeleteData(
                table: "PlayerStatuses",
                keyColumn: "PlayerStatusId",
                keyValue: (byte)2);

            migrationBuilder.DeleteData(
                table: "PlayerStatuses",
                keyColumn: "PlayerStatusId",
                keyValue: (byte)3);

            migrationBuilder.DeleteData(
                table: "PlayerStatuses",
                keyColumn: "PlayerStatusId",
                keyValue: (byte)4);

            migrationBuilder.DeleteData(
                table: "PlayerStatuses",
                keyColumn: "PlayerStatusId",
                keyValue: (byte)5);

            migrationBuilder.DeleteData(
                table: "PlayerStatuses",
                keyColumn: "PlayerStatusId",
                keyValue: (byte)6);

            migrationBuilder.DeleteData(
                table: "PlayerStatuses",
                keyColumn: "PlayerStatusId",
                keyValue: (byte)7);

            migrationBuilder.DeleteData(
                table: "Privileges",
                keyColumn: "PrivilegeId",
                keyValue: (byte)1);

            migrationBuilder.DeleteData(
                table: "Privileges",
                keyColumn: "PrivilegeId",
                keyValue: (byte)2);

            migrationBuilder.DeleteData(
                table: "Privileges",
                keyColumn: "PrivilegeId",
                keyValue: (byte)3);

            migrationBuilder.DeleteData(
                table: "Privileges",
                keyColumn: "PrivilegeId",
                keyValue: (byte)4);

            migrationBuilder.DeleteData(
                table: "Privileges",
                keyColumn: "PrivilegeId",
                keyValue: (byte)5);

            migrationBuilder.DropColumn(
                name: "UserSqlModelUserId",
                table: "UserPrivileges");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EventKinds",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivileges_PrivilegeId",
                table: "UserPrivileges",
                column: "PrivilegeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivileges_UserId",
                table: "UserPrivileges",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlayerKindId",
                table: "Players",
                column: "PlayerKindId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlayerStatusId",
                table: "Players",
                column: "PlayerStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameStatusId",
                table: "Games",
                column: "GameStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventKindId",
                table: "Events",
                column: "EventKindId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventKinds_EventKindId",
                table: "Events",
                column: "EventKindId",
                principalTable: "EventKinds",
                principalColumn: "EventKindId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameStatuses_GameStatusId",
                table: "Games",
                column: "GameStatusId",
                principalTable: "GameStatuses",
                principalColumn: "GameStatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_PlayerKinds_PlayerKindId",
                table: "Players",
                column: "PlayerKindId",
                principalTable: "PlayerKinds",
                principalColumn: "PlayerKindId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_PlayerStatuses_PlayerStatusId",
                table: "Players",
                column: "PlayerStatusId",
                principalTable: "PlayerStatuses",
                principalColumn: "PlayerStatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPrivileges_Privileges_PrivilegeId",
                table: "UserPrivileges",
                column: "PrivilegeId",
                principalTable: "Privileges",
                principalColumn: "PrivilegeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPrivileges_Users_UserId",
                table: "UserPrivileges",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
