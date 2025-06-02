using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class ProductUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImgaeUrl",
                table: "Products",
                newName: "ImageUrl");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "PasswordSalt" },
                values: new object[] { "UjRJ6aZDtLSNtS4Z2iNavTbYI2QIjVviCTuggKJpfso=", "JCfmyZickAH/jRYWkSuL9w==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Password", "PasswordSalt" },
                values: new object[] { "UjRJ6aZDtLSNtS4Z2iNavTbYI2QIjVviCTuggKJpfso=", "JCfmyZickAH/jRYWkSuL9w==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Products",
                newName: "ImgaeUrl");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "PasswordSalt" },
                values: new object[] { "k/NFv1d2cH/NRAb9EhsQprd/sL9e8Ey44kh34zPSlRw=", "TQLRTQLr6vKr07vrPZk+RQ==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Password", "PasswordSalt" },
                values: new object[] { "k/NFv1d2cH/NRAb9EhsQprd/sL9e8Ey44kh34zPSlRw=", "TQLRTQLr6vKr07vrPZk+RQ==" });
        }
    }
}
