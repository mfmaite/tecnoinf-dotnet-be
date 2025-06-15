using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class AddGeneralParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralParameters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralParameters", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "GeneralParameters",
                columns: new[] { "Id", "Description", "Key", "Value" },
                values: new object[] { -1, "Moneda por defecto para la aplicación", "Currency", "USD" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralParameters");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 14, 2, 42, 4, 606, DateTimeKind.Utc).AddTicks(6708), "DunZ2vxyToxjhIPT2Ya9ci6moBUR4+Y+FqHAvMrPn18=", "sQL2cXAYerKlPtrXl+nHjA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 14, 2, 42, 4, 606, DateTimeKind.Utc).AddTicks(6679), "DunZ2vxyToxjhIPT2Ya9ci6moBUR4+Y+FqHAvMrPn18=", "sQL2cXAYerKlPtrXl+nHjA==" });
        }
    }
}
