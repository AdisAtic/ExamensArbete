using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceTrackingAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemovePriceAndMinimumQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumQuantity",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductPrices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinimumQuantity",
                table: "ProductPrices",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductPrices",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
