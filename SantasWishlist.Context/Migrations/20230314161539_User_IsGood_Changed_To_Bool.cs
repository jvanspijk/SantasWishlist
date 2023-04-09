using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SantasWishlist.Context.Migrations
{
    /// <inheritdoc />
    public partial class UserIsGoodChangedToBool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoodType",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsGood",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGood",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "GoodType",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
