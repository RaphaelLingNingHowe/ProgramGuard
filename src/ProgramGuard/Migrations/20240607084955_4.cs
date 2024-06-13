using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
namespace ProgramGuard.Migrations
{
    /// <inheritdoc />
    public partial class _4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_AppUserId",
                table: "AspNetUserRoles");
            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_AppUserId",
                table: "AspNetUserRoles");
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b46b073c-79e4-4611-bd3f-90029b41df80");
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b7efd0cb-3122-494a-99f7-688b761d4f12");
            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "AspNetUserRoles");
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "57dac402-3fd8-4568-9033-10f00dbfe2cd", null, "Admin", "ADMIN" },
                    { "6c607313-3b6a-4eca-ac97-e4a7a6decf57", null, "User", "USER" }
                });
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "57dac402-3fd8-4568-9033-10f00dbfe2cd");
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6c607313-3b6a-4eca-ac97-e4a7a6decf57");
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "AspNetUserRoles",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b46b073c-79e4-4611-bd3f-90029b41df80", null, "User", "USER" },
                    { "b7efd0cb-3122-494a-99f7-688b761d4f12", null, "Admin", "ADMIN" }
                });
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_AppUserId",
                table: "AspNetUserRoles",
                column: "AppUserId");
            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_AppUserId",
                table: "AspNetUserRoles",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
