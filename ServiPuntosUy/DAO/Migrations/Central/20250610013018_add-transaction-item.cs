using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class addtransactionitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 10, 1, 30, 18, 251, DateTimeKind.Utc).AddTicks(370), "1OlPQoK3YjJ87wZxZ/F06R2J4GrBVxCVKTWBsLYJlss=", "uMJb7QtXTawWgAgIB7Gw/g==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 10, 1, 30, 18, 251, DateTimeKind.Utc).AddTicks(340), "1OlPQoK3YjJ87wZxZ/F06R2J4GrBVxCVKTWBsLYJlss=", "uMJb7QtXTawWgAgIB7Gw/g==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 10, 0, 25, 43, 830, DateTimeKind.Utc).AddTicks(1750), "F2arxYh7A7tiZWrCa+Q6aBA4f9DoBxLw4ShVSrVSLRI=", "PxdBuujGhYL6cBpgNUo81A==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 10, 0, 25, 43, 830, DateTimeKind.Utc).AddTicks(1720), "F2arxYh7A7tiZWrCa+Q6aBA4f9DoBxLw4ShVSrVSLRI=", "PxdBuujGhYL6cBpgNUo81A==" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "Email", "IsVerified", "LastLoginDate", "Name", "NotificationsEnabled", "Password", "PasswordSalt", "PointBalance", "Role", "TenantId" },
                values: new object[] { -3, 1, "adminAncap1@servipuntos.uy", false, new DateTime(2025, 6, 10, 0, 25, 43, 830, DateTimeKind.Utc).AddTicks(1770), "Admin branch", true, "F2arxYh7A7tiZWrCa+Q6aBA4f9DoBxLw4ShVSrVSLRI=", "PxdBuujGhYL6cBpgNUo81A==", 0, 3, 1 });
        }
    }
}
