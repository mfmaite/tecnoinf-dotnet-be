using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class AddGoogleIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 14, 21, 10, 38, 453, DateTimeKind.Utc).AddTicks(328), "loq59Li4ednNlyXGYxyILXvqksuxlMJ7O6wodN7u/b4=", "AkaGhSjc+zM4kcTrHcp2+w==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 14, 21, 10, 38, 453, DateTimeKind.Utc).AddTicks(286), "loq59Li4ednNlyXGYxyILXvqksuxlMJ7O6wodN7u/b4=", "AkaGhSjc+zM4kcTrHcp2+w==" });
        }
    }
}
