using Microsoft.EntityFrameworkCore.Migrations;

namespace ActivityCenter.Migrations
{
    public partial class SM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Happenings",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Title",
                table: "Happenings",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
