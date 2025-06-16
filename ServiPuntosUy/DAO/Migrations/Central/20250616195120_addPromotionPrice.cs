using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class addPromotionPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Promotions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 16, 19, 51, 19, 648, DateTimeKind.Utc).AddTicks(4300), "HkpItQSRf7sgLGXpgTvQUfcGLhCWgJnxl6+l7S6vBWE=", "/YqS4KL1CJi31s09QJFWvA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 16, 19, 51, 19, 648, DateTimeKind.Utc).AddTicks(4270), "HkpItQSRf7sgLGXpgTvQUfcGLhCWgJnxl6+l7S6vBWE=", "/YqS4KL1CJi31s09QJFWvA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Promotions");

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
