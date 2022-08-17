using ExcelToSQL.DATA.Endtities;
using Microsoft.EntityFrameworkCore;

namespace ExcelToSQL.DATA
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        public DbSet<ExcelData> ExcelDatas { get; set; }
    }
}
