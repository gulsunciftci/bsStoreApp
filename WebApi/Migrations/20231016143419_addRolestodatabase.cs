using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addRolestodatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "398c44c7-0767-45a4-b029-7ca9239e30f6", "eeff2c20-72a1-44a3-9e5f-a48a7092a092", "Editor", "EDITOR" },
                    { "3ab5bc42-cc7f-4680-8a8e-a0b62b9c0fe2", "0c277005-b04c-4fa5-8e29-0c9064dcc7cf", "Admin", "ADMIN" },
                    { "7449fc59-acfe-4b9b-a4af-33b19c5f860a", "b12f57ec-bcb8-4b28-91bb-f04f758ba878", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "398c44c7-0767-45a4-b029-7ca9239e30f6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3ab5bc42-cc7f-4680-8a8e-a0b62b9c0fe2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7449fc59-acfe-4b9b-a4af-33b19c5f860a");
        }
    }
}
