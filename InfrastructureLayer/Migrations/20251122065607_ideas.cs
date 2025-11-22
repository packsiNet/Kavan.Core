using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class ideas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Signal_CryptocurrencyId",
                schema: "dbo",
                table: "Signal");

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
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_Idea", x => x.IdeaId);
                });

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
                name: "IX_Idea_IsPublic",
                schema: "dbo",
                table: "Idea",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_Idea_Symbol_Timeframe",
                schema: "dbo",
                table: "Idea",
                columns: new[] { "Symbol", "Timeframe" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Idea",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Signal_CryptocurrencyId_SignalTime",
                schema: "dbo",
                table: "Signal");

            migrationBuilder.DropIndex(
                name: "IX_Signal_Symbol_Timeframe_SignalCategory_SignalName_Direction_SignalTime",
                schema: "dbo",
                table: "Signal");

            migrationBuilder.CreateIndex(
                name: "IX_Signal_CryptocurrencyId",
                schema: "dbo",
                table: "Signal",
                column: "CryptocurrencyId");
        }
    }
}
