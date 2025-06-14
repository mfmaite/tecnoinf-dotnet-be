using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class AddTransactionTypeAndPointsSpent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PointsSpent",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PointsSpent",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Transactions");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 11, 1, 24, 13, 730, DateTimeKind.Utc).AddTicks(7350), "L632D3r3mrtdniiprj//8pU4ZTmHB+UqxX18BvEYhNI=", "u4q07UGIRLTEFm8WJe6gXw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 11, 1, 24, 13, 730, DateTimeKind.Utc).AddTicks(7230), "L632D3r3mrtdniiprj//8pU4ZTmHB+UqxX18BvEYhNI=", "u4q07UGIRLTEFm8WJe6gXw==" });
        }
    }
}
