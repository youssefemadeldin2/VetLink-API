using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetLink.Data.Migrations
{
    /// <inheritdoc />
    public partial class Wishlists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WishlistProductId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WishlistUsreId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WishlistProductId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WishlistUsreId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    UsreId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => new { x.UsreId, x.ProductId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_WishlistUsreId_WishlistProductId",
                table: "Users",
                columns: new[] { "WishlistUsreId", "WishlistProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_WishlistUsreId_WishlistProductId",
                table: "Products",
                columns: new[] { "WishlistUsreId", "WishlistProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Wishlists_WishlistUsreId_WishlistProductId",
                table: "Products",
                columns: new[] { "WishlistUsreId", "WishlistProductId" },
                principalTable: "Wishlists",
                principalColumns: new[] { "UsreId", "ProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Wishlists_WishlistUsreId_WishlistProductId",
                table: "Users",
                columns: new[] { "WishlistUsreId", "WishlistProductId" },
                principalTable: "Wishlists",
                principalColumns: new[] { "UsreId", "ProductId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Wishlists_WishlistUsreId_WishlistProductId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Wishlists_WishlistUsreId_WishlistProductId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Users_WishlistUsreId_WishlistProductId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Products_WishlistUsreId_WishlistProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WishlistProductId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WishlistUsreId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WishlistProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WishlistUsreId",
                table: "Products");
        }
    }
}
