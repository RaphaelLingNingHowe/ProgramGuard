using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProgramGuard.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionLogs_AspNetUsers_UserId1",
                table: "ActionLogs");

            migrationBuilder.DropIndex(
                name: "IX_ActionLogs_UserId1",
                table: "ActionLogs");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "537e9744-a80e-4367-83a3-1866f1b6efe9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c64ad47c-94de-4f51-bfdc-23f93d093476");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ActionLogs");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ActionLogs");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "241647ae-d7db-4d0b-9404-4ad0509225c8", null, "User", "USER" },
                    { "4c973d7e-f4ea-4467-b1fc-1efdba765dea", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "241647ae-d7db-4d0b-9404-4ad0509225c8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4c973d7e-f4ea-4467-b1fc-1efdba765dea");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ActionLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "ActionLogs",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "537e9744-a80e-4367-83a3-1866f1b6efe9", null, "User", "USER" },
                    { "c64ad47c-94de-4f51-bfdc-23f93d093476", null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_UserId1",
                table: "ActionLogs",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionLogs_AspNetUsers_UserId1",
                table: "ActionLogs",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
