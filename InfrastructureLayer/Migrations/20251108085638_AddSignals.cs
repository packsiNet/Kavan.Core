using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddSignals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Signal",
                schema: "dbo",
                columns: table => new
                {
                    SignalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Timeframe = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SignalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SignalCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SignalName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    BreakoutLevel = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NearestResistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NearestSupport = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PivotR1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PivotR2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PivotR3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PivotS1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PivotS2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PivotS3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Atr = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tolerance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VolumeRatio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BodySize = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CandleOpenTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CandleCloseTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CandleOpen = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CandleHigh = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CandleLow = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CandleClose = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CandleVolume = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signal", x => x.SignalId);
                    table.ForeignKey(
                        name: "FK_Signal_Cryptocurrency_CryptocurrencyId",
                        column: x => x.CryptocurrencyId,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SignalCandle",
                schema: "dbo",
                columns: table => new
                {
                    SignalCandleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SignalId = table.Column<int>(type: "int", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Timeframe = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OpenTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CloseTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    High = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Close = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignalCandle", x => x.SignalCandleId);
                    table.ForeignKey(
                        name: "FK_SignalCandle_Signal_SignalId",
                        column: x => x.SignalId,
                        principalSchema: "dbo",
                        principalTable: "Signal",
                        principalColumn: "SignalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Signal_CryptocurrencyId",
                schema: "dbo",
                table: "Signal",
                column: "CryptocurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SignalCandle_SignalId",
                schema: "dbo",
                table: "SignalCandle",
                column: "SignalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SignalCandle",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Signal",
                schema: "dbo");
        }
    }
}
