using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "MedianGasPriceGwei",
                schema: "dbo",
                table: "DuneGasPriceSnapshot",
                type: "decimal(38,20)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EthTransferPriceUsd",
                schema: "dbo",
                table: "DuneGasPriceSnapshot",
                type: "decimal(38,20)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,10)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "MedianGasPriceGwei",
                schema: "dbo",
                table: "DuneGasPriceSnapshot",
                type: "decimal(28,10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,20)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EthTransferPriceUsd",
                schema: "dbo",
                table: "DuneGasPriceSnapshot",
                type: "decimal(28,10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,20)");
        }
    }
}
