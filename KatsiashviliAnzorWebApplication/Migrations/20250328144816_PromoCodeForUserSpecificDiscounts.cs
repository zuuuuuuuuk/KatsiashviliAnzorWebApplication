using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KatsiashviliAnzorWebApplication.Migrations
{
    public partial class PromoCodeForUserSpecificDiscounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PromoCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromoCode",
                table: "Orders");
        }
    }
}
