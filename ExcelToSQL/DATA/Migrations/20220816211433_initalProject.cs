using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExcelToSQL.DATA.Migrations
{
    public partial class initalProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExcelDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Segment = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Product = table.Column<string>(nullable: true),
                    discountBrand = table.Column<string>(nullable: true),
                    unitsSold = table.Column<double>(nullable: false),
                    Manifactur = table.Column<double>(nullable: false),
                    salePrice = table.Column<double>(nullable: false),
                    grossSales = table.Column<double>(nullable: false),
                    Discounts = table.Column<double>(nullable: false),
                    Sales = table.Column<double>(nullable: false),
                    COGS = table.Column<double>(nullable: false),
                    Profit = table.Column<double>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcelDatas", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExcelDatas");
        }
    }
}
