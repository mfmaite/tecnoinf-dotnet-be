using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class AddPromotionProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromotionProducts",
                columns: table => new
                {
                    PromotionId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionProducts", x => new { x.PromotionId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_PromotionProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionProducts_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_PromotionProducts_ProductId",
                table: "PromotionProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionProducts");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 10, 4, 14, 34, 323, DateTimeKind.Utc).AddTicks(3280), "tnx/euppy2dFn5WrquNu7Ehg6bz4ApsT+gEzQhWxXwg=", "vNxRez94WnSuBrcBsZD2MA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 10, 4, 14, 34, 323, DateTimeKind.Utc).AddTicks(3250), "tnx/euppy2dFn5WrquNu7Ehg6bz4ApsT+gEzQhWxXwg=", "vNxRez94WnSuBrcBsZD2MA==" });
        }
    }
}
