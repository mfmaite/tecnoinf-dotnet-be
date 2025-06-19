using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class RemoveGoogleIdAddIsGoogleUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsGoogleUser",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "IsGoogleUser", "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { false, new DateTime(2025, 6, 19, 1, 54, 13, 277, DateTimeKind.Utc).AddTicks(7750), "nfrui92FtvMJMR5ZUCQQxkDUxTfu+fF1dcIMlODoyv4=", "+B6MCaVIHXvhYymzRZNw+A==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "IsGoogleUser", "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { false, new DateTime(2025, 6, 19, 1, 54, 13, 277, DateTimeKind.Utc).AddTicks(7727), "nfrui92FtvMJMR5ZUCQQxkDUxTfu+fF1dcIMlODoyv4=", "+B6MCaVIHXvhYymzRZNw+A==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGoogleUser",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "GoogleId", "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { null, new DateTime(2025, 6, 16, 21, 43, 38, 667, DateTimeKind.Utc).AddTicks(7822), "Ed1N4omY8E7BAy0IOcMrNT3JtZe/ZV3/FVTML+wbN58=", "5Gedh+PkIxFsPEqAQleJqg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "GoogleId", "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { null, new DateTime(2025, 6, 16, 21, 43, 38, 667, DateTimeKind.Utc).AddTicks(7762), "Ed1N4omY8E7BAy0IOcMrNT3JtZe/ZV3/FVTML+wbN58=", "5Gedh+PkIxFsPEqAQleJqg==" });
        }
    }
}
