using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListApp.Services.Database.Migrations.UserDb
{
    /// <inheritdoc />
    public partial class AddMoreUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty);

            _ = migrationBuilder.AddColumn<string>(
                name: "UniqueTag",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: string.Empty);

            _ = migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UniqueTag",
                table: "AspNetUsers",
                column: "UniqueTag",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder?.DropIndex(
                name: "IX_AspNetUsers_UniqueTag",
                table: "AspNetUsers");

            _ = migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            _ = migrationBuilder.DropColumn(
                name: "UniqueTag",
                table: "AspNetUsers");
        }
    }
}
