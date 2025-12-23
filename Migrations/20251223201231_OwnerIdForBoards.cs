using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskr.Migrations
{
    /// <inheritdoc />
    public partial class OwnerIdForBoards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Boards",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Boards");
        }
    }
}
