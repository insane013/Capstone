using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListApp.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class Tags : Migration
    {
        private static readonly string[] Columns = new[] { "TodoListId", "Tag" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.CreateTable(
                name: "TaskTagEntity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TodoListId = table.Column<long>(type: "bigint", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(450)", nullable: false),
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_TaskTagEntity", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_TaskTagEntity_TodoLists_TodoListId",
                        column: x => x.TodoListId,
                        principalTable: "TodoLists",
                        principalColumn: "Id");
                });

            _ = migrationBuilder.CreateTable(
                name: "TaskTagEntityTodoTaskEntity",
                columns: table => new
                {
                    RelatedTasksId = table.Column<long>(type: "bigint", nullable: false),
                    TagsId = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_TaskTagEntityTodoTaskEntity", x => new { x.RelatedTasksId, x.TagsId });
                    _ = table.ForeignKey(
                        name: "FK_TaskTagEntityTodoTaskEntity_TaskTagEntity_TagsId",
                        column: x => x.TagsId,
                        principalTable: "TaskTagEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    _ = table.ForeignKey(
                        name: "FK_TaskTagEntityTodoTaskEntity_TodoTasks_RelatedTasksId",
                        column: x => x.RelatedTasksId,
                        principalTable: "TodoTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_TaskTagEntity_TodoListId_Tag",
                table: "TaskTagEntity",
                columns: Columns,
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_TaskTagEntityTodoTaskEntity_TagsId",
                table: "TaskTagEntityTodoTaskEntity",
                column: "TagsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.DropTable(
                name: "TaskTagEntityTodoTaskEntity");

            _ = migrationBuilder.DropTable(
                name: "TaskTagEntity");
        }
    }
}
