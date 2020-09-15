using Microsoft.EntityFrameworkCore.Migrations;

namespace Sinav.Data.Migrations
{
    public partial class Init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UnionBranches",
                nullable: false,
                defaultValue: false);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
