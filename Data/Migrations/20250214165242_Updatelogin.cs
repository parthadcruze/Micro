using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microbiology.Data.Migrations
{
    /// <inheritdoc />
    public partial class Updatelogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Login");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Login",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Login",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Login");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Login");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Login",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
