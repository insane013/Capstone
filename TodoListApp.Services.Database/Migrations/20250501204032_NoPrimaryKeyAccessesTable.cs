using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListApp.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class NoPrimaryKeyAccessesTable : Migration
    {
        private static readonly string[] Columns = new[] { "UserId", "TodoListId" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.DropPrimaryKey(
                name: "PK_Accesses",
                table: "Accesses");

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_Accesses",
                table: "Accesses",
                columns: Columns);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.DropPrimaryKey(
                name: "PK_Accesses",
                table: "Accesses");

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_Accesses",
                table: "Accesses",
                column: "UserId");
        }
    }
}
