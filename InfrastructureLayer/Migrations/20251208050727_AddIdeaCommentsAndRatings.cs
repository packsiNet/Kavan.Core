using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddIdeaCommentsAndRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdeaComment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "IdeaRating",
                schema: "dbo");
        }
    }
}
