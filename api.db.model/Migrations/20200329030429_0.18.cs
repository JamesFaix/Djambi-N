using Microsoft.EntityFrameworkCore.Migrations;

namespace Apex.Api.Db.Model.Migrations
{
    public partial class _018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NeutralPlayerNames",
                table: "NeutralPlayerNames");

            migrationBuilder.DropColumn(
                name: "NeutralPlayerNameId",
                table: "NeutralPlayerNames");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NeutralPlayerNameId",
                table: "NeutralPlayerNames",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NeutralPlayerNames",
                table: "NeutralPlayerNames",
                column: "NeutralPlayerNameId");
        }
    }
}
