using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class UpdateUserSeedLastLoginDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 5, 28, 18, 43, 49, 337, DateTimeKind.Utc).AddTicks(5880), "vIxaoEQaRXF8lq6Hhh+b5N3S2Mz3BDviTzh53hiI/OA=", "TV32TVXeq4n01fZUsGBB9A==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 5, 28, 18, 43, 49, 337, DateTimeKind.Utc).AddTicks(5903), "vIxaoEQaRXF8lq6Hhh+b5N3S2Mz3BDviTzh53hiI/OA=", "TV32TVXeq4n01fZUsGBB9A==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 5, 28, 18, 11, 7, 817, DateTimeKind.Utc).AddTicks(4924), "my7pXmYAkcOxBxz8jPIDKWd8OM9BO0J8c4FlP0aoWtY=", "/H5m2GJ7ogp+2KaxtcQE2w==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 5, 28, 18, 11, 7, 817, DateTimeKind.Utc).AddTicks(4953), "my7pXmYAkcOxBxz8jPIDKWd8OM9BO0J8c4FlP0aoWtY=", "/H5m2GJ7ogp+2KaxtcQE2w==" });
        }
    }
}
