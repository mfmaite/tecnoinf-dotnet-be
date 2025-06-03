using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class FixFuelPricesTenantRelationOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelPrices_Tenants_TenantId",
                table: "FuelPrices");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 3, 2, 2, 54, 429, DateTimeKind.Utc).AddTicks(3362), "tC2nAzwdxkNHCWrDUO8VFRgJy7C+EwbwFQ7SXioXOFk=", "t6k1G133aTxqgFq0eTwAVg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 3, 2, 2, 54, 429, DateTimeKind.Utc).AddTicks(3390), "tC2nAzwdxkNHCWrDUO8VFRgJy7C+EwbwFQ7SXioXOFk=", "t6k1G133aTxqgFq0eTwAVg==" });

            migrationBuilder.AddForeignKey(
                name: "FK_FuelPrices_Tenants_TenantId",
                table: "FuelPrices",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelPrices_Tenants_TenantId",
                table: "FuelPrices");

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

            migrationBuilder.AddForeignKey(
                name: "FK_FuelPrices_Tenants_TenantId",
                table: "FuelPrices",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
