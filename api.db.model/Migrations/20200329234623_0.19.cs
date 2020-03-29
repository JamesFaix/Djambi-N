using Microsoft.EntityFrameworkCore.Migrations;

namespace Apex.Api.Db.Model.Migrations
{
    public partial class _019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NeutralPlayerNameId",
                table: "NeutralPlayerNames",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NeutralPlayerNames",
                table: "NeutralPlayerNames",
                column: "NeutralPlayerNameId");

            migrationBuilder.InsertData(
                table: "NeutralPlayerNames",
                columns: new[] { "NeutralPlayerNameId", "Name" },
                values: new object[,]
                {
                    { 0, "SPORKMASTER" },
                    { 1, "dwight-schrute" },
                    { 2, "1337h4x" },
                    { 3, "DragonBjorn" },
                    { 4, "docta-octagon" },
                    { 5, "Sam_I_Am" },
                    { 6, "New_Boots" },
                    { 7, "mysterious-stranger" },
                    { 8, "Riemann" },
                    { 9, "PT3R0D4C7YL" },
                    { 10, "Rhombicuboctohedron" },
                    { 11, "Schmorpheus" },
                    { 12, "TheMangler" },
                    { 13, "ManBearPig" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NeutralPlayerNames",
                table: "NeutralPlayerNames");

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 0);

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "NeutralPlayerNames",
                keyColumn: "NeutralPlayerNameId",
                keyValue: 13);

            migrationBuilder.DropColumn(
                name: "NeutralPlayerNameId",
                table: "NeutralPlayerNames");
        }
    }
}
