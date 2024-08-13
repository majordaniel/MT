using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTMiddleware.Core.Migrations
{
    public partial class updatedmodelparameter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "CustomerTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reference",
                table: "CustomerTransactions");
        }
    }
}
