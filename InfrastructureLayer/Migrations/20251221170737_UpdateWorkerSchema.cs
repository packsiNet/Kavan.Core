using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWorkerSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropForeignKey(
                name: "FK_Candle_1d_Cryptocurrency_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1d");

            migrationBuilder.DropForeignKey(
                name: "FK_Candle_1h_Cryptocurrency_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1h");

            migrationBuilder.DropForeignKey(
                name: "FK_Candle_1m_Cryptocurrency_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1m");

            migrationBuilder.DropForeignKey(
                name: "FK_Candle_4h_Cryptocurrency_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_4h");

            migrationBuilder.DropForeignKey(
                name: "FK_Candle_5m_Cryptocurrency_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_5m");

            migrationBuilder.DropIndex(
                name: "IX_Candle_5m_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_5m");

            migrationBuilder.DropIndex(
                name: "IX_Candle_4h_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_4h");

            migrationBuilder.DropIndex(
                name: "IX_Candle_1m_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1m");

            migrationBuilder.DropIndex(
                name: "IX_Candle_1h_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1h");

            migrationBuilder.DropIndex(
                name: "IX_Candle_1d_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1d");

            migrationBuilder.DropColumn(
                name: "CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_5m");

            migrationBuilder.DropColumn(
                name: "CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_4h");

            migrationBuilder.DropColumn(
                name: "CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1m");

            migrationBuilder.DropColumn(
                name: "CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1h");

            migrationBuilder.DropColumn(
                name: "CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1d");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                schema: "dbo",
                table: "Cryptocurrency",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            /*
            migrationBuilder.AddColumn<bool>(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_5m",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_5m",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_4h",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_4h",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_1w",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_1w",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_1m",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_1m",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_1h",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_1h",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_1d",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_1d",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_15m",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_15m",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_5m");

            migrationBuilder.DropColumn(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_5m");

            migrationBuilder.DropColumn(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_4h");

            migrationBuilder.DropColumn(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_4h");

            migrationBuilder.DropColumn(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_1w");

            migrationBuilder.DropColumn(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_1w");

            migrationBuilder.DropColumn(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_1m");

            migrationBuilder.DropColumn(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_1m");

            migrationBuilder.DropColumn(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_1h");

            migrationBuilder.DropColumn(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_1h");

            migrationBuilder.DropColumn(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_1d");

            migrationBuilder.DropColumn(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_1d");

            migrationBuilder.DropColumn(
                name: "IsFinal",
                schema: "dbo",
                table: "Candle_15m");

            migrationBuilder.DropColumn(
                name: "LastUpdatedUtc",
                schema: "dbo",
                table: "Candle_15m");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                schema: "dbo",
                table: "Cryptocurrency",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_5m",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_4h",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1m",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1h",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1d",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candle_5m_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_5m",
                column: "CryptocurrencyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Candle_4h_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_4h",
                column: "CryptocurrencyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Candle_1m_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1m",
                column: "CryptocurrencyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Candle_1h_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1h",
                column: "CryptocurrencyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Candle_1d_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1d",
                column: "CryptocurrencyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Candle_1d_Cryptocurrency_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1d",
                column: "CryptocurrencyId1",
                principalSchema: "dbo",
                principalTable: "Cryptocurrency",
                principalColumn: "CryptocurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candle_1h_Cryptocurrency_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1h",
                column: "CryptocurrencyId1",
                principalSchema: "dbo",
                principalTable: "Cryptocurrency",
                principalColumn: "CryptocurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candle_1m_Cryptocurrency_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_1m",
                column: "CryptocurrencyId1",
                principalSchema: "dbo",
                principalTable: "Cryptocurrency",
                principalColumn: "CryptocurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candle_4h_Cryptocurrency_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_4h",
                column: "CryptocurrencyId1",
                principalSchema: "dbo",
                principalTable: "Cryptocurrency",
                principalColumn: "CryptocurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candle_5m_Cryptocurrency_CryptocurrencyId1",
                schema: "dbo",
                table: "Candle_5m",
                column: "CryptocurrencyId1",
                principalSchema: "dbo",
                principalTable: "Cryptocurrency",
                principalColumn: "CryptocurrencyId");
        }
    }
}
