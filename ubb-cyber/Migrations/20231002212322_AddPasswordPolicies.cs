using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubb_cyber.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OverrideMinPasswordLength",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OverrideNumbersCount",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OverridePasswordExpireDays",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OverrideUppercaseCount",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordExpireDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PasswordPolicies",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MinPasswordLength = table.Column<int>(type: "int", nullable: false),
                    PasswordExpireDays = table.Column<int>(type: "int", nullable: false),
                    UppercaseCount = table.Column<int>(type: "int", nullable: false),
                    NumbersCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordPolicies", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "UsedPasswords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedPasswords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsedPasswords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsedPasswords_UserId",
                table: "UsedPasswords",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordPolicies");

            migrationBuilder.DropTable(
                name: "UsedPasswords");

            migrationBuilder.DropColumn(
                name: "OverrideMinPasswordLength",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OverrideNumbersCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OverridePasswordExpireDays",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OverrideUppercaseCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordExpireDate",
                table: "Users");
        }
    }
}
