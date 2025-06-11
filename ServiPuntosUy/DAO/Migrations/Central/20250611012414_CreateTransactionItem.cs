using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class CreateTransactionItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Set TenantId = NULL for all users referencing TenantId = 1
            migrationBuilder.Sql(@"UPDATE Users SET TenantId = NULL WHERE TenantId = 1");

            // Now we can safely delete the users
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            // Now we can safely delete the tenant
            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "Id",
                keyValue: 1);

            // Check if the foreign key constraints exist before trying to drop them
            var productStocksFkExists = migrationBuilder.Sql(
                "SELECT COUNT(*) FROM sys.foreign_keys WHERE name = 'FK_ProductStocks_Tenants_TenantId'").ToString() == "1";
            var transactionsFkExists = migrationBuilder.Sql(
                "SELECT COUNT(*) FROM sys.foreign_keys WHERE name = 'FK_Transactions_Tenants_TenantId'").ToString() == "1";

            if (productStocksFkExists)
            {
                migrationBuilder.DropForeignKey(
                    name: "FK_ProductStocks_Tenants_TenantId",
                    table: "ProductStocks");
            }

            if (transactionsFkExists)
            {
                migrationBuilder.DropForeignKey(
                    name: "FK_Transactions_Tenants_TenantId",
                    table: "Transactions");
            }

            // Check if the indexes exist before trying to drop them
            var transactionsIndexExists = migrationBuilder.Sql(
                "SELECT COUNT(*) FROM sys.indexes WHERE name = 'IX_Transactions_TenantId'").ToString() == "1";
            var productStocksIndexExists = migrationBuilder.Sql(
                "SELECT COUNT(*) FROM sys.indexes WHERE name = 'IX_ProductStocks_TenantId'").ToString() == "1";

            if (transactionsIndexExists)
            {
                migrationBuilder.DropIndex(
                    name: "IX_Transactions_TenantId",
                    table: "Transactions");
            }

            if (productStocksIndexExists)
            {
                migrationBuilder.DropIndex(
                    name: "IX_ProductStocks_TenantId",
                    table: "ProductStocks");
            }

            // Check if the columns exist before trying to drop them
            var transactionsColumnExists = migrationBuilder.Sql(
                "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('Transactions') AND name = 'TenantId'").ToString() == "1";
            var productStocksColumnExists = migrationBuilder.Sql(
                "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('ProductStocks') AND name = 'TenantId'").ToString() == "1";

            if (transactionsColumnExists)
            {
                migrationBuilder.DropColumn(
                    name: "TenantId",
                    table: "Transactions");
            }

            if (productStocksColumnExists)
            {
                migrationBuilder.DropColumn(
                    name: "TenantId",
                    table: "ProductStocks");
            }

            // Insert new seed data
            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "Name" },
                values: new object[] { -1, "ancap" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "Email", "IsVerified", "LastLoginDate", "Name", "NotificationsEnabled", "Password", "PasswordSalt", "PointBalance", "Role", "TenantId" },
                values: new object[] { -1, null, "admin@servipuntos.uy", false, new DateTime(2025, 6, 11, 1, 24, 13, 730, DateTimeKind.Utc).AddTicks(7230), "Admin Central", true, "L632D3r3mrtdniiprj//8pU4ZTmHB+UqxX18BvEYhNI=", "u4q07UGIRLTEFm8WJe6gXw==", 0, 1, null });

            migrationBuilder.InsertData(
                table: "TenantUIs",
                columns: new[] { "Id", "LogoUrl", "PrimaryColor", "SecondaryColor", "TenantId" },
                values: new object[] { -1, "https://example.com/logo-ancap.png", "#0000FF", "#FFFF00", -1 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BranchId", "Email", "IsVerified", "LastLoginDate", "Name", "NotificationsEnabled", "Password", "PasswordSalt", "PointBalance", "Role", "TenantId" },
                values: new object[] { -2, null, "admintenant@servipuntos.uy", false, new DateTime(2025, 6, 11, 1, 24, 13, 730, DateTimeKind.Utc).AddTicks(7350), "Admin Tenant", true, "L632D3r3mrtdniiprj//8pU4ZTmHB+UqxX18BvEYhNI=", "u4q07UGIRLTEFm8WJe6gXw==", 0, 2, -1 });
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

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ProductStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TenantId",
                table: "Transactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_TenantId",
                table: "ProductStocks",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStocks_Tenants_TenantId",
                table: "ProductStocks",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Tenants_TenantId",
                table: "Transactions",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
