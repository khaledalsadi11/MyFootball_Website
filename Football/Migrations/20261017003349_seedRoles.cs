using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Football.Migrations
{
    /// <inheritdoc />
    public partial class seedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a97b2105-51d1-4e23-a94a-6b8d472a3c01", "a97b2105-51d1-4e23-a94a-6b8d472a3c01", "User", "USER" },
                    { "a97b2105-51d1-4e23-a94a-6b8d472a3c02", "a97b2105-51d1-4e23-a94a-6b8d472a3c02", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a97b2105-51d1-4e23-a94a-6b8d472a3c01");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a97b2105-51d1-4e23-a94a-6b8d472a3c02");
        }
    }
}
