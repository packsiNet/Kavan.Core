using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddSignalTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create Signals Table
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Signals]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Signals](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [CryptocurrencyId] [int] NOT NULL,
                        [Symbol] [nvarchar](50) NOT NULL,
                        [Timeframe] [nvarchar](10) NOT NULL,
                        [SignalTime] [datetime2](7) NOT NULL,
                        [SignalCategory] [nvarchar](50) NOT NULL,
                        [SignalName] [nvarchar](50) NOT NULL,
                        [Direction] [int] NOT NULL,
                        [BreakoutLevel] [decimal](18, 8) NOT NULL,
                        [NearestResistance] [decimal](18, 8) NOT NULL,
                        [NearestSupport] [decimal](18, 8) NOT NULL,
                        [PivotR1] [decimal](18, 8) NOT NULL,
                        [PivotR2] [decimal](18, 8) NOT NULL,
                        [PivotR3] [decimal](18, 8) NOT NULL,
                        [PivotS1] [decimal](18, 8) NOT NULL,
                        [PivotS2] [decimal](18, 8) NOT NULL,
                        [PivotS3] [decimal](18, 8) NOT NULL,
                        [Atr] [decimal](18, 8) NOT NULL,
                        [Tolerance] [decimal](18, 8) NOT NULL,
                        [VolumeRatio] [decimal](18, 8) NOT NULL,
                        [BodySize] [decimal](18, 8) NOT NULL,
                        [CandleOpenTime] [datetime2](7) NOT NULL,
                        [CandleCloseTime] [datetime2](7) NOT NULL,
                        [CandleOpen] [decimal](18, 8) NOT NULL,
                        [CandleHigh] [decimal](18, 8) NOT NULL,
                        [CandleLow] [decimal](18, 8) NOT NULL,
                        [CandleClose] [decimal](18, 8) NOT NULL,
                        [CandleVolume] [decimal](18, 8) NOT NULL,
                        [IsActive] [bit] NOT NULL,
                        [IsDeleted] [bit] NOT NULL,
                        [CreatedAt] [datetime2](7) NOT NULL,
                        [UpdatedAt] [datetime2](7) NULL,
                        [CreatedByUserId] [int] NULL,
                        [CreatedByIp] [char](15) NULL,
                        [ModifiedByUserId] [int] NULL,
                        [ModifiedByIp] [char](15) NULL,
                        [RowVersion] [rowversion] NOT NULL,
                        CONSTRAINT [PK_Signals] PRIMARY KEY CLUSTERED ([Id] ASC),
                        CONSTRAINT [FK_Signals_Cryptocurrency_CryptocurrencyId] FOREIGN KEY([CryptocurrencyId]) REFERENCES [dbo].[Cryptocurrency] ([CryptocurrencyId])
                    )
                END
            ");

            // 2. Create SignalCandles Table
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SignalCandles]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[SignalCandles](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [SignalId] [int] NOT NULL,
                        [Index] [int] NOT NULL,
                        [Timeframe] [nvarchar](10) NOT NULL,
                        [OpenTime] [datetime2](7) NOT NULL,
                        [CloseTime] [datetime2](7) NOT NULL,
                        [Open] [decimal](18, 8) NOT NULL,
                        [High] [decimal](18, 8) NOT NULL,
                        [Low] [decimal](18, 8) NOT NULL,
                        [Close] [decimal](18, 8) NOT NULL,
                        [Volume] [decimal](18, 8) NOT NULL,
                        [IsTrigger] [bit] NOT NULL DEFAULT 0,
                        [IsActive] [bit] NOT NULL,
                        [IsDeleted] [bit] NOT NULL,
                        [CreatedAt] [datetime2](7) NOT NULL,
                        [UpdatedAt] [datetime2](7) NULL,
                        [CreatedByUserId] [int] NULL,
                        [CreatedByIp] [char](15) NULL,
                        [ModifiedByUserId] [int] NULL,
                        [ModifiedByIp] [char](15) NULL,
                        [RowVersion] [rowversion] NOT NULL,
                        CONSTRAINT [PK_SignalCandles] PRIMARY KEY CLUSTERED ([Id] ASC),
                        CONSTRAINT [FK_SignalCandles_Signals_SignalId] FOREIGN KEY([SignalId]) REFERENCES [dbo].[Signals] ([Id]) ON DELETE CASCADE
                    )
                END
            ");

            // 3. Update Candle Tables
            var candleTables = new[] { "Candle_1m", "Candle_5m", "Candle_15m", "Candle_1h", "Candle_4h", "Candle_1d", "Candle_1w" };
            foreach (var table in candleTables)
            {
                migrationBuilder.Sql($@"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{table}]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[{table}]') AND name = 'IsFinal')
                        BEGIN
                            ALTER TABLE [dbo].[{table}] ADD [IsFinal] [bit] NOT NULL DEFAULT 0;
                        END

                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[{table}]') AND name = 'LastUpdatedUtc')
                        BEGIN
                            ALTER TABLE [dbo].[{table}] ADD [LastUpdatedUtc] [datetime2](7) NOT NULL DEFAULT '0001-01-01';
                        END
                    END
                ");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS [dbo].[SignalCandles]");
            migrationBuilder.Sql("DROP TABLE IF EXISTS [dbo].[Signals]");
            
            var candleTables = new[] { "Candle_1m", "Candle_5m", "Candle_15m", "Candle_1h", "Candle_4h", "Candle_1d", "Candle_1w" };
            foreach (var table in candleTables)
            {
                migrationBuilder.Sql($@"
                    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[{table}]') AND name = 'IsFinal')
                    BEGIN
                        ALTER TABLE [dbo].[{table}] DROP COLUMN [IsFinal];
                    END
                    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[{table}]') AND name = 'LastUpdatedUtc')
                    BEGIN
                        ALTER TABLE [dbo].[{table}] DROP COLUMN [LastUpdatedUtc];
                    END
                ");
            }
        }
    }
}
