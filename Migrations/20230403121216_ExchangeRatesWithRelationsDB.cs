using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication2.Migrations
{
    public partial class ExchangeRatesWithRelationsDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyUSD",
                table: "ExchangeRates");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "ExchangeRates",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Country = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Code);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencyCode",
                table: "ExchangeRates",
                column: "CurrencyCode");

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeRates_Currency_CurrencyCode",
                table: "ExchangeRates",
                column: "CurrencyCode",
                principalTable: "Currency",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeRates_Currency_CurrencyCode",
                table: "ExchangeRates");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeRates_CurrencyCode",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "ExchangeRates");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyUSD",
                table: "ExchangeRates",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
