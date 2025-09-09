using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListApp.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class TagsRenameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.DropForeignKey(
                name: "FK_TaskTagEntity_TodoLists_TodoListId",
                table: "TaskTagEntity");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_TaskTagEntityTodoTaskEntity_TaskTagEntity_TagsId",
                table: "TaskTagEntityTodoTaskEntity");

            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_TaskTagEntity",
                table: "TaskTagEntity");

            _ = migrationBuilder.RenameTable(
                name: "TaskTagEntity",
                newName: "Tags");

            _ = migrationBuilder.RenameIndex(
                name: "IX_TaskTagEntity_TodoListId_Tag",
                table: "Tags",
                newName: "IX_Tags_TodoListId_Tag");

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Tags_TodoLists_TodoListId",
                table: "Tags",
                column: "TodoListId",
                principalTable: "TodoLists",
                principalColumn: "Id");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_TaskTagEntityTodoTaskEntity_Tags_TagsId",
                table: "TaskTagEntityTodoTaskEntity",
                column: "TagsId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.DropForeignKey(
                name: "FK_Tags_TodoLists_TodoListId",
                table: "Tags");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_TaskTagEntityTodoTaskEntity_Tags_TagsId",
                table: "TaskTagEntityTodoTaskEntity");

            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            _ = migrationBuilder.RenameTable(
                name: "Tags",
                newName: "TaskTagEntity");

            _ = migrationBuilder.RenameIndex(
                name: "IX_Tags_TodoListId_Tag",
                table: "TaskTagEntity",
                newName: "IX_TaskTagEntity_TodoListId_Tag");

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_TaskTagEntity",
                table: "TaskTagEntity",
                column: "Id");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_TaskTagEntity_TodoLists_TodoListId",
                table: "TaskTagEntity",
                column: "TodoListId",
                principalTable: "TodoLists",
                principalColumn: "Id");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_TaskTagEntityTodoTaskEntity_TaskTagEntity_TagsId",
                table: "TaskTagEntityTodoTaskEntity",
                column: "TagsId",
                principalTable: "TaskTagEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
