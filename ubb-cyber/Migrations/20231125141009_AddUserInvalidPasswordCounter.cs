using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubb_cyber.Migrations
{
    /// <inheritdoc />
    public partial class AddUserInvalidPasswordCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvalidPasswordCounter",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvalidPasswordCounter",
                table: "Users");
        }
    }
}
