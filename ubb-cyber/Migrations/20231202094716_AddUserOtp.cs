using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubb_cyber.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Otp",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Otp",
                table: "Users");
        }
    }
}
