using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class AddPromotionFieldsToTransactionItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "TransactionItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PromotionId",
                table: "TransactionItems",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 21, 0, 13, 46, 430, DateTimeKind.Utc).AddTicks(919), "BAXEY93LMZ+mECUatvpJPJ/+bGjV+buL73GEi/JIJnE=", "S73QxcNlvtRqpvggUNwpxA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 21, 0, 13, 46, 430, DateTimeKind.Utc).AddTicks(898), "BAXEY93LMZ+mECUatvpJPJ/+bGjV+buL73GEi/JIJnE=", "S73QxcNlvtRqpvggUNwpxA==" });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionItems_PromotionId",
                table: "TransactionItems",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItems_Promotions_PromotionId",
                table: "TransactionItems",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItems_Promotions_PromotionId",
                table: "TransactionItems");

            migrationBuilder.DropIndex(
                name: "IX_TransactionItems_PromotionId",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "TransactionItems");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 19, 1, 54, 13, 277, DateTimeKind.Utc).AddTicks(7750), "nfrui92FtvMJMR5ZUCQQxkDUxTfu+fF1dcIMlODoyv4=", "+B6MCaVIHXvhYymzRZNw+A==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 19, 1, 54, 13, 277, DateTimeKind.Utc).AddTicks(7727), "nfrui92FtvMJMR5ZUCQQxkDUxTfu+fF1dcIMlODoyv4=", "+B6MCaVIHXvhYymzRZNw+A==" });
        }
    }
}
