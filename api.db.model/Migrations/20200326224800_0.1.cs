using Microsoft.EntityFrameworkCore.Migrations;

namespace Apex.Api.Db.Model.Migrations
{
    public partial class _01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventKinds",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventKinds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameStatuses",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NeutralPlayerNames",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NeutralPlayerNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerKinds",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerKinds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStatuses",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Privileges",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Privileges", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventKinds");

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
        }
    }
}
