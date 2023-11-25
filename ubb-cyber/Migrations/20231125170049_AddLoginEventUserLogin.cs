using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubb_cyber.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginEventUserLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserLogin",
                table: "LoginEvents",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserLogin",
                table: "LoginEvents");
        }
    }
}
