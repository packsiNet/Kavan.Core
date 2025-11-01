using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketAnalysisResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketAnalysisResult",
                schema: "dbo",
                columns: table => new
                {
                    MarketAnalysisResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Signals = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnalyzedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalSymbolsAnalyzed = table.Column<int>(type: "int", nullable: false),
                    SignalsGenerated = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketAnalysisResult", x => x.MarketAnalysisResultId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketAnalysisResult_AnalyzedAt",
                schema: "dbo",
                table: "MarketAnalysisResult",
                column: "AnalyzedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketAnalysisResult_RequestId",
                schema: "dbo",
                table: "MarketAnalysisResult",
                column: "RequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketAnalysisResult",
                schema: "dbo");
        }
    }
}
