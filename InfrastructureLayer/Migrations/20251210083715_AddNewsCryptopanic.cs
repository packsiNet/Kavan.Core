using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddNewsCryptopanic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsInstrument",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "NewsPost",
                schema: "dbo");
        }
    }
}
