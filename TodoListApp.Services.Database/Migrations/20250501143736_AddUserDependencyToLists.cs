using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListApp.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDependencyToLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.AddColumn<string>(
                name: "OwnerId",
                table: "TodoLists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty);

            _ = migrationBuilder.CreateTable(
                name: "UserTodoAccess",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TodoListId = table.Column<long>(type: "bigint", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_UserTodoAccess", x => x.UserId);
                    _ = table.ForeignKey(
                        name: "FK_UserTodoAccess_TodoLists_TodoListId",
                        column: x => x.TodoListId,
                        principalTable: "TodoLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_UserTodoAccess_TodoListId",
                table: "UserTodoAccess",
                column: "TodoListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.DropTable(
                name: "UserTodoAccess");

            _ = migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "TodoLists");
        }
    }
}
