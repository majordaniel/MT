using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTMiddleware.Core.Migrations
{
    public partial class initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomersTransactionTags_CustomerDetails_CustomerDetailsId",
                table: "CustomersTransactionTags");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTransactions_CustomersTransactionTags_CustomersChannelTransKeyId",
                table: "CustomerTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomersTransactionTags",
                table: "CustomersTransactionTags");

            migrationBuilder.RenameTable(
                name: "CustomersTransactionTags",
                newName: "CustomersChannelTransKey");

            migrationBuilder.RenameIndex(
                name: "IX_CustomersTransactionTags_CustomerDetailsId",
                table: "CustomersChannelTransKey",
                newName: "IX_CustomersChannelTransKey_CustomerDetailsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomersChannelTransKey",
                table: "CustomersChannelTransKey",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersChannelTransKey_CustomerDetails_CustomerDetailsId",
                table: "CustomersChannelTransKey",
                column: "CustomerDetailsId",
                principalTable: "CustomerDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTransactions_CustomersChannelTransKey_CustomersChannelTransKeyId",
                table: "CustomerTransactions",
                column: "CustomersChannelTransKeyId",
                principalTable: "CustomersChannelTransKey",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomersChannelTransKey_CustomerDetails_CustomerDetailsId",
                table: "CustomersChannelTransKey");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTransactions_CustomersChannelTransKey_CustomersChannelTransKeyId",
                table: "CustomerTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomersChannelTransKey",
                table: "CustomersChannelTransKey");

            migrationBuilder.RenameTable(
                name: "CustomersChannelTransKey",
                newName: "CustomersTransactionTags");

            migrationBuilder.RenameIndex(
                name: "IX_CustomersChannelTransKey_CustomerDetailsId",
                table: "CustomersTransactionTags",
                newName: "IX_CustomersTransactionTags_CustomerDetailsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomersTransactionTags",
                table: "CustomersTransactionTags",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersTransactionTags_CustomerDetails_CustomerDetailsId",
                table: "CustomersTransactionTags",
                column: "CustomerDetailsId",
                principalTable: "CustomerDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTransactions_CustomersTransactionTags_CustomersChannelTransKeyId",
                table: "CustomerTransactions",
                column: "CustomersChannelTransKeyId",
                principalTable: "CustomersTransactionTags",
                principalColumn: "Id");
        }
    }
}
