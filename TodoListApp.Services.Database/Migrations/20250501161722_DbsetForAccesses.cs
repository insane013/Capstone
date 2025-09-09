using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListApp.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class DbsetForAccesses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.DropForeignKey(
                name: "FK_UserTodoAccess_TodoLists_TodoListId",
                table: "UserTodoAccess");

            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_UserTodoAccess",
                table: "UserTodoAccess");

            _ = migrationBuilder.RenameTable(
                name: "UserTodoAccess",
                newName: "Accesses");

            _ = migrationBuilder.RenameIndex(
                name: "IX_UserTodoAccess_TodoListId",
                table: "Accesses",
                newName: "IX_Accesses_TodoListId");

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_Accesses",
                table: "Accesses",
                column: "UserId");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Accesses_TodoLists_TodoListId",
                table: "Accesses",
                column: "TodoListId",
                principalTable: "TodoLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.DropForeignKey(
                name: "FK_Accesses_TodoLists_TodoListId",
                table: "Accesses");

            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_Accesses",
                table: "Accesses");

            _ = migrationBuilder.RenameTable(
                name: "Accesses",
                newName: "UserTodoAccess");

            _ = migrationBuilder.RenameIndex(
                name: "IX_Accesses_TodoListId",
                table: "UserTodoAccess",
                newName: "IX_UserTodoAccess_TodoListId");

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_UserTodoAccess",
                table: "UserTodoAccess",
                column: "UserId");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_UserTodoAccess_TodoLists_TodoListId",
                table: "UserTodoAccess",
                column: "TodoListId",
                principalTable: "TodoLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
