using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebShop.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCustomerAddressRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_CustomerAddress_AddressCustomerAddressId",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerAddress",
                table: "CustomerAddress");

            migrationBuilder.RenameTable(
                name: "CustomerAddress",
                newName: "CustomerAddresses");

            migrationBuilder.RenameColumn(
                name: "AddressCustomerAddressId",
                table: "Customers",
                newName: "AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_AddressCustomerAddressId",
                table: "Customers",
                newName: "IX_Customers_AddressId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerAddresses",
                table: "CustomerAddresses",
                column: "CustomerAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_CustomerAddresses_AddressId",
                table: "Customers",
                column: "AddressId",
                principalTable: "CustomerAddresses",
                principalColumn: "CustomerAddressId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_CustomerAddresses_AddressId",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerAddresses",
                table: "CustomerAddresses");

            migrationBuilder.RenameTable(
                name: "CustomerAddresses",
                newName: "CustomerAddress");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "Customers",
                newName: "AddressCustomerAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_AddressId",
                table: "Customers",
                newName: "IX_Customers_AddressCustomerAddressId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerAddress",
                table: "CustomerAddress",
                column: "CustomerAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_CustomerAddress_AddressCustomerAddressId",
                table: "Customers",
                column: "AddressCustomerAddressId",
                principalTable: "CustomerAddress",
                principalColumn: "CustomerAddressId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
