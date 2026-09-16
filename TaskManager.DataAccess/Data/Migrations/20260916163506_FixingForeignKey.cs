using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixingForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_AspNetUsers_AssignedToId",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_AssignedToId",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "TaskItems");

            migrationBuilder.AlterColumn<string>(
                name: "AssignedToUserId",
                table: "TaskItems",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_AssignedToUserId",
                table: "TaskItems",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_AspNetUsers_AssignedToUserId",
                table: "TaskItems",
                column: "AssignedToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_AspNetUsers_AssignedToUserId",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_AssignedToUserId",
                table: "TaskItems");

            migrationBuilder.AlterColumn<string>(
                name: "AssignedToUserId",
                table: "TaskItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedToId",
                table: "TaskItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_AssignedToId",
                table: "TaskItems",
                column: "AssignedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_AspNetUsers_AssignedToId",
                table: "TaskItems",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
