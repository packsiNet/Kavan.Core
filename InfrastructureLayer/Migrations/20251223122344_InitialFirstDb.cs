using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialFirstDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "BitcoinActiveAddress",
                schema: "dbo",
                columns: table => new
                {
                    BitcoinActiveAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExecutionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    QueryId = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionStartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionEndedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowCount = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Users = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitcoinActiveAddress", x => x.BitcoinActiveAddressId);
                });

            migrationBuilder.CreateTable(
                name: "Channel",
                schema: "dbo",
                columns: table => new
                {
                    ChannelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    AccessType = table.Column<int>(type: "int", nullable: false),
                    UniqueCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    BannerUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Channel", x => x.ChannelId);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMembership",
                schema: "dbo",
                columns: table => new
                {
                    ChannelMembershipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsMuted = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ChannelMembership", x => x.ChannelMembershipId);
                });

            migrationBuilder.CreateTable(
                name: "ChannelNewsDetail",
                schema: "dbo",
                columns: table => new
                {
                    ChannelNewsDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelNewsDetail", x => x.ChannelNewsDetailId);
                });

            migrationBuilder.CreateTable(
                name: "ChannelPost",
                schema: "dbo",
                columns: table => new
                {
                    ChannelPostId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LikesCount = table.Column<int>(type: "int", nullable: false),
                    CommentsCount = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_ChannelPost", x => x.ChannelPostId);
                });

            migrationBuilder.CreateTable(
                name: "ChannelPostComment",
                schema: "dbo",
                columns: table => new
                {
                    ChannelPostCommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
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
                    table.PrimaryKey("PK_ChannelPostComment", x => x.ChannelPostCommentId);
                });

            migrationBuilder.CreateTable(
                name: "ChannelPostReaction",
                schema: "dbo",
                columns: table => new
                {
                    ChannelPostReactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Reaction = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
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
                    table.PrimaryKey("PK_ChannelPostReaction", x => x.ChannelPostReactionId);
                });

            migrationBuilder.CreateTable(
                name: "ChannelRating",
                schema: "dbo",
                columns: table => new
                {
                    ChannelRatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Stars = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_ChannelRating", x => x.ChannelRatingId);
                });

            migrationBuilder.CreateTable(
                name: "ChannelSignalDetail",
                schema: "dbo",
                columns: table => new
                {
                    ChannelSignalDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Timeframe = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TradeType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StopLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelSignalDetail", x => x.ChannelSignalDetailId);
                });

            migrationBuilder.CreateTable(
                name: "ChannelSignalEntry",
                schema: "dbo",
                columns: table => new
                {
                    ChannelSignalEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelSignalEntry", x => x.ChannelSignalEntryId);
                });

            migrationBuilder.CreateTable(
                name: "ChannelSignalTp",
                schema: "dbo",
                columns: table => new
                {
                    ChannelSignalTpId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelSignalTp", x => x.ChannelSignalTpId);
                });

            migrationBuilder.CreateTable(
                name: "CourseCategory",
                schema: "dbo",
                columns: table => new
                {
                    CourseCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_CourseCategory", x => x.CourseCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Cryptocurrency",
                schema: "dbo",
                columns: table => new
                {
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaseAsset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuoteAsset = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Cryptocurrency", x => x.CryptocurrencyId);
                });

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
                    MedianGasPriceGwei = table.Column<decimal>(type: "decimal(38,20)", nullable: false),
                    EthTransferPriceUsd = table.Column<decimal>(type: "decimal(38,20)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuneGasPriceSnapshot", x => x.DuneGasPriceSnapshotId);
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

            migrationBuilder.CreateTable(
                name: "FinancialPeriod",
                schema: "dbo",
                columns: table => new
                {
                    FinancialPeriodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StartDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodType = table.Column<int>(type: "int", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_FinancialPeriod", x => x.FinancialPeriodId);
                });

            migrationBuilder.CreateTable(
                name: "Idea",
                schema: "dbo",
                columns: table => new
                {
                    IdeaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: true),
                    Timeframe = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Trend = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitleTranslate = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DescriptionTranslate = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_Idea", x => x.IdeaId);
                });

            migrationBuilder.CreateTable(
                name: "NewsPost",
                schema: "dbo",
                columns: table => new
                {
                    NewsPostId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<int>(type: "int", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtRemote = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    OriginalUrl = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    SourceTitle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SourceRegion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    SourceDomain = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SourceType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    VotesNegative = table.Column<int>(type: "int", nullable: false),
                    VotesPositive = table.Column<int>(type: "int", nullable: false),
                    VotesImportant = table.Column<int>(type: "int", nullable: false),
                    VotesLiked = table.Column<int>(type: "int", nullable: false),
                    VotesDisliked = table.Column<int>(type: "int", nullable: false),
                    VotesLol = table.Column<int>(type: "int", nullable: false),
                    VotesToxic = table.Column<int>(type: "int", nullable: false),
                    VotesSaved = table.Column<int>(type: "int", nullable: false),
                    VotesComments = table.Column<int>(type: "int", nullable: false),
                    PanicScore = table.Column<int>(type: "int", nullable: true),
                    PanicScore1h = table.Column<int>(type: "int", nullable: true),
                    Author = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ContentOriginal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentClean = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_NewsPost", x => x.NewsPostId);
                });

            migrationBuilder.CreateTable(
                name: "Plan",
                schema: "dbo",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceMonthly = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceYearly = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Plan", x => x.PlanId);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioEntry",
                schema: "dbo",
                columns: table => new
                {
                    PortfolioEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    BuyPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    BuyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_PortfolioEntry", x => x.PortfolioEntryId);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioSale",
                schema: "dbo",
                columns: table => new
                {
                    PortfolioSaleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    SellPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    SellDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_PortfolioSale", x => x.PortfolioSaleId);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "dbo",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                schema: "dbo",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RelatedEntityId = table.Column<int>(type: "int", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_Transaction", x => x.TransactionId);
                });

            migrationBuilder.CreateTable(
                name: "UserAccount",
                schema: "dbo",
                columns: table => new
                {
                    UserAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferredByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ConfirmEmail = table.Column<bool>(type: "bit", nullable: false),
                    PhonePrefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmPhoneNumber = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InviteCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastLoginTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SecurityCode = table.Column<int>(type: "int", nullable: true),
                    ExpireSecurityCode = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedAttempts = table.Column<int>(type: "int", nullable: false),
                    LockedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvitedByUserId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_UserAccount", x => x.UserAccountId);
                    table.ForeignKey(
                        name: "FK_UserAccount_UserAccount_InvitedByUserId",
                        column: x => x.InvitedByUserId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId");
                });

            migrationBuilder.CreateTable(
                name: "Watchlist",
                schema: "dbo",
                columns: table => new
                {
                    WatchlistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
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
                    table.PrimaryKey("PK_Watchlist", x => x.WatchlistId);
                });

            migrationBuilder.CreateTable(
                name: "WatchlistItem",
                schema: "dbo",
                columns: table => new
                {
                    WatchlistItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WatchlistId = table.Column<int>(type: "int", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_WatchlistItem", x => x.WatchlistItemId);
                });

            migrationBuilder.CreateTable(
                name: "AggregationState",
                schema: "dbo",
                columns: table => new
                {
                    AggregationStateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: false),
                    Timeframe = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LastProcessedOpenTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregationState", x => x.AggregationStateId);
                    table.ForeignKey(
                        name: "FK_AggregationState_Cryptocurrency_CryptocurrencyId",
                        column: x => x.CryptocurrencyId,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candle_15m",
                schema: "dbo",
                columns: table => new
                {
                    Candle_15mId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: false),
                    OpenTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CloseTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    High = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Close = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    NumberOfTrades = table.Column<int>(type: "int", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candle_15m", x => x.Candle_15mId);
                    table.ForeignKey(
                        name: "FK_Candle_15m_Cryptocurrency_CryptocurrencyId",
                        column: x => x.CryptocurrencyId,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candle_1d",
                schema: "dbo",
                columns: table => new
                {
                    Candle_1dId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: false),
                    OpenTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CloseTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    High = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Close = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    NumberOfTrades = table.Column<int>(type: "int", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candle_1d", x => x.Candle_1dId);
                    table.ForeignKey(
                        name: "FK_Candle_1d_Cryptocurrency_CryptocurrencyId",
                        column: x => x.CryptocurrencyId,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candle_1h",
                schema: "dbo",
                columns: table => new
                {
                    Candle_1hId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: false),
                    OpenTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CloseTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    High = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Close = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    NumberOfTrades = table.Column<int>(type: "int", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candle_1h", x => x.Candle_1hId);
                    table.ForeignKey(
                        name: "FK_Candle_1h_Cryptocurrency_CryptocurrencyId",
                        column: x => x.CryptocurrencyId,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candle_1m",
                schema: "dbo",
                columns: table => new
                {
                    Candle_1mId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: false),
                    OpenTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CloseTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    High = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Close = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    NumberOfTrades = table.Column<int>(type: "int", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candle_1m", x => x.Candle_1mId);
                    table.ForeignKey(
                        name: "FK_Candle_1m_Cryptocurrency_CryptocurrencyId",
                        column: x => x.CryptocurrencyId,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candle_1w",
                schema: "dbo",
                columns: table => new
                {
                    Candle_1wId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: false),
                    OpenTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CloseTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    High = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Close = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    NumberOfTrades = table.Column<int>(type: "int", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candle_1w", x => x.Candle_1wId);
                    table.ForeignKey(
                        name: "FK_Candle_1w_Cryptocurrency_CryptocurrencyId",
                        column: x => x.CryptocurrencyId,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candle_4h",
                schema: "dbo",
                columns: table => new
                {
                    Candle_4hId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: false),
                    OpenTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CloseTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    High = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Close = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    NumberOfTrades = table.Column<int>(type: "int", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candle_4h", x => x.Candle_4hId);
                    table.ForeignKey(
                        name: "FK_Candle_4h_Cryptocurrency_CryptocurrencyId",
                        column: x => x.CryptocurrencyId,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candle_5m",
                schema: "dbo",
                columns: table => new
                {
                    Candle_5mId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: false),
                    OpenTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CloseTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    High = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Close = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 8, nullable: false),
                    NumberOfTrades = table.Column<int>(type: "int", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candle_5m", x => x.Candle_5mId);
                    table.ForeignKey(
                        name: "FK_Candle_5m_Cryptocurrency_CryptocurrencyId",
                        column: x => x.CryptocurrencyId,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "Trade",
                schema: "dbo",
                columns: table => new
                {
                    TradeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Side = table.Column<int>(type: "int", nullable: false),
                    EntryPrice = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    StopLoss = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    Leverage = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OpenedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinancialPeriodId = table.Column<int>(type: "int", nullable: false),
                    Emotion_ConfidenceLevel = table.Column<int>(type: "int", nullable: true),
                    Emotion_EmotionBeforeEntry = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Emotion_PlanCompliance = table.Column<bool>(type: "bit", nullable: true),
                    Result_ExitPrice = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    Result_ExitReason = table.Column<int>(type: "int", nullable: true),
                    Result_RMultiple = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Result_PnLPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Result_HoldingTime = table.Column<TimeSpan>(type: "time", nullable: true),
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
                    table.PrimaryKey("PK_Trade", x => x.TradeId);
                    table.ForeignKey(
                        name: "FK_Trade_FinancialPeriod_FinancialPeriodId",
                        column: x => x.FinancialPeriodId,
                        principalSchema: "dbo",
                        principalTable: "FinancialPeriod",
                        principalColumn: "FinancialPeriodId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsInstrument",
                schema: "dbo",
                columns: table => new
                {
                    NewsInstrumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NewsPostId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    MarketCapUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceInUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceInBtc = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceInEth = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceInEur = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MarketRank = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_NewsInstrument", x => x.NewsInstrumentId);
                    table.ForeignKey(
                        name: "FK_NewsInstrument_NewsPost_NewsPostId",
                        column: x => x.NewsPostId,
                        principalSchema: "dbo",
                        principalTable: "NewsPost",
                        principalColumn: "NewsPostId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanFeature",
                schema: "dbo",
                columns: table => new
                {
                    PlanFeatureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_PlanFeature", x => x.PlanFeatureId);
                    table.ForeignKey(
                        name: "FK_PlanFeature_Plan_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "dbo",
                        principalTable: "Plan",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                schema: "dbo",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Goal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsFree = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CourseLevelValue = table.Column<byte>(type: "tinyint", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    OwnerUserId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_Course", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Course_CourseCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "dbo",
                        principalTable: "CourseCategory",
                        principalColumn: "CourseCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Course_UserAccount_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdeaComment",
                schema: "dbo",
                columns: table => new
                {
                    IdeaCommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdeaId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
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
                    table.PrimaryKey("PK_IdeaComment", x => x.IdeaCommentId);
                    table.ForeignKey(
                        name: "FK_IdeaComment_Idea_IdeaId",
                        column: x => x.IdeaId,
                        principalSchema: "dbo",
                        principalTable: "Idea",
                        principalColumn: "IdeaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdeaComment_UserAccount_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdeaRating",
                schema: "dbo",
                columns: table => new
                {
                    IdeaRatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdeaId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_IdeaRating", x => x.IdeaRatingId);
                    table.ForeignKey(
                        name: "FK_IdeaRating_Idea_IdeaId",
                        column: x => x.IdeaId,
                        principalSchema: "dbo",
                        principalTable: "Idea",
                        principalColumn: "IdeaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdeaRating_UserAccount_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                schema: "dbo",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAccountId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
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
                    table.PrimaryKey("PK_Notification", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notification_UserAccount_UserAccountId",
                        column: x => x.UserAccountId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationProfile",
                schema: "dbo",
                columns: table => new
                {
                    OrganizationProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAccountId = table.Column<int>(type: "int", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DescriptionDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FoundedYear = table.Column<int>(type: "int", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactEmailPublic = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPhonePublic = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BannerUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HasExchange = table.Column<bool>(type: "bit", nullable: false),
                    HasInvestmentPanel = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_OrganizationProfile", x => x.OrganizationProfileId);
                    table.ForeignKey(
                        name: "FK_OrganizationProfile_UserAccount_UserAccountId",
                        column: x => x.UserAccountId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                schema: "dbo",
                columns: table => new
                {
                    RefreshTokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAccountId = table.Column<int>(type: "int", nullable: false),
                    UserFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JwtId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_RefreshToken", x => x.RefreshTokenId);
                    table.ForeignKey(
                        name: "FK_RefreshToken_UserAccount_UserAccountId",
                        column: x => x.UserAccountId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFollow",
                schema: "dbo",
                columns: table => new
                {
                    UserFollowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FollowerUserId = table.Column<int>(type: "int", nullable: false),
                    FolloweeUserId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_UserFollow", x => x.UserFollowId);
                    table.ForeignKey(
                        name: "FK_UserFollow_UserAccount_FolloweeUserId",
                        column: x => x.FolloweeUserId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFollow_UserAccount_FollowerUserId",
                        column: x => x.FollowerUserId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPlan",
                schema: "dbo",
                columns: table => new
                {
                    UserPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAccountId = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_UserPlan", x => x.UserPlanId);
                    table.ForeignKey(
                        name: "FK_UserPlan_Plan_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "dbo",
                        principalTable: "Plan",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPlan_UserAccount_UserAccountId",
                        column: x => x.UserAccountId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                schema: "dbo",
                columns: table => new
                {
                    UserProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAccountId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AboutMe = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_UserProfile", x => x.UserProfileId);
                    table.ForeignKey(
                        name: "FK_UserProfile_UserAccount_UserAccountId",
                        column: x => x.UserAccountId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "dbo",
                columns: table => new
                {
                    UserRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    UserAccountId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_UserRole", x => x.UserRoleId);
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_UserAccount_UserAccountId",
                        column: x => x.UserAccountId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Cascade);
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
                    IsTrigger = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
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
                    table.PrimaryKey("PK_SignalCandle", x => x.SignalCandleId);
                    table.ForeignKey(
                        name: "FK_SignalCandle_Signal_SignalId",
                        column: x => x.SignalId,
                        principalSchema: "dbo",
                        principalTable: "Signal",
                        principalColumn: "SignalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradeTp",
                schema: "dbo",
                columns: table => new
                {
                    TradeTpId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradeId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    IsHit = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeTp", x => x.TradeTpId);
                    table.ForeignKey(
                        name: "FK_TradeTp_Trade_TradeId",
                        column: x => x.TradeId,
                        principalSchema: "dbo",
                        principalTable: "Trade",
                        principalColumn: "TradeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseEnrollment",
                schema: "dbo",
                columns: table => new
                {
                    CourseEnrollmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    UserAccountId = table.Column<int>(type: "int", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PricePaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CouponCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_CourseEnrollment", x => x.CourseEnrollmentId);
                    table.ForeignKey(
                        name: "FK_CourseEnrollment_Course_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "dbo",
                        principalTable: "Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseEnrollment_UserAccount_UserAccountId",
                        column: x => x.UserAccountId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lesson",
                schema: "dbo",
                columns: table => new
                {
                    LessonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    PublishAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFreePreview = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_Lesson", x => x.LessonId);
                    table.ForeignKey(
                        name: "FK_Lesson_Course_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "dbo",
                        principalTable: "Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationActivity",
                schema: "dbo",
                columns: table => new
                {
                    OrganizationActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationProfileId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_OrganizationActivity", x => x.OrganizationActivityId);
                    table.ForeignKey(
                        name: "FK_OrganizationActivity_OrganizationProfile_OrganizationProfileId",
                        column: x => x.OrganizationProfileId,
                        principalSchema: "dbo",
                        principalTable: "OrganizationProfile",
                        principalColumn: "OrganizationProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationEmail",
                schema: "dbo",
                columns: table => new
                {
                    OrganizationEmailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationProfileId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_OrganizationEmail", x => x.OrganizationEmailId);
                    table.ForeignKey(
                        name: "FK_OrganizationEmail_OrganizationProfile_OrganizationProfileId",
                        column: x => x.OrganizationProfileId,
                        principalSchema: "dbo",
                        principalTable: "OrganizationProfile",
                        principalColumn: "OrganizationProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationExchange",
                schema: "dbo",
                columns: table => new
                {
                    OrganizationExchangeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationProfileId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_OrganizationExchange", x => x.OrganizationExchangeId);
                    table.ForeignKey(
                        name: "FK_OrganizationExchange_OrganizationProfile_OrganizationProfileId",
                        column: x => x.OrganizationProfileId,
                        principalSchema: "dbo",
                        principalTable: "OrganizationProfile",
                        principalColumn: "OrganizationProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationInvestmentPanel",
                schema: "dbo",
                columns: table => new
                {
                    OrganizationInvestmentPanelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationProfileId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinimumInvestment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProfitShareModel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_OrganizationInvestmentPanel", x => x.OrganizationInvestmentPanelId);
                    table.ForeignKey(
                        name: "FK_OrganizationInvestmentPanel_OrganizationProfile_OrganizationProfileId",
                        column: x => x.OrganizationProfileId,
                        principalSchema: "dbo",
                        principalTable: "OrganizationProfile",
                        principalColumn: "OrganizationProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationLicense",
                schema: "dbo",
                columns: table => new
                {
                    OrganizationLicenseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationProfileId = table.Column<int>(type: "int", nullable: false),
                    RegulatorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_OrganizationLicense", x => x.OrganizationLicenseId);
                    table.ForeignKey(
                        name: "FK_OrganizationLicense_OrganizationProfile_OrganizationProfileId",
                        column: x => x.OrganizationProfileId,
                        principalSchema: "dbo",
                        principalTable: "OrganizationProfile",
                        principalColumn: "OrganizationProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPhone",
                schema: "dbo",
                columns: table => new
                {
                    OrganizationPhoneId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationProfileId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_OrganizationPhone", x => x.OrganizationPhoneId);
                    table.ForeignKey(
                        name: "FK_OrganizationPhone_OrganizationProfile_OrganizationProfileId",
                        column: x => x.OrganizationProfileId,
                        principalSchema: "dbo",
                        principalTable: "OrganizationProfile",
                        principalColumn: "OrganizationProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationSocialLink",
                schema: "dbo",
                columns: table => new
                {
                    OrganizationSocialLinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationProfileId = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_OrganizationSocialLink", x => x.OrganizationSocialLinkId);
                    table.ForeignKey(
                        name: "FK_OrganizationSocialLink_OrganizationProfile_OrganizationProfileId",
                        column: x => x.OrganizationProfileId,
                        principalSchema: "dbo",
                        principalTable: "OrganizationProfile",
                        principalColumn: "OrganizationProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationWebsite",
                schema: "dbo",
                columns: table => new
                {
                    OrganizationWebsiteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationProfileId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_OrganizationWebsite", x => x.OrganizationWebsiteId);
                    table.ForeignKey(
                        name: "FK_OrganizationWebsite_OrganizationProfile_OrganizationProfileId",
                        column: x => x.OrganizationProfileId,
                        principalSchema: "dbo",
                        principalTable: "OrganizationProfile",
                        principalColumn: "OrganizationProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaFile",
                schema: "dbo",
                columns: table => new
                {
                    MediaFileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true),
                    MediaFileTypeValue = table.Column<byte>(type: "tinyint", nullable: false),
                    IsStreamOnly = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
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
                    table.PrimaryKey("PK_MediaFile", x => x.MediaFileId);
                    table.ForeignKey(
                        name: "FK_MediaFile_Lesson_LessonId",
                        column: x => x.LessonId,
                        principalSchema: "dbo",
                        principalTable: "Lesson",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AggregationState_CryptocurrencyId_Timeframe",
                schema: "dbo",
                table: "AggregationState",
                columns: new[] { "CryptocurrencyId", "Timeframe" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BitcoinActiveAddress_Time",
                schema: "dbo",
                table: "BitcoinActiveAddress",
                column: "Time",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candle_15m_CryptocurrencyId_OpenTime",
                schema: "dbo",
                table: "Candle_15m",
                columns: new[] { "CryptocurrencyId", "OpenTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candle_15m_OpenTime",
                schema: "dbo",
                table: "Candle_15m",
                column: "OpenTime");

            migrationBuilder.CreateIndex(
                name: "IX_Candle_1d_CryptocurrencyId_OpenTime",
                schema: "dbo",
                table: "Candle_1d",
                columns: new[] { "CryptocurrencyId", "OpenTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candle_1d_OpenTime",
                schema: "dbo",
                table: "Candle_1d",
                column: "OpenTime");

            migrationBuilder.CreateIndex(
                name: "IX_Candle_1h_CryptocurrencyId_OpenTime",
                schema: "dbo",
                table: "Candle_1h",
                columns: new[] { "CryptocurrencyId", "OpenTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candle_1h_OpenTime",
                schema: "dbo",
                table: "Candle_1h",
                column: "OpenTime");

            migrationBuilder.CreateIndex(
                name: "IX_Candle_1m_CryptocurrencyId_OpenTime",
                schema: "dbo",
                table: "Candle_1m",
                columns: new[] { "CryptocurrencyId", "OpenTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candle_1m_OpenTime",
                schema: "dbo",
                table: "Candle_1m",
                column: "OpenTime");

            migrationBuilder.CreateIndex(
                name: "IX_Candle_1w_CryptocurrencyId_OpenTime",
                schema: "dbo",
                table: "Candle_1w",
                columns: new[] { "CryptocurrencyId", "OpenTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candle_1w_OpenTime",
                schema: "dbo",
                table: "Candle_1w",
                column: "OpenTime");

            migrationBuilder.CreateIndex(
                name: "IX_Candle_4h_CryptocurrencyId_OpenTime",
                schema: "dbo",
                table: "Candle_4h",
                columns: new[] { "CryptocurrencyId", "OpenTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candle_4h_OpenTime",
                schema: "dbo",
                table: "Candle_4h",
                column: "OpenTime");

            migrationBuilder.CreateIndex(
                name: "IX_Candle_5m_CryptocurrencyId_OpenTime",
                schema: "dbo",
                table: "Candle_5m",
                columns: new[] { "CryptocurrencyId", "OpenTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candle_5m_OpenTime",
                schema: "dbo",
                table: "Candle_5m",
                column: "OpenTime");

            migrationBuilder.CreateIndex(
                name: "IX_Channel_OwnerUserId",
                schema: "dbo",
                table: "Channel",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Channel_Slug",
                schema: "dbo",
                table: "Channel",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channel_UniqueCode",
                schema: "dbo",
                table: "Channel",
                column: "UniqueCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMembership_ChannelId_UserId",
                schema: "dbo",
                table: "ChannelMembership",
                columns: new[] { "ChannelId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelPost_ChannelId",
                schema: "dbo",
                table: "ChannelPost",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelPostComment_PostId",
                schema: "dbo",
                table: "ChannelPostComment",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelPostReaction_PostId",
                schema: "dbo",
                table: "ChannelPostReaction",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelPostReaction_PostId_UserId",
                schema: "dbo",
                table: "ChannelPostReaction",
                columns: new[] { "PostId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelRating_ChannelId",
                schema: "dbo",
                table: "ChannelRating",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelRating_ChannelId_UserId",
                schema: "dbo",
                table: "ChannelRating",
                columns: new[] { "ChannelId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelSignalEntry_PostId",
                schema: "dbo",
                table: "ChannelSignalEntry",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelSignalTp_PostId",
                schema: "dbo",
                table: "ChannelSignalTp",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_CategoryId",
                schema: "dbo",
                table: "Course",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_OwnerUserId",
                schema: "dbo",
                table: "Course",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Slug",
                schema: "dbo",
                table: "Course",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseCategory_Slug",
                schema: "dbo",
                table: "CourseCategory",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollment_CourseId",
                schema: "dbo",
                table: "CourseEnrollment",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollment_UserAccountId_CourseId",
                schema: "dbo",
                table: "CourseEnrollment",
                columns: new[] { "UserAccountId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cryptocurrency_Symbol",
                schema: "dbo",
                table: "Cryptocurrency",
                column: "Symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DuneDailyTxCountSnapshot_Time",
                schema: "dbo",
                table: "DuneDailyTxCountSnapshot",
                column: "Time",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DuneEtfIssuerFlowSnapshot_Time_EtfTicker",
                schema: "dbo",
                table: "DuneEtfIssuerFlowSnapshot",
                columns: new[] { "Time", "EtfTicker" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DuneGasPriceSnapshot_Time",
                schema: "dbo",
                table: "DuneGasPriceSnapshot",
                column: "Time",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DuneMetricsSnapshot_ExecutionId",
                schema: "dbo",
                table: "DuneMetricsSnapshot",
                column: "ExecutionId",
                unique: true,
                filter: "[ExecutionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPeriod_IsClosed",
                schema: "dbo",
                table: "FinancialPeriod",
                column: "IsClosed");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialPeriod_UserId",
                schema: "dbo",
                table: "FinancialPeriod",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Idea_IsPublic",
                schema: "dbo",
                table: "Idea",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_Idea_Symbol_Timeframe",
                schema: "dbo",
                table: "Idea",
                columns: new[] { "Symbol", "Timeframe" });

            migrationBuilder.CreateIndex(
                name: "IX_IdeaComment_IdeaId",
                schema: "dbo",
                table: "IdeaComment",
                column: "IdeaId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaComment_UserId",
                schema: "dbo",
                table: "IdeaComment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaRating_IdeaId_UserId",
                schema: "dbo",
                table: "IdeaRating",
                columns: new[] { "IdeaId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdeaRating_UserId",
                schema: "dbo",
                table: "IdeaRating",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_CourseId_Order",
                schema: "dbo",
                table: "Lesson",
                columns: new[] { "CourseId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_PublishAt",
                schema: "dbo",
                table: "Lesson",
                column: "PublishAt");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFile_LessonId_MediaFileTypeValue",
                schema: "dbo",
                table: "MediaFile",
                columns: new[] { "LessonId", "MediaFileTypeValue" });

            migrationBuilder.CreateIndex(
                name: "IX_NewsInstrument_NewsPostId",
                schema: "dbo",
                table: "NewsInstrument",
                column: "NewsPostId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsPost_ExternalId",
                schema: "dbo",
                table: "NewsPost",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserAccountId",
                schema: "dbo",
                table: "Notification",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationActivity_OrganizationProfileId",
                schema: "dbo",
                table: "OrganizationActivity",
                column: "OrganizationProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationEmail_OrganizationProfileId",
                schema: "dbo",
                table: "OrganizationEmail",
                column: "OrganizationProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationExchange_OrganizationProfileId",
                schema: "dbo",
                table: "OrganizationExchange",
                column: "OrganizationProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationInvestmentPanel_OrganizationProfileId",
                schema: "dbo",
                table: "OrganizationInvestmentPanel",
                column: "OrganizationProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationLicense_OrganizationProfileId",
                schema: "dbo",
                table: "OrganizationLicense",
                column: "OrganizationProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhone_OrganizationProfileId",
                schema: "dbo",
                table: "OrganizationPhone",
                column: "OrganizationProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProfile_Country",
                schema: "dbo",
                table: "OrganizationProfile",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProfile_IsPublic",
                schema: "dbo",
                table: "OrganizationProfile",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProfile_UserAccountId",
                schema: "dbo",
                table: "OrganizationProfile",
                column: "UserAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSocialLink_OrganizationProfileId",
                schema: "dbo",
                table: "OrganizationSocialLink",
                column: "OrganizationProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationWebsite_OrganizationProfileId",
                schema: "dbo",
                table: "OrganizationWebsite",
                column: "OrganizationProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanFeature_PlanId",
                schema: "dbo",
                table: "PlanFeature",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioEntry_CryptocurrencyId",
                schema: "dbo",
                table: "PortfolioEntry",
                column: "CryptocurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioEntry_Symbol",
                schema: "dbo",
                table: "PortfolioEntry",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioEntry_Symbol_BuyDate",
                schema: "dbo",
                table: "PortfolioEntry",
                columns: new[] { "Symbol", "BuyDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioSale_CryptocurrencyId",
                schema: "dbo",
                table: "PortfolioSale",
                column: "CryptocurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioSale_Symbol",
                schema: "dbo",
                table: "PortfolioSale",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioSale_Symbol_SellDate",
                schema: "dbo",
                table: "PortfolioSale",
                columns: new[] { "Symbol", "SellDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserAccountId",
                schema: "dbo",
                table: "RefreshToken",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Signal_CryptocurrencyId_SignalTime",
                schema: "dbo",
                table: "Signal",
                columns: new[] { "CryptocurrencyId", "SignalTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Signal_Symbol_Timeframe_SignalCategory_SignalName_Direction_SignalTime",
                schema: "dbo",
                table: "Signal",
                columns: new[] { "Symbol", "Timeframe", "SignalCategory", "SignalName", "Direction", "SignalTime" });

            migrationBuilder.CreateIndex(
                name: "IX_SignalCandle_SignalId_Index",
                schema: "dbo",
                table: "SignalCandle",
                columns: new[] { "SignalId", "Index" });

            migrationBuilder.CreateIndex(
                name: "IX_SignalCandle_SignalId_IsTrigger",
                schema: "dbo",
                table: "SignalCandle",
                columns: new[] { "SignalId", "IsTrigger" });

            migrationBuilder.CreateIndex(
                name: "IX_Trade_FinancialPeriodId",
                schema: "dbo",
                table: "Trade",
                column: "FinancialPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_Status",
                schema: "dbo",
                table: "Trade",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_Symbol",
                schema: "dbo",
                table: "Trade",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_UserId",
                schema: "dbo",
                table: "Trade",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeTp_TradeId",
                schema: "dbo",
                table: "TradeTp",
                column: "TradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ReferenceId",
                schema: "dbo",
                table: "Transaction",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_UserId",
                schema: "dbo",
                table: "Transaction",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_InvitedByUserId",
                schema: "dbo",
                table: "UserAccount",
                column: "InvitedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollow_FolloweeUserId",
                schema: "dbo",
                table: "UserFollow",
                column: "FolloweeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollow_FollowerUserId_FolloweeUserId",
                schema: "dbo",
                table: "UserFollow",
                columns: new[] { "FollowerUserId", "FolloweeUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPlan_PlanId",
                schema: "dbo",
                table: "UserPlan",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlan_UserAccountId",
                schema: "dbo",
                table: "UserPlan",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_UserAccountId",
                schema: "dbo",
                table: "UserProfile",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                schema: "dbo",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserAccountId",
                schema: "dbo",
                table: "UserRole",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Watchlist_OwnerUserId",
                schema: "dbo",
                table: "Watchlist",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchlistItem_Symbol",
                schema: "dbo",
                table: "WatchlistItem",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_WatchlistItem_WatchlistId_Symbol",
                schema: "dbo",
                table: "WatchlistItem",
                columns: new[] { "WatchlistId", "Symbol" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregationState",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BitcoinActiveAddress",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Candle_15m",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Candle_1d",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Candle_1h",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Candle_1m",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Candle_1w",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Candle_4h",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Candle_5m",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Channel",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ChannelMembership",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ChannelNewsDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ChannelPost",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ChannelPostComment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ChannelPostReaction",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ChannelRating",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ChannelSignalDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ChannelSignalEntry",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ChannelSignalTp",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CourseEnrollment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DuneDailyTxCountSnapshot",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DuneEtfIssuerFlowSnapshot",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DuneGasPriceSnapshot",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DuneMetricsSnapshot",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "IdeaComment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "IdeaRating",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MediaFile",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "NewsInstrument",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Notification",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrganizationActivity",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrganizationEmail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrganizationExchange",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrganizationInvestmentPanel",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrganizationLicense",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrganizationPhone",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrganizationSocialLink",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrganizationWebsite",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PlanFeature",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PortfolioEntry",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PortfolioSale",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RefreshToken",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SignalCandle",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TradeTp",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Transaction",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserFollow",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserPlan",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserProfile",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Watchlist",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WatchlistItem",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Idea",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Lesson",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "NewsPost",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrganizationProfile",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Signal",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Trade",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Plan",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Course",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Cryptocurrency",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FinancialPeriod",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CourseCategory",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserAccount",
                schema: "dbo");
        }
    }
}
