using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    /// <inheritdoc />
    public partial class UpdateTenantIdInBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "PasswordSalt" },
                values: new object[] { "eqfEMwjIhFvPikVt0rT11HRRtPA3z+ZLlI6lm82Hb/M=", "wtVAt8oceNJozzjPC/D/sw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Password", "PasswordSalt" },
                values: new object[] { "eqfEMwjIhFvPikVt0rT11HRRtPA3z+ZLlI6lm82Hb/M=", "wtVAt8oceNJozzjPC/D/sw==" });
        }
    }
}
