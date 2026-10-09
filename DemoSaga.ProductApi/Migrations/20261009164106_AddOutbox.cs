using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DemoSaga.ProductService.Migrations
{
    /// <inheritdoc />
    public partial class AddOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("4a19d49a-4500-48ca-889a-ddf40fda4a5b"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("4bc897b3-cc6f-471a-9610-5174a792cbe2"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("6144efba-4775-4a06-8809-15504cd2adee"));

            migrationBuilder.CreateTable(
                name: "ProductOutBoxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOutBoxMessages", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BasePrice", "Name", "Quantity" },
                values: new object[,]
                {
                    { new Guid("3668f1b6-e7c9-40c7-978e-647f72e656ad"), 200m, "Product 2", 200 },
                    { new Guid("7ad2b48d-63ff-47ff-b2e7-a149554d4489"), 700m, "Product 3", 300 },
                    { new Guid("de863de8-d7be-4b74-a2ac-fe58e73cb462"), 1000m, "Product 1", 100 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductOutBoxMessages_CreatedAtUtc_PublishedAtUtc",
                table: "ProductOutBoxMessages",
                columns: new[] { "CreatedAtUtc", "PublishedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductOutBoxMessages");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("3668f1b6-e7c9-40c7-978e-647f72e656ad"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("7ad2b48d-63ff-47ff-b2e7-a149554d4489"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("de863de8-d7be-4b74-a2ac-fe58e73cb462"));

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BasePrice", "Name", "Quantity" },
                values: new object[,]
                {
                    { new Guid("4a19d49a-4500-48ca-889a-ddf40fda4a5b"), 200m, "Product 2", 200 },
                    { new Guid("4bc897b3-cc6f-471a-9610-5174a792cbe2"), 1000m, "Product 1", 100 },
                    { new Guid("6144efba-4775-4a06-8809-15504cd2adee"), 700m, "Product 3", 300 }
                });
        }
    }
}
