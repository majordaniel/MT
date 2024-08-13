using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTMiddleware.Core.Migrations
{
    public partial class addedMoreEnums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "customerId",
                table: "CustomerAccounts",
                newName: "CifId");

            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                table: "CustomerTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "CustomerTransactions");

            migrationBuilder.RenameColumn(
                name: "CifId",
                table: "CustomerAccounts",
                newName: "customerId");
        }
    }
}
