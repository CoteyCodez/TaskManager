using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInitUserTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TaskItems",
                columns: new[] { "Id", "AssignedToUserId", "CreatedAt", "CreatedById", "Description", "DueDate", "OrganizationId", "Status", "Title" },
                values: new object[] { 1, "e89b78c6-a35d-4c84-a40c-09ddd190f366", new DateTime(2026, 9, 21, 16, 2, 15, 599, DateTimeKind.Utc).AddTicks(4459), null, "This is a sample task", new DateTime(2026, 9, 28, 16, 2, 15, 599, DateTimeKind.Utc).AddTicks(4658), 1, "Assigned", "Sample Task" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
