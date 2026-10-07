using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPrivateTaskTargetId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrivateTaskTargetId",
                table: "TaskItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrivateTaskTargetId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_PrivateTaskTargetId",
                table: "TaskItems",
                column: "PrivateTaskTargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_AspNetUsers_PrivateTaskTargetId",
                table: "TaskItems",
                column: "PrivateTaskTargetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_AspNetUsers_PrivateTaskTargetId",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_PrivateTaskTargetId",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "PrivateTaskTargetId",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "PrivateTaskTargetId",
                table: "AspNetUsers");
        }
    }
}
