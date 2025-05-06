using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceTrackingAPI.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyProductPriceModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPrice",
                table: "ProductPrices");

            migrationBuilder.RenameColumn(
                name: "LowestPriceLast30Days",
                table: "ProductPrices",
                newName: "Price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "ProductPrices",
                newName: "LowestPriceLast30Days");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentPrice",
                table: "ProductPrices",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
