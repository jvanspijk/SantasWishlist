using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SantasWishlist.Context.Migrations
{
    /// <inheritdoc />
    public partial class updateuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_WishLists_WishListName",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_WishListName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WishListName",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WishListName",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_WishListName",
                table: "AspNetUsers",
                column: "WishListName");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_WishLists_WishListName",
                table: "AspNetUsers",
                column: "WishListName",
                principalTable: "WishLists",
                principalColumn: "Name");
        }
    }
}
