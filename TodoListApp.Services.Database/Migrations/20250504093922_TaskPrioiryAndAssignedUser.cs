using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListApp.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class TaskPrioiryAndAssignedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.AddColumn<string>(
                name: "AssignedUserId",
                table: "TodoTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty);

            _ = migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "TodoTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.DropColumn(
                name: "AssignedUserId",
                table: "TodoTasks");

            _ = migrationBuilder.DropColumn(
                name: "Priority",
                table: "TodoTasks");
        }
    }
}
