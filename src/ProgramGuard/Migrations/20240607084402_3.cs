using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
namespace ProgramGuard.Migrations
{
    /// <inheritdoc />
    public partial class _3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b46b073c-79e4-4611-bd3f-90029b41df80", null, "User", "USER" },
                    { "b7efd0cb-3122-494a-99f7-688b761d4f12", null, "Admin", "ADMIN" }
                });
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b46b073c-79e4-4611-bd3f-90029b41df80");
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b7efd0cb-3122-494a-99f7-688b761d4f12");
        }
    }
}
