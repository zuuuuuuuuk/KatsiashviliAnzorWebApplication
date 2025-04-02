using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KatsiashviliAnzorWebApplication.Migrations
{
    public partial class AddedPaymentInfoToOrderModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentInfo",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentInfo",
                table: "Orders");
        }
    }
}
