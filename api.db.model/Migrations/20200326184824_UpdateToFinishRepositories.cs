using Microsoft.EntityFrameworkCore.Migrations;

namespace Apex.Api.Db.Model.Migrations
{
    public partial class UpdateToFinishRepositories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Id",
                table: "GameStatuses",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<byte>(
                name: "StatusId",
                table: "Games",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "KindId",
                table: "Events",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "Id",
                table: "EventKinds",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "GameStatuses",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte))
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "Games",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte));

            migrationBuilder.AlterColumn<int>(
                name: "KindId",
                table: "Events",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "EventKinds",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte))
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
