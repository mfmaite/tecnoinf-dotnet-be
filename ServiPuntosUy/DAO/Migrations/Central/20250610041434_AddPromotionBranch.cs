using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class AddPromotionBranch : Migration
    {
        /// <inheritdoc />
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Verificar y eliminar la clave foránea solo si existe
    migrationBuilder.Sql(@"
        IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ProductStocks_Tenants_TenantId')
        BEGIN
            ALTER TABLE [ProductStocks] DROP CONSTRAINT [FK_ProductStocks_Tenants_TenantId];
        END
    ");

    // Verificar y eliminar el índice solo si existe
    migrationBuilder.Sql(@"
        IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ProductStocks_TenantId')
        BEGIN
            DROP INDEX [IX_ProductStocks_TenantId] ON [ProductStocks];
        END
    ");

    // Verificar y eliminar la columna TenantId solo si existe
    migrationBuilder.Sql(@"
        IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'TenantId' AND Object_ID = Object_ID(N'ProductStocks'))
        BEGIN
            ALTER TABLE [ProductStocks] DROP COLUMN [TenantId];
        END
    ");

    // Crear la tabla PromotionBranches
    migrationBuilder.CreateTable(
        name: "PromotionBranches",
        columns: table => new
        {
            PromotionId = table.Column<int>(type: "int", nullable: false),
            BranchId = table.Column<int>(type: "int", nullable: false),
            TenantId = table.Column<int>(type: "int", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_PromotionBranches", x => new { x.PromotionId, x.BranchId, x.TenantId });
            table.ForeignKey(
                name: "FK_PromotionBranches_Branches_BranchId",
                column: x => x.BranchId,
                principalTable: "Branches",
                principalColumn: "Id");
            table.ForeignKey(
                name: "FK_PromotionBranches_Promotions_PromotionId",
                column: x => x.PromotionId,
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                name: "FK_PromotionBranches_Tenants_TenantId",
                column: x => x.TenantId,
                principalTable: "Tenants",
                principalColumn: "Id");
        });

    // Actualizar datos en la tabla Users
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

    // Crear índices en la tabla PromotionBranches
    migrationBuilder.CreateIndex(
        name: "IX_PromotionBranches_BranchId",
        table: "PromotionBranches",
        column: "BranchId");

    migrationBuilder.CreateIndex(
        name: "IX_PromotionBranches_TenantId",
        table: "PromotionBranches",
        column: "TenantId");
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionBranches");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ProductStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 5, 0, 59, 43, 91, DateTimeKind.Utc).AddTicks(4268), "74lI/359Ugnlj9bBElG6b9ggs9qdkIm6S1lsMt4IqYM=", "yGzQElt+vx7mngS52PM05A==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastLoginDate", "Password", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 6, 5, 0, 59, 43, 91, DateTimeKind.Utc).AddTicks(4244), "74lI/359Ugnlj9bBElG6b9ggs9qdkIm6S1lsMt4IqYM=", "yGzQElt+vx7mngS52PM05A==" });

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
        }
    }
}
