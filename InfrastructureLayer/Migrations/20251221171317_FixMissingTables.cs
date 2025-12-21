using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class FixMissingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DuneGasPriceSnapshot]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[DuneGasPriceSnapshot](
                        [DuneGasPriceSnapshotId] [int] IDENTITY(1,1) NOT NULL,
                        [ExecutionId] [nvarchar](64) NULL,
                        [QueryId] [int] NOT NULL,
                        [SubmittedAt] [datetime2](7) NOT NULL,
                        [ExpiresAt] [datetime2](7) NOT NULL,
                        [ExecutionStartedAt] [datetime2](7) NOT NULL,
                        [ExecutionEndedAt] [datetime2](7) NOT NULL,
                        [RowCount] [int] NOT NULL,
                        [Time] [datetime2](7) NOT NULL,
                        [MedianGasPriceGwei] [decimal](38, 20) NOT NULL,
                        [EthTransferPriceUsd] [decimal](38, 20) NOT NULL,
                        [IsActive] [bit] NOT NULL,
                        [IsDeleted] [bit] NOT NULL,
                        [CreatedAt] [datetime2](7) NOT NULL,
                        [UpdatedAt] [datetime2](7) NULL,
                        CONSTRAINT [PK_DuneGasPriceSnapshot] PRIMARY KEY CLUSTERED 
                        (
                            [DuneGasPriceSnapshotId] ASC
                        )
                    )
                END

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DuneDailyTxCountSnapshot]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[DuneDailyTxCountSnapshot](
                        [DuneDailyTxCountSnapshotId] [int] IDENTITY(1,1) NOT NULL,
                        [ExecutionId] [nvarchar](64) NULL,
                        [QueryId] [int] NOT NULL,
                        [SubmittedAt] [datetime2](7) NOT NULL,
                        [ExpiresAt] [datetime2](7) NOT NULL,
                        [ExecutionStartedAt] [datetime2](7) NOT NULL,
                        [ExecutionEndedAt] [datetime2](7) NOT NULL,
                        [RowCount] [int] NOT NULL,
                        [Time] [datetime2](7) NOT NULL,
                        [TxCount] [decimal](28, 10) NOT NULL,
                        [TxCountMovingAverage] [decimal](28, 10) NOT NULL,
                        [IsActive] [bit] NOT NULL,
                        [IsDeleted] [bit] NOT NULL,
                        [CreatedAt] [datetime2](7) NOT NULL,
                        [UpdatedAt] [datetime2](7) NULL,
                        CONSTRAINT [PK_DuneDailyTxCountSnapshot] PRIMARY KEY CLUSTERED 
                        (
                            [DuneDailyTxCountSnapshotId] ASC
                        )
                    )
                END

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DuneEtfIssuerFlowSnapshot]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[DuneEtfIssuerFlowSnapshot](
                        [DuneEtfIssuerFlowSnapshotId] [int] IDENTITY(1,1) NOT NULL,
                        [ExecutionId] [nvarchar](64) NULL,
                        [QueryId] [int] NOT NULL,
                        [SubmittedAt] [datetime2](7) NOT NULL,
                        [ExpiresAt] [datetime2](7) NOT NULL,
                        [ExecutionStartedAt] [datetime2](7) NOT NULL,
                        [ExecutionEndedAt] [datetime2](7) NOT NULL,
                        [RowCount] [int] NOT NULL,
                        [Time] [datetime2](7) NOT NULL,
                        [Issuer] [nvarchar](128) NOT NULL,
                        [EtfTicker] [nvarchar](32) NOT NULL,
                        [Amount] [decimal](28, 10) NOT NULL,
                        [AmountUsd] [decimal](28, 10) NOT NULL,
                        [AmountNetFlow] [decimal](28, 10) NULL,
                        [AmountUsdNetFlow] [decimal](28, 10) NULL,
                        [IsActive] [bit] NOT NULL,
                        [IsDeleted] [bit] NOT NULL,
                        [CreatedAt] [datetime2](7) NOT NULL,
                        [UpdatedAt] [datetime2](7) NULL,
                        CONSTRAINT [PK_DuneEtfIssuerFlowSnapshot] PRIMARY KEY CLUSTERED 
                        (
                            [DuneEtfIssuerFlowSnapshotId] ASC
                        )
                    )
                END

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DuneMetricsSnapshot]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[DuneMetricsSnapshot](
                        [DuneMetricsSnapshotId] [int] IDENTITY(1,1) NOT NULL,
                        [ExecutionId] [nvarchar](64) NULL,
                        [QueryId] [int] NOT NULL,
                        [SubmittedAt] [datetime2](7) NOT NULL,
                        [ExpiresAt] [datetime2](7) NOT NULL,
                        [ExecutionStartedAt] [datetime2](7) NOT NULL,
                        [ExecutionEndedAt] [datetime2](7) NOT NULL,
                        [RowCount] [int] NOT NULL,
                        [TvlInThousands] [decimal](28, 10) NOT NULL,
                        [UsdTvlInBillions] [decimal](28, 10) NOT NULL,
                        [PastWeekFlows] [decimal](28, 10) NOT NULL,
                        [FlowsUsdSinceApprovalInThousands] [decimal](28, 10) NOT NULL,
                        [PastWeekFlowsUsdInThousands] [decimal](28, 10) NOT NULL,
                        [PercentageOfBtc] [decimal](28, 10) NOT NULL,
                        [BtcSupply] [decimal](28, 10) NOT NULL,
                        [SixMonthsAnnualisedImpactOnSupply] [decimal](28, 10) NOT NULL,
                        [ThreeMonthsAnnualisedImpactOnSupply] [decimal](28, 10) NOT NULL,
                        [MonthlyAnnualisedImpactOnSupply] [decimal](28, 10) NOT NULL,
                        [ByWeeklyAnnualisedImpactOnSupply] [decimal](28, 10) NOT NULL,
                        [WeekAnnualisedImpactOnSupply] [decimal](28, 10) NOT NULL,
                        [CreatedByIp] [char](15) NULL,
                        [CreatedByUserId] [int] NULL,
                        [ModifiedByIp] [char](15) NULL,
                        [ModifiedByUserId] [int] NULL,
                        [RowVersion] [rowversion] NOT NULL,
                        [IsActive] [bit] NOT NULL,
                        [IsDeleted] [bit] NOT NULL,
                        [CreatedAt] [datetime2](7) NOT NULL,
                        [UpdatedAt] [datetime2](7) NULL,
                        CONSTRAINT [PK_DuneMetricsSnapshot] PRIMARY KEY CLUSTERED 
                        (
                            [DuneMetricsSnapshotId] ASC
                        )
                    )
                END

            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
