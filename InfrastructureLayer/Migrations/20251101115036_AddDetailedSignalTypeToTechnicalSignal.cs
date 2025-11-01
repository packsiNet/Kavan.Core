using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddDetailedSignalTypeToTechnicalSignal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TechnicalSignal_Symbol_IndicatorCategory_SignalType_TimeFrame",
                schema: "dbo",
                table: "TechnicalSignal");

            migrationBuilder.AddColumn<int>(
                name: "DetailedSignalType",
                schema: "dbo",
                table: "TechnicalSignal",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalSignal_DetailedSignalType",
                schema: "dbo",
                table: "TechnicalSignal",
                column: "DetailedSignalType");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalSignal_Symbol_IndicatorCategory_SignalType_DetailedSignalType_TimeFrame",
                schema: "dbo",
                table: "TechnicalSignal",
                columns: new[] { "Symbol", "IndicatorCategory", "SignalType", "DetailedSignalType", "TimeFrame" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TechnicalSignal_DetailedSignalType",
                schema: "dbo",
                table: "TechnicalSignal");

            migrationBuilder.DropIndex(
                name: "IX_TechnicalSignal_Symbol_IndicatorCategory_SignalType_DetailedSignalType_TimeFrame",
                schema: "dbo",
                table: "TechnicalSignal");

            migrationBuilder.DropColumn(
                name: "DetailedSignalType",
                schema: "dbo",
                table: "TechnicalSignal");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalSignal_Symbol_IndicatorCategory_SignalType_TimeFrame",
                schema: "dbo",
                table: "TechnicalSignal",
                columns: new[] { "Symbol", "IndicatorCategory", "SignalType", "TimeFrame" });
        }
    }
}
