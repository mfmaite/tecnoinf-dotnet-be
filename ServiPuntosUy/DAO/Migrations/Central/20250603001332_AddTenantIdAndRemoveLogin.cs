using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class AddTenantIdAndRemoveLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 3, 0, 13, 32, 18, DateTimeKind.Utc).AddTicks(3031), "g1j0f6iAH7aRmA2ahGayJdwsPp1EovB936txv6SwP2w=", "vw3EjDkilPNreOEjDmPmyw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 3, 0, 13, 32, 18, DateTimeKind.Utc).AddTicks(3062), "g1j0f6iAH7aRmA2ahGayJdwsPp1EovB936txv6SwP2w=", "vw3EjDkilPNreOEjDmPmyw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 3, 0, 12, 37, 344, DateTimeKind.Utc).AddTicks(3214), "ao8rDR2Tn9cMw8/hIIQRmbVpxrsXaAL+vK+0Nm57/xM=", "ICv7/cxHUNbnjHcKxjgT5A==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 3, 0, 12, 37, 344, DateTimeKind.Utc).AddTicks(3238), "ao8rDR2Tn9cMw8/hIIQRmbVpxrsXaAL+vK+0Nm57/xM=", "ICv7/cxHUNbnjHcKxjgT5A==" });
        }
    }
}
