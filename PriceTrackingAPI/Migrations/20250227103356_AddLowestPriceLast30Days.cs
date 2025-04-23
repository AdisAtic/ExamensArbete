using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceTrackingAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddLowestPriceLast30Days : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LowestPriceLast30Days",
                table: "ProductPrices",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LowestPriceLast30Days",
                table: "ProductPrices");
        }
    }
}
