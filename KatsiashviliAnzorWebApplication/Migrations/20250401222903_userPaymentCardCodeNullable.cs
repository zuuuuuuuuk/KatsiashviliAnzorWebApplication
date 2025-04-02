using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KatsiashviliAnzorWebApplication.Migrations
{
    public partial class userPaymentCardCodeNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentCardCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentCardCode",
                table: "Users");
        }
    }
}
