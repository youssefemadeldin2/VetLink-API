using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetLink.Data.Migrations
{
    /// <inheritdoc />
    public partial class indexs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Shipments_SellerId",
                table: "Shipments",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                table: "Coupons",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shipments_SellerId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_Code",
                table: "Coupons");
        }
    }
}
