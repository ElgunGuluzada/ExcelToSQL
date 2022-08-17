using System;

namespace ExcelToSQL.DATA.Endtities
{
    public class ExcelData
    {
        public int Id { get; set; }
        public string Segment { get; set; }
        public string Country { get; set; }
        public string Product { get; set; }
        public string discountBrand { get; set; }
        public double unitsSold { get; set; }
        public double Manifactur { get; set; }
        public double salePrice { get; set; }
        public double grossSales { get; set; }
        public double Discounts { get; set; }
        public double Sales { get; set; }
        public double COGS { get; set; }
        public double Profit { get; set; }
        public DateTime Date { get; set; }
    }
}
