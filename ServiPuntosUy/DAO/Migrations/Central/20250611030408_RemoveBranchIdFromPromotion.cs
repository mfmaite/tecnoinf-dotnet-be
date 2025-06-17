using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class RemoveBranchIdFromPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Branches_BranchId",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_BranchId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Promotions");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 11, 3, 4, 7, 933, DateTimeKind.Utc).AddTicks(3430), "BVKIseYCO5V2wQGu3MJomKX1Qxhx7HQQGZvyOVsDBHQ=", "D9SvKJglirCvAIG7LY/oVw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 11, 3, 4, 7, 933, DateTimeKind.Utc).AddTicks(3400), "BVKIseYCO5V2wQGu3MJomKX1Qxhx7HQQGZvyOVsDBHQ=", "D9SvKJglirCvAIG7LY/oVw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Promotions",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 10, 23, 9, 29, 993, DateTimeKind.Utc).AddTicks(5340), "qq/R9RjIoeDqHnR8V6/hEX69GoymiJ+6oIM4MqhkCi8=", "m6dWUsemHMxzHOIp3E78qQ==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 10, 23, 9, 29, 993, DateTimeKind.Utc).AddTicks(5260), "qq/R9RjIoeDqHnR8V6/hEX69GoymiJ+6oIM4MqhkCi8=", "m6dWUsemHMxzHOIp3E78qQ==" });

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_BranchId",
                table: "Promotions",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Branches_BranchId",
                table: "Promotions",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");
        }
    }
}
