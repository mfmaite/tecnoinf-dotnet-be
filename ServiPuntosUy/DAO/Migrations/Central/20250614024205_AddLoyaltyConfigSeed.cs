using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class AddLoyaltyConfigSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LoyaltyConfigs",
                columns: new[] { "Id", "AccumulationRule", "ExpiricyPolicyDays", "PointsName", "PointsValue", "TenantId" },
                values: new object[] { -1, 100m, 180, "Puntos", 1, -1 });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LoyaltyConfigs",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 13, 23, 30, 35, 858, DateTimeKind.Utc).AddTicks(7343), "2GZKzZMQle5c7hbbuOWb6711Qrl1+i6oBhE3w04j7vo=", "4fEENXLtRFJzwZnfLpnA4Q==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 13, 23, 30, 35, 858, DateTimeKind.Utc).AddTicks(7306), "2GZKzZMQle5c7hbbuOWb6711Qrl1+i6oBhE3w04j7vo=", "4fEENXLtRFJzwZnfLpnA4Q==" });
        }
    }
}
