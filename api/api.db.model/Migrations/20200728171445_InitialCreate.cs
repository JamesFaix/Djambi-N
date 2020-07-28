using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Apex.Api.Db.Model.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventKinds",
                columns: table => new
                {
                    EventKindId = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventKinds", x => x.EventKindId);
                });

            migrationBuilder.CreateTable(
                name: "GameStatuses",
                columns: table => new
                {
                    GameStatusId = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStatuses", x => x.GameStatusId);
                });

            migrationBuilder.CreateTable(
                name: "NeutralPlayerNames",
                columns: table => new
                {
                    NeutralPlayerNameId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NeutralPlayerNames", x => x.NeutralPlayerNameId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerKinds",
                columns: table => new
                {
                    PlayerKindId = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerKinds", x => x.PlayerKindId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStatuses",
                columns: table => new
                {
                    PlayerStatusId = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStatuses", x => x.PlayerStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Privileges",
                columns: table => new
                {
                    PrivilegeId = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Privileges", x => x.PrivilegeId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    Password = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    FailedLoginAttempts = table.Column<byte>(nullable: false),
                    LastFailedLoginAttemptOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.UniqueConstraint("AK_Users_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedByUserId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    GameStatusId = table.Column<byte>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    RegionCount = table.Column<byte>(nullable: false),
                    AllowGuests = table.Column<bool>(nullable: false),
                    IsPublic = table.Column<bool>(nullable: false),
                    TurnCycleJson = table.Column<string>(nullable: true),
                    PiecesJson = table.Column<string>(nullable: true),
                    CurrentTurnJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_Games_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    SessionId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Token = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ExpiresOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPrivileges",
                columns: table => new
                {
                    UserPrivilegeId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    PrivilegeId = table.Column<byte>(nullable: false),
                    UserSqlModelUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrivileges", x => x.UserPrivilegeId);
                    table.ForeignKey(
                        name: "FK_UserPrivileges_Users_UserSqlModelUserId",
                        column: x => x.UserSqlModelUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    PlayerKindId = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    PlayerStatusId = table.Column<byte>(nullable: false),
                    ColorId = table.Column<byte>(nullable: true),
                    StartingRegion = table.Column<byte>(nullable: true),
                    StartingTurnNumber = table.Column<byte>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                    table.UniqueConstraint("AK_Players_GameId_Name", x => new { x.GameId, x.Name });
                    table.ForeignKey(
                        name: "FK_Players_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Players_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Snapshots",
                columns: table => new
                {
                    SnapshotId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameId = table.Column<int>(nullable: false),
                    CreatedByUserId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    SnapshotJson = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snapshots", x => x.SnapshotId);
                    table.ForeignKey(
                        name: "FK_Snapshots_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Snapshots_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GameId = table.Column<int>(nullable: false),
                    CreatedByUserId = table.Column<int>(nullable: false),
                    ActingPlayerId = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    EventKindId = table.Column<byte>(nullable: false),
                    EffectsJson = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Events_Players_ActingPlayerId",
                        column: x => x.ActingPlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    { (byte)4, "Over" },
                    { (byte)3, "Canceled" },
                    { (byte)1, "Pending" },
                    { (byte)2, "InProgress" }
                });

            migrationBuilder.InsertData(
                table: "NeutralPlayerNames",
                columns: new[] { "NeutralPlayerNameId", "Name" },
                values: new object[,]
                {
                    { 9, "PT3R0D4C7YL" },
                    { 13, "ManBearPig" },
                    { 12, "TheMangler" },
                    { 11, "Schmorpheus" },
                    { 10, "Rhombicuboctohedron" },
                    { 8, "Riemann" },
                    { 7, "mysterious-stranger" },
                    { 5, "Sam_I_Am" },
                    { 4, "docta-octagon" },
                    { 3, "DragonBjorn" },
                    { 2, "1337h4x" },
                    { 1, "dwight-schrute" },
                    { 0, "SPORKMASTER" },
                    { 6, "New_Boots" }
                });

            migrationBuilder.InsertData(
                table: "PlayerKinds",
                columns: new[] { "PlayerKindId", "Name" },
                values: new object[,]
                {
                    { (byte)2, "Guest" },
                    { (byte)1, "User" },
                    { (byte)3, "Neutral" }
                });

            migrationBuilder.InsertData(
                table: "PlayerStatuses",
                columns: new[] { "PlayerStatusId", "Name" },
                values: new object[,]
                {
                    { (byte)6, "AcceptsDraw" },
                    { (byte)5, "WillConcede" },
                    { (byte)4, "Conceded" },
                    { (byte)1, "Pending" },
                    { (byte)2, "Alive" },
                    { (byte)7, "Victorious" },
                    { (byte)3, "Eliminated" }
                });

            migrationBuilder.InsertData(
                table: "Privileges",
                columns: new[] { "PrivilegeId", "Name" },
                values: new object[,]
                {
                    { (byte)3, "OpenParticipation" },
                    { (byte)4, "ViewGames" },
                    { (byte)1, "EditUsers" },
                    { (byte)2, "EditPendingGames" },
                    { (byte)5, "Snapshots" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_ActingPlayerId",
                table: "Events",
                column: "ActingPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatedByUserId",
                table: "Events",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_GameId",
                table: "Events",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_CreatedByUserId",
                table: "Games",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId",
                table: "Players",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Snapshots_CreatedByUserId",
                table: "Snapshots",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Snapshots_GameId",
                table: "Snapshots",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivileges_UserSqlModelUserId",
                table: "UserPrivileges",
                column: "UserSqlModelUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventKinds");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "GameStatuses");

            migrationBuilder.DropTable(
                name: "NeutralPlayerNames");

            migrationBuilder.DropTable(
                name: "PlayerKinds");

            migrationBuilder.DropTable(
                name: "PlayerStatuses");

            migrationBuilder.DropTable(
                name: "Privileges");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Snapshots");

            migrationBuilder.DropTable(
                name: "UserPrivileges");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
