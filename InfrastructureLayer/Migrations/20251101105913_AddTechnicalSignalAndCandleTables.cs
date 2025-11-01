using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicalSignalAndCandleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechnicalSignal",
                schema: "dbo",
                columns: table => new
                {
                    TechnicalSignalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IndicatorCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IndicatorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConditionTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SignalType = table.Column<int>(type: "int", nullable: false),
                    TimeFrame = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: true),
                    AdditionalData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalSignal", x => x.TechnicalSignalId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalSignal_CreatedAt",
                schema: "dbo",
                table: "TechnicalSignal",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalSignal_IndicatorCategory",
                schema: "dbo",
                table: "TechnicalSignal",
                column: "IndicatorCategory");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalSignal_IndicatorName",
                schema: "dbo",
                table: "TechnicalSignal",
                column: "IndicatorName");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalSignal_SignalType",
                schema: "dbo",
                table: "TechnicalSignal",
                column: "SignalType");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalSignal_Symbol",
                schema: "dbo",
                table: "TechnicalSignal",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalSignal_Symbol_IndicatorCategory_SignalType_TimeFrame",
                schema: "dbo",
                table: "TechnicalSignal",
                columns: new[] { "Symbol", "IndicatorCategory", "SignalType", "TimeFrame" });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalSignal_TimeFrame",
                schema: "dbo",
                table: "TechnicalSignal",
                column: "TimeFrame");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicalSignal",
                schema: "dbo");
        }
    }
}
