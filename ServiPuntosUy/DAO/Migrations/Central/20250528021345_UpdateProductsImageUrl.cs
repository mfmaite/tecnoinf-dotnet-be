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
            // Verificar si la columna ImgaeUrl existe antes de intentar renombrarla
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1
                    FROM sys.columns 
                    WHERE Name = N'ImgaeUrl'
                    AND Object_ID = Object_ID(N'Products')
                )
                BEGIN
                    EXEC sp_rename N'[Products].[ImgaeUrl]', N'ImageUrl', N'COLUMN';
                END
            ");

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
            // Verificar si la columna ImageUrl existe antes de intentar renombrarla
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1
                    FROM sys.columns 
                    WHERE Name = N'ImageUrl'
                    AND Object_ID = Object_ID(N'Products')
                )
                BEGIN
                    EXEC sp_rename N'[Products].[ImageUrl]', N'ImgaeUrl', N'COLUMN';
                END
            ");

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
