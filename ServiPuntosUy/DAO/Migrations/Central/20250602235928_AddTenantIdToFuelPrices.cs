using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class AddTenantIdToFuelPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "FuelPrices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 2, 23, 59, 27, 956, DateTimeKind.Utc).AddTicks(3039), "f5s5+wHlTLd1sHhsb+dXxInF6R2MEGqxUNNS0fW1yeE=", "EOZyaEf/9OL6EtO7lJrytg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 2, 23, 59, 27, 956, DateTimeKind.Utc).AddTicks(3062), "f5s5+wHlTLd1sHhsb+dXxInF6R2MEGqxUNNS0fW1yeE=", "EOZyaEf/9OL6EtO7lJrytg==" });

            // Actualizar los registros existentes para establecer el TenantId basado en el Branch
            migrationBuilder.Sql(@"
                UPDATE FuelPrices
                SET TenantId = (
                    SELECT b.TenantId
                    FROM Branches b
                    WHERE b.Id = FuelPrices.BranchId
                )
            ");

            migrationBuilder.CreateIndex(
                name: "IX_FuelPrices_TenantId",
                table: "FuelPrices",
                column: "TenantId");

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

            migrationBuilder.DropIndex(
                name: "IX_FuelPrices_TenantId",
                table: "FuelPrices");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FuelPrices");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 5, 28, 18, 43, 49, 337, DateTimeKind.Utc).AddTicks(5880), "UjRJ6aZDtLSNtS4Z2iNavTbYI2QIjVviCTuggKJpfso=", "JCfmyZickAH/jRYWkSuL9w==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 5, 28, 18, 43, 49, 337, DateTimeKind.Utc).AddTicks(5903), "UjRJ6aZDtLSNtS4Z2iNavTbYI2QIjVviCTuggKJpfso=", "JCfmyZickAH/jRYWkSuL9w==" });
        }
    }
}
