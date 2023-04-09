using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SantasWishlist.Context.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIsGoodToWasGoodForConsistency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsGood",
                table: "AspNetUsers",
                newName: "WasGood");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WasGood",
                table: "AspNetUsers",
                newName: "IsGood");
        }
    }
}
