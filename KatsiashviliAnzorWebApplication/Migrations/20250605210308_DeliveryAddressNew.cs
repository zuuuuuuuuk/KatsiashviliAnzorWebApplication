using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KatsiashviliAnzorWebApplication.Migrations
{
    public partial class DeliveryAddressNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAddress_Users_UserId",
                table: "DeliveryAddress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryAddress",
                table: "DeliveryAddress");

            migrationBuilder.RenameTable(
                name: "DeliveryAddress",
                newName: "DeliveryAddresses");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAddress_UserId",
                table: "DeliveryAddresses",
                newName: "IX_DeliveryAddresses_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryAddresses",
                table: "DeliveryAddresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAddresses_Users_UserId",
                table: "DeliveryAddresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAddresses_Users_UserId",
                table: "DeliveryAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryAddresses",
                table: "DeliveryAddresses");

            migrationBuilder.RenameTable(
                name: "DeliveryAddresses",
                newName: "DeliveryAddress");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAddresses_UserId",
                table: "DeliveryAddress",
                newName: "IX_DeliveryAddress_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryAddress",
                table: "DeliveryAddress",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAddress_Users_UserId",
                table: "DeliveryAddress",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
