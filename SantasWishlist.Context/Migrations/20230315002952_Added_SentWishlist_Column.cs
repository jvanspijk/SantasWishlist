using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SantasWishlist.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddedSentWishlistColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SentWishlist",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SentWishlist",
                table: "AspNetUsers");
        }
    }
}
