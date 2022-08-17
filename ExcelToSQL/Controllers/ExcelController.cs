using Aspose.Cells;
using ExcelToSQL.DATA;
using ExcelToSQL.DATA.Endtities;
using ExcelToSQL.DTO;
using ExcelToSQL.Interface;
using ExcelToSQL.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelToSQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _service;
        private readonly IWebHostEnvironment _env;

        public ExcelController(AppDbContext context, IEmailService service, IWebHostEnvironment env)
        {
            _context = context;
            _service = service;
            _env = env;
        }
        /// <summary>
        /// Upload File 
        /// </summary>
        /// <param name="file"></param>
        /// <returns> Api/Excel </returns>
        /// <parameters></parameters>
        [HttpPost]
        public async Task<IActionResult> UploadData(IFormFile file)
        {
            if (file == null) return BadRequest();
            var exten = Path.GetExtension(file.FileName);
            if (exten != ".xls" && exten != ".xlsx") return StatusCode(1, "Only .xls or .xlsx Format");
            if (file.Length / (1024 * 1024) > 5) return StatusCode(2, "Oversize");


            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        ExcelData excelData = new ExcelData();
                        excelData.Segment = worksheet.Cells[row, 1].Value.ToString().Trim();
                        excelData.Country = worksheet.Cells[row, 2].Value.ToString().Trim();
                        excelData.Product = worksheet.Cells[row, 3].Value.ToString().Trim();
                        excelData.discountBrand = worksheet.Cells[row, 4].Value.ToString().Trim();
                        excelData.unitsSold = double.Parse(worksheet.Cells[row, 5].Value.ToString());
                        excelData.Manifactur = double.Parse(worksheet.Cells[row, 6].Value.ToString());
                        excelData.salePrice = double.Parse(worksheet.Cells[row, 7].Value.ToString());
                        excelData.grossSales = double.Parse(worksheet.Cells[row, 8].Value.ToString());
                        excelData.Discounts = double.Parse(worksheet.Cells[row, 9].Value.ToString());
                        excelData.Sales = double.Parse(worksheet.Cells[row, 10].Value.ToString());
                        excelData.COGS = double.Parse(worksheet.Cells[row, 11].Value.ToString());
                        excelData.Profit = double.Parse(worksheet.Cells[row, 12].Value.ToString());
                        excelData.Date = DateTime.Parse(worksheet.Cells[row, 13].Value.ToString());

                        await _context.AddAsync(excelData);
                    }
                };
                await _context.SaveChangesAsync();
            };
            return StatusCode(201, "Data Saved To SQL");
        }

        [HttpGet]
        public async Task<IActionResult> SendRepo([FromQuery] SendFilterDto filter, [FromQuery] SendType type)
        {
            List<ReturnDataDto> dataList = new List<ReturnDataDto>();
            var datas = await _context.ExcelDatas.Where(e => e.Date <= filter.EndData && e.Date >= filter.StartData).AsQueryable().AsNoTracking().ToListAsync();
            switch (type)
            {
                case SendType.Segment:
                    datas.GroupBy(d => d.Segment).Select(data => new ReturnDataDto
                    {
                        Name = data.Key,
                        Count = data.Count(),
                        totalProfits = data.Sum(x => x.Profit),
                        totalDiscounts = data.Sum(x => x.Discounts),
                        totalSales = data.Sum(x => x.Sales),
                    }).ToList();
                    break;
                case SendType.Country:
                    datas.GroupBy(d => d.Country).Select(data => new ReturnDataDto
                    {
                        Name = data.Key,
                        Count = data.Count(),
                        totalProfits = data.Sum(x => x.Profit),
                        totalDiscounts = data.Sum(x => x.Discounts),
                        totalSales = data.Sum(x => x.Sales),
                    }).ToList();
                    break;
                case SendType.Product:
                    datas.GroupBy(d => d.Product).Select(data => new ReturnDataDto
                    {
                        Name = data.Key,
                        Count = data.Count(),
                        totalProfits = data.Sum(x => x.Profit),
                        totalDiscounts = data.Sum(x => x.Discounts),
                        totalSales = data.Sum(x => x.Sales),
                    }).ToList();
                    break;
                case SendType.Discount:
                    ReturnDataDto data = new ReturnDataDto();
                    foreach (var item in datas.OrderBy(p => p.Product).ToList())
                    {
                        data.totalDiscounts = 1 - (item.salePrice - item.Discounts) / 100;
                        //data.totalDiscounts = 100 * (item.Discounts / item.salePrice);
                        dataList.Add(data);
                    }
                    break;
                default:
                    break;
            }

            string fileName = Guid.NewGuid().ToString()+".xlsx";

            var pathFolder = Path.Combine(_env.WebRootPath,fileName);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1").Cells[1, 1].LoadFromCollection(dataList, true);
                package.SaveAs(pathFolder);

                MemoryStream ms = new MemoryStream();

                using (var file = new FileStream(pathFolder, FileMode.Open, FileAccess.Read))
                {
                    var bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                    file.Close();
                    _service.SendEmail(filter.AccepttorEmail, "Salam", "Hesabatiniz", fileName, bytes);
                }
             
                return Ok("Sended");
            }

        }
    }
}
