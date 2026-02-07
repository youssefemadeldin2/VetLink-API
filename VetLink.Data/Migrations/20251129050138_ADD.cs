using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetLink.Data.Migrations
{
    /// <inheritdoc />
    public partial class ADD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Addresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Addresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Egypt");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Addresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Addresses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Egypt");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Addresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_UserId",
                table: "Addresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_UserId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Addresses");
        }
    }
}
