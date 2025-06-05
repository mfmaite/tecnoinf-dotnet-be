using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class AddTenantUISeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "Name" },
                values: new object[] { -1, "ancap" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "Email", "IsVerified", "LastLoginDate", "Name", "NotificationsEnabled", "Password", "PasswordSalt", "PointBalance", "Role", "TenantId" },
                values: new object[] { -1, null, "admin@servipuntos.uy", false, new DateTime(2025, 6, 5, 0, 59, 43, 91, DateTimeKind.Utc).AddTicks(4244), "Admin Central", true, "74lI/359Ugnlj9bBElG6b9ggs9qdkIm6S1lsMt4IqYM=", "yGzQElt+vx7mngS52PM05A==", 0, 1, null });

            migrationBuilder.InsertData(
                table: "TenantUIs",
                columns: new[] { "Id", "LogoUrl", "PrimaryColor", "SecondaryColor", "TenantId" },
                values: new object[] { -1, "https://example.com/logo-ancap.png", "#0000FF", "#FFFF00", -1 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "Email", "IsVerified", "LastLoginDate", "Name", "NotificationsEnabled", "Password", "PasswordSalt", "PointBalance", "Role", "TenantId" },
                values: new object[] { -2, null, "admintenant@servipuntos.uy", false, new DateTime(2025, 6, 5, 0, 59, 43, 91, DateTimeKind.Utc).AddTicks(4268), "Admin Tenant", true, "74lI/359Ugnlj9bBElG6b9ggs9qdkIm6S1lsMt4IqYM=", "yGzQElt+vx7mngS52PM05A==", 0, 2, -1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TenantUIs",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "ancap" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "Email", "IsVerified", "LastLoginDate", "Name", "NotificationsEnabled", "Password", "PasswordSalt", "PointBalance", "Role", "TenantId" },
                values: new object[,]
                {
                    { 1, null, "admin@servipuntos.uy", false, new DateTime(2025, 6, 3, 2, 2, 54, 429, DateTimeKind.Utc).AddTicks(3362), "Admin Central", true, "tC2nAzwdxkNHCWrDUO8VFRgJy7C+EwbwFQ7SXioXOFk=", "t6k1G133aTxqgFq0eTwAVg==", 0, 1, null },
                    { 2, null, "admintenant@servipuntos.uy", false, new DateTime(2025, 6, 3, 2, 2, 54, 429, DateTimeKind.Utc).AddTicks(3390), "Admin Tenant", true, "tC2nAzwdxkNHCWrDUO8VFRgJy7C+EwbwFQ7SXioXOFk=", "t6k1G133aTxqgFq0eTwAVg==", 0, 2, 1 }
                });
        }
    }
}
