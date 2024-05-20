using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SupplierMaintenanceWeb.Data;
using SupplierMaintenanceWeb.Models;
using SupplierMaintenanceWeb.Models.Entities;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SupplierMaintenanceWeb.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public SuppliersController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddSupplierViewModel viewModel)
        {
            var supplier = new Supplier
            {
                SupplierCode = viewModel.SupplierCode,
                Name = viewModel.Name,
                Address = viewModel.Address,
                Province = viewModel.Province,
                City = viewModel.City,
                PIC = viewModel.PIC
            };

            await dbContext.Suppliers.AddAsync(supplier);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Suppliers"); ;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string supplierCode, string province, string city)
        {
            var suppliers = from s in dbContext.Suppliers select s;

            Debug.Write("test" + "\n");

            Debug.Write(suppliers + "\n");

            if (!string.IsNullOrEmpty(supplierCode))
            {
                suppliers = suppliers.Where(s => s.SupplierCode.Contains(supplierCode));
            }

            if (!string.IsNullOrEmpty(province))
            {
                suppliers = suppliers.Where(s => s.Province == province);
            }

            if (!string.IsNullOrEmpty(city))
            {
                suppliers = suppliers.Where(s => s.City == city);
            }

            var supplierList = await suppliers.ToListAsync();

            var locations = await dbContext.Locations.ToListAsync();
            var provinces = locations.Select(l => l.Province).Distinct().ToList();

            ViewBag.Provinces = new SelectList(provinces);
            ViewBag.Locations = locations;

            return View(supplierList);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(Supplier viewModel)
        {
            var supplier = await dbContext.Suppliers.FindAsync(viewModel.SupplierCode);


            Debug.Write("TEST\n");

            Debug.Write(viewModel.SupplierCode + "\n");

            if (supplier is not null)
            {
                supplier.Name = viewModel.Name;
                supplier.Address = viewModel.Address;
                supplier.Province = viewModel.Province;
                supplier.City = viewModel.City;
                supplier.PIC = viewModel.PIC;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Suppliers");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string selectedSuppliers)
        {
            var supplier = await dbContext.Suppliers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SupplierCode == selectedSuppliers);

            Debug.Write("TEST\n");

            Debug.Write(selectedSuppliers + "\n");

            if (supplier is not null)
            {
                dbContext.Suppliers.Remove(supplier);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Suppliers");
        }

        [HttpGet]
        public async Task<IActionResult> Download()
        {
            var suppliers = await dbContext.Suppliers.ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Suppliers");
                worksheet.Cells["A1"].Value = "Supplier Code";
                worksheet.Cells["B1"].Value = "Supplier Name";
                worksheet.Cells["C1"].Value = "Address";
                worksheet.Cells["D1"].Value = "Province";
                worksheet.Cells["E1"].Value = "City";
                worksheet.Cells["F1"].Value = "PIC";

                for (int i = 0; i < suppliers.Count; i++)
                {
                    var supplier = suppliers[i];
                    worksheet.Cells[i + 2, 1].Value = supplier.SupplierCode;
                    worksheet.Cells[i + 2, 2].Value = supplier.Name;
                    worksheet.Cells[i + 2, 3].Value = supplier.Address;
                    worksheet.Cells[i + 2, 4].Value = supplier.Province;
                    worksheet.Cells[i + 2, 5].Value = supplier.City;
                    worksheet.Cells[i + 2, 6].Value = supplier.PIC;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Suppliers-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.Message = "Please select a valid file.";
                return RedirectToAction(nameof(Index));
            }

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        ViewBag.Message = "No worksheet found in the Excel file.";
                        return RedirectToAction(nameof(Index));
                    }

                    int rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var supplierCode = worksheet.Cells[row, 1].Value?.ToString().Trim();
                        if (string.IsNullOrEmpty(supplierCode))
                            continue;

                        var supplier = await dbContext.Suppliers.FirstOrDefaultAsync(s => s.SupplierCode == supplierCode);
                        if (supplier == null)
                        {
                            supplier = new Supplier { SupplierCode = supplierCode };
                            dbContext.Suppliers.Add(supplier);
                        }

                        supplier.Name = worksheet.Cells[row, 2].Value?.ToString().Trim();
                        supplier.Address = worksheet.Cells[row, 3].Value?.ToString().Trim();
                        supplier.Province = worksheet.Cells[row, 4].Value?.ToString().Trim();
                        supplier.City = worksheet.Cells[row, 5].Value?.ToString().Trim();
                        supplier.PIC = worksheet.Cells[row, 6].Value?.ToString().Trim();
                    }

                    await dbContext.SaveChangesAsync();
                }
            }

            ViewBag.Message = "Upload successful.";
            return RedirectToAction(nameof(Index));
        }
    }
}
