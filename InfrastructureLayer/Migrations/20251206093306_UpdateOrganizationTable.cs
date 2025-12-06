using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrganizationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutMe",
                schema: "dbo",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionDetails",
                schema: "dbo",
                table: "OrganizationProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                schema: "dbo",
                table: "OrganizationProfile",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationEmail_OrganizationProfileId",
                schema: "dbo",
                table: "OrganizationEmail",
                column: "OrganizationProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPhone_OrganizationProfileId",
                schema: "dbo",
                table: "OrganizationPhone",
                column: "OrganizationProfileId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationEmail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrganizationPhone",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserFollow",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "AboutMe",
                schema: "dbo",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "DescriptionDetails",
                schema: "dbo",
                table: "OrganizationProfile");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                schema: "dbo",
                table: "OrganizationProfile");
        }
    }
}
