using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class addMenuTree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    table.PrimaryKey("PK_CourseCategory", x => x.CourseCategoryId);
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseEnrollment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MediaFile",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Lesson",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Course",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CourseCategory",
                schema: "dbo");
        }
    }
}
