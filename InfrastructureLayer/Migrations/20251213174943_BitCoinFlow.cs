using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class BitCoinFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DuneEtfIssuerFlowSnapshot",
                schema: "dbo",
                columns: table => new
                {
                    DuneEtfIssuerFlowSnapshotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExecutionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    QueryId = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionStartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionEndedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowCount = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    EtfTicker = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    AmountUsd = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    AmountNetFlow = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    AmountUsdNetFlow = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuneEtfIssuerFlowSnapshot", x => x.DuneEtfIssuerFlowSnapshotId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DuneEtfIssuerFlowSnapshot_Time_EtfTicker",
                schema: "dbo",
                table: "DuneEtfIssuerFlowSnapshot",
                columns: new[] { "Time", "EtfTicker" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DuneEtfIssuerFlowSnapshot",
                schema: "dbo");
        }
    }
}
