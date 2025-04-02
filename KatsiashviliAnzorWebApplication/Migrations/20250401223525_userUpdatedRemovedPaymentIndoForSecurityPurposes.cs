using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KatsiashviliAnzorWebApplication.Migrations
{
    public partial class userUpdatedRemovedPaymentIndoForSecurityPurposes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentCardCode",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentCardCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
