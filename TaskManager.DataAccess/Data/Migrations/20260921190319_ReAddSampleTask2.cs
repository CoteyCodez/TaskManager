using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReAddSampleTask2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "TaskItems",
                columns: new[] { "Id", "AssignedToUserId", "CreatedAt", "CreatedById", "Description", "DueDate", "OrganizationId", "Status", "Title" },
                values: new object[] { 5, "e89b78c6-a35d-4c84-a40c-09ddd190f366", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "This is a sample task again", new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Assigned", "Sample Task Again" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.InsertData(
                table: "TaskItems",
                columns: new[] { "Id", "AssignedToUserId", "CreatedAt", "CreatedById", "Description", "DueDate", "OrganizationId", "Status", "Title" },
                values: new object[] { 1, "e89b78c6-a35d-4c84-a40c-09ddd190f366", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "This is a sample task", new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Assigned", "Sample Task" });
        }
    }
}
