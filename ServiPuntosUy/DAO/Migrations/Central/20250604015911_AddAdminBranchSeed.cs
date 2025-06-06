using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class AddAdminBranchSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 4, 1, 59, 11, 368, DateTimeKind.Utc).AddTicks(9600), "3bmFvIrUxrqWXIYBbxE835ZffLJC0c4qJeZdHJMe250=", "8GpMFyuC9qfwBbrQvmiK1Q==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 4, 1, 59, 11, 368, DateTimeKind.Utc).AddTicks(9630), "3bmFvIrUxrqWXIYBbxE835ZffLJC0c4qJeZdHJMe250=", "8GpMFyuC9qfwBbrQvmiK1Q==" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "Email", "IsVerified", "LastLoginDate", "Name", "NotificationsEnabled", "Password", "PasswordSalt", "PointBalance", "Role", "TenantId" },
                values: new object[] { 3, 1, "adminAncap1@servipuntos.uy", false, new DateTime(2025, 6, 4, 1, 59, 11, 368, DateTimeKind.Utc).AddTicks(9650), "Admin branch", true, "3bmFvIrUxrqWXIYBbxE835ZffLJC0c4qJeZdHJMe250=", "8GpMFyuC9qfwBbrQvmiK1Q==", 0, 3, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

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
        }
    }
}
