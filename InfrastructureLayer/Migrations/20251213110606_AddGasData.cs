using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddGasData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DuneGasPriceSnapshot",
                schema: "dbo",
                columns: table => new
                {
                    DuneGasPriceSnapshotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExecutionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    QueryId = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionStartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionEndedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowCount = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MedianGasPriceGwei = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    EthTransferPriceUsd = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuneGasPriceSnapshot", x => x.DuneGasPriceSnapshotId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DuneGasPriceSnapshot_Time",
                schema: "dbo",
                table: "DuneGasPriceSnapshot",
                column: "Time",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DuneGasPriceSnapshot",
                schema: "dbo");
        }
    }
}
