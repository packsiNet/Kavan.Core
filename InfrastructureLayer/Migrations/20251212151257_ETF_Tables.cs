using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class ETF_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DuneDailyTxCountSnapshot",
                schema: "dbo",
                columns: table => new
                {
                    DuneDailyTxCountSnapshotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExecutionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    QueryId = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionStartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionEndedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowCount = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TxCount = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    TxCountMovingAverage = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuneDailyTxCountSnapshot", x => x.DuneDailyTxCountSnapshotId);
                });

            migrationBuilder.CreateTable(
                name: "DuneMetricsSnapshot",
                schema: "dbo",
                columns: table => new
                {
                    DuneMetricsSnapshotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExecutionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    QueryId = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionStartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionEndedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowCount = table.Column<int>(type: "int", nullable: false),
                    TvlInThousands = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    UsdTvlInBillions = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    PastWeekFlows = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    FlowsUsdSinceApprovalInThousands = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    PastWeekFlowsUsdInThousands = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    PercentageOfBtc = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    BtcSupply = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    SixMonthsAnnualisedImpactOnSupply = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    ThreeMonthsAnnualisedImpactOnSupply = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    MonthlyAnnualisedImpactOnSupply = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    ByWeeklyAnnualisedImpactOnSupply = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    WeekAnnualisedImpactOnSupply = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    CreatedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuneMetricsSnapshot", x => x.DuneMetricsSnapshotId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DuneDailyTxCountSnapshot_Time",
                schema: "dbo",
                table: "DuneDailyTxCountSnapshot",
                column: "Time",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DuneMetricsSnapshot_ExecutionId",
                schema: "dbo",
                table: "DuneMetricsSnapshot",
                column: "ExecutionId",
                unique: true,
                filter: "[ExecutionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DuneDailyTxCountSnapshot",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DuneMetricsSnapshot",
                schema: "dbo");
        }
    }
}
