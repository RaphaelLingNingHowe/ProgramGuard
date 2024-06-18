using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgramGuard.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoginTime",
                table: "AspNetUsers",
                newName: "LastLoginTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastLoginTime",
                table: "AspNetUsers",
                newName: "LoginTime");
        }
    }
}
