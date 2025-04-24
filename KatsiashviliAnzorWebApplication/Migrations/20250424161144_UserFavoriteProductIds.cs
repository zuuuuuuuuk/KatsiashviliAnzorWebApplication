using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KatsiashviliAnzorWebApplication.Migrations
{
    public partial class UserFavoriteProductIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FavoriteProductIds",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FavoriteProductIds",
                table: "Users");
        }
    }
}
