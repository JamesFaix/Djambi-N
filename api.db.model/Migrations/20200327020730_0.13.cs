using Microsoft.EntityFrameworkCore.Migrations;

namespace Apex.Api.Db.Model.Migrations
{
    public partial class _013 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventKinds_KindId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameStatuses_StatusId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_PlayerKinds_KindId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_PlayerStatuses_StatusId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_KindId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_StatusId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Games_StatusId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Events_KindId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "KindId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "KindId",
                table: "Events");

            migrationBuilder.AddColumn<byte>(
                name: "PlayerKindId",
                table: "Players",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "PlayerStatusId",
                table: "Players",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "GameStatusId",
                table: "Games",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "EventKindId",
                table: "Events",
                nullable: false,
                defaultValue: (byte)0);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "PlayerKindId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PlayerStatusId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GameStatusId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "EventKindId",
                table: "Events");

            migrationBuilder.AddColumn<byte>(
                name: "KindId",
                table: "Players",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "StatusId",
                table: "Players",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "StatusId",
                table: "Games",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "KindId",
                table: "Events",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Players_KindId",
                table: "Players",
                column: "KindId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_StatusId",
                table: "Players",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_StatusId",
                table: "Games",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_KindId",
                table: "Events",
                column: "KindId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventKinds_KindId",
                table: "Events",
                column: "KindId",
                principalTable: "EventKinds",
                principalColumn: "EventKindId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameStatuses_StatusId",
                table: "Games",
                column: "StatusId",
                principalTable: "GameStatuses",
                principalColumn: "GameStatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_PlayerKinds_KindId",
                table: "Players",
                column: "KindId",
                principalTable: "PlayerKinds",
                principalColumn: "PlayerKindId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_PlayerStatuses_StatusId",
                table: "Players",
                column: "StatusId",
                principalTable: "PlayerStatuses",
                principalColumn: "PlayerStatusId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
