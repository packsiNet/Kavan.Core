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
                name: "Cryptocurrency",
                schema: "dbo",
                columns: table => new
                {
                    CryptocurrencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BaseAsset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuoteAsset = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Cryptocurrency", x => x.CryptocurrencyId);
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
                    table.PrimaryKey("PK_Plan", x => x.PlanId);
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
                    table.PrimaryKey("PK_Role", x => x.RoleId);
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
                    table.PrimaryKey("PK_UserAccount", x => x.UserAccountId);
                    table.ForeignKey(
                        name: "FK_UserAccount_UserAccount_InvitedByUserId",
                        column: x => x.InvitedByUserId,
                        principalSchema: "dbo",
                        principalTable: "UserAccount",
                        principalColumn: "UserAccountId");
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
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CryptocurrencyId1 = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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
                    NumberOfTrades = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Candle_1d_Cryptocurrency_CryptocurrencyId1",
                        column: x => x.CryptocurrencyId1,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId");
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
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CryptocurrencyId1 = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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
                    NumberOfTrades = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Candle_1h_Cryptocurrency_CryptocurrencyId1",
                        column: x => x.CryptocurrencyId1,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId");
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
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CryptocurrencyId1 = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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
                    NumberOfTrades = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Candle_1m_Cryptocurrency_CryptocurrencyId1",
                        column: x => x.CryptocurrencyId1,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId");
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
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CryptocurrencyId1 = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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
                    NumberOfTrades = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Candle_4h_Cryptocurrency_CryptocurrencyId1",
                        column: x => x.CryptocurrencyId1,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId");
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
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CryptocurrencyId1 = table.Column<int>(type: "int", nullable: true),
                    ModifiedByIp = table.Column<string>(type: "char(15)", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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
                    NumberOfTrades = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Candle_5m_Cryptocurrency_CryptocurrencyId1",
                        column: x => x.CryptocurrencyId1,
                        principalSchema: "dbo",
                        principalTable: "Cryptocurrency",
                        principalColumn: "CryptocurrencyId");
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
                name: "IX_Candle_1d_CryptocurrencyId_OpenTime",
                schema: "dbo",
                table: "Candle_1d",
                columns: new[] { "CryptocurrencyId", "OpenTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candle_1d_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1d",
                column: "CryptocurrencyId1");

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
                name: "IX_Candle_1h_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1h",
                column: "CryptocurrencyId1");

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
                name: "IX_Candle_1m_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1m",
                column: "CryptocurrencyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Candle_1m_OpenTime",
                schema: "dbo",
                table: "Candle_1m",
                column: "OpenTime");

            migrationBuilder.CreateIndex(
                name: "IX_Candle_4h_CryptocurrencyId_OpenTime",
                schema: "dbo",
                table: "Candle_4h",
                columns: new[] { "CryptocurrencyId", "OpenTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candle_4h_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_4h",
                column: "CryptocurrencyId1");

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
                name: "IX_Candle_5m_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_5m",
                column: "CryptocurrencyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Candle_5m_OpenTime",
                schema: "dbo",
                table: "Candle_5m",
                column: "OpenTime");

            migrationBuilder.CreateIndex(
                name: "IX_Cryptocurrency_Symbol",
                schema: "dbo",
                table: "Cryptocurrency",
                column: "Symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserAccountId",
                schema: "dbo",
                table: "Notification",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanFeature_PlanId",
                schema: "dbo",
                table: "PlanFeature",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserAccountId",
                schema: "dbo",
                table: "RefreshToken",
                column: "UserAccountId");

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

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_InvitedByUserId",
                schema: "dbo",
                table: "UserAccount",
                column: "InvitedByUserId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "Candle_4h",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Candle_5m",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Notification",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PlanFeature",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RefreshToken",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SignalCandle",
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
                name: "Signal",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Plan",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserAccount",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Cryptocurrency",
                schema: "dbo");
        }
    }
}
