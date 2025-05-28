using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class UpdateProductsImageUrl : Migration
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
                values: new object[] { "xZTYp+1wW9S0zRKK5Zma72x6rdOnUwvKucGrx5ewYUs=", "uqVv29SziAuBEu2U5VDSow==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Password", "PasswordSalt" },
                values: new object[] { "xZTYp+1wW9S0zRKK5Zma72x6rdOnUwvKucGrx5ewYUs=", "uqVv29SziAuBEu2U5VDSow==" });
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
