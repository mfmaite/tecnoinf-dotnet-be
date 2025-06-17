using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class AddTenantIdToServiceAvailabilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Agregar la columna TenantId a la tabla ServiceAvailabilities
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ServiceAvailabilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Actualizar los registros existentes para establecer el TenantId basado en el Service
            migrationBuilder.Sql(@"
                UPDATE ServiceAvailabilities
                SET TenantId = (
                    SELECT s.TenantId
                    FROM Services s
                    WHERE s.Id = ServiceAvailabilities.ServiceId
                )
            ");

            // Crear un índice para la columna TenantId
            migrationBuilder.CreateIndex(
                name: "IX_ServiceAvailabilities_TenantId",
                table: "ServiceAvailabilities",
                column: "TenantId");

            // Agregar la restricción de clave foránea
            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAvailabilities_Tenants_TenantId",
                table: "ServiceAvailabilities",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 17, 22, 6, 27, 818, DateTimeKind.Utc).AddTicks(9029), "f1KJtVaSVY3WVOCZhEnuS/lR3mgy3vV3asxrc1URgTU=", "n1w304zV4QpR6S2tVsIAyQ==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 17, 22, 6, 27, 818, DateTimeKind.Utc).AddTicks(9007), "f1KJtVaSVY3WVOCZhEnuS/lR3mgy3vV3asxrc1URgTU=", "n1w304zV4QpR6S2tVsIAyQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar la restricción de clave foránea
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAvailabilities_Tenants_TenantId",
                table: "ServiceAvailabilities");

            // Eliminar el índice
            migrationBuilder.DropIndex(
                name: "IX_ServiceAvailabilities_TenantId",
                table: "ServiceAvailabilities");

            // Eliminar la columna TenantId
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ServiceAvailabilities");

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
    }
}
