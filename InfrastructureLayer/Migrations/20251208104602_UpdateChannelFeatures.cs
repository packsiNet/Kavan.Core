using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChannelFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "dbo",
                table: "Channel",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Category",
                schema: "dbo",
                table: "Channel",
                newName: "UniqueCode");

            migrationBuilder.AddColumn<int>(
                name: "CommentsCount",
                schema: "dbo",
                table: "ChannelPost",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikesCount",
                schema: "dbo",
                table: "ChannelPost",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsMuted",
                schema: "dbo",
                table: "ChannelMembership",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "AccessType",
                schema: "dbo",
                table: "Channel",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                schema: "dbo",
                table: "Channel",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                schema: "dbo",
                table: "Channel",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                schema: "dbo",
                table: "Channel",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                schema: "dbo",
                table: "Channel",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "dbo",
                table: "Channel",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Channel_UniqueCode",
                schema: "dbo",
                table: "Channel",
                column: "UniqueCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelPostComment_PostId",
                schema: "dbo",
                table: "ChannelPostComment",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelPostComment",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Channel_UniqueCode",
                schema: "dbo",
                table: "Channel");

            migrationBuilder.DropColumn(
                name: "CommentsCount",
                schema: "dbo",
                table: "ChannelPost");

            migrationBuilder.DropColumn(
                name: "LikesCount",
                schema: "dbo",
                table: "ChannelPost");

            migrationBuilder.DropColumn(
                name: "IsMuted",
                schema: "dbo",
                table: "ChannelMembership");

            migrationBuilder.DropColumn(
                name: "BannerUrl",
                schema: "dbo",
                table: "Channel");

            migrationBuilder.DropColumn(
                name: "Currency",
                schema: "dbo",
                table: "Channel");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                schema: "dbo",
                table: "Channel");

            migrationBuilder.DropColumn(
                name: "Price",
                schema: "dbo",
                table: "Channel");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "dbo",
                table: "Channel");

            migrationBuilder.RenameColumn(
                name: "UniqueCode",
                schema: "dbo",
                table: "Channel",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "Title",
                schema: "dbo",
                table: "Channel",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "AccessType",
                schema: "dbo",
                table: "Channel",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
