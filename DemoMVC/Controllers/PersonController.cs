using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.IO;
using DemoMVC.Models.Process;

namespace DemoMVC.Controllers
{
    public class PersonController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ExcelProcess _excelProcess = new ExcelProcess();

        public PersonController(ApplicationDbContext context)
        {
            _context = context;

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null)
            {
                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension != ".xls" && fileExtension != ".xlsx")
                {
                    ModelState.AddModelError("", "Please choose excel file to upload!");
                }
                else
                {
                    var fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + fileExtension;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);
                    var fileLocation = new FileInfo(filePath).ToString();

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    
                    var dt = _excelProcess.ExcelToDataTable(fileLocation) as DataTable;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var ps = new Person()
                            {
                                PersonId = dt.Rows[i][0]?.ToString() ?? "",
                                FullName = dt.Rows[i][1].ToString(),
                                Address = dt.Rows[i][2].ToString()
                            };

                            _context.Add(ps);
                        }
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            return View(); 
        }
        public IActionResult Download()
        
        {
           
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 

            var fileName = "YourFileName" + ".xlsx";

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Your Name");

                worksheet.Cells["A1"].Value = "PersonID";
                worksheet.Cells["B1"].Value = "FullName";
                worksheet.Cells["C1"].Value = "Address";

                var personList = _context.People.ToList();  

                worksheet.Cells["A2"].LoadFromCollection(personList, false);
                var stream = new MemoryStream(excelPackage.GetAsByteArray());

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.People.ToListAsync());
        }
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.People
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        public IActionResult Create()
        {
            var lastPerson = _context.People
                .OrderByDescending(p => p.PersonId)
                .FirstOrDefault();

            

            string newID = "PS001";

            if (lastPerson != null && !string.IsNullOrEmpty(lastPerson.PersonId))
            {
                string lastId = lastPerson.PersonId.Trim();

                // Kiểm tra đúng định dạng PSxxx
                if (lastId.Length >= 3 && int.TryParse(lastId.Substring(2), out int lastNumber))
                {
                    newID = $"PS{(lastNumber + 1).ToString("D3")}";
                }
            }


            var person = new Person
            {
                PersonId = newID,
                FullName = "",
                Address = ""
            };

            return View(person);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonId,FullName,Address")] Person person)
        {
            var lastPerson = _context.People
               .OrderByDescending(p => p.PersonId)
               .FirstOrDefault();



            string newID = "PS001";

            if (lastPerson != null && !string.IsNullOrEmpty(lastPerson.PersonId))
            {
                string lastId = lastPerson.PersonId.Trim();

                // Kiểm tra đúng định dạng PSxxx
                if (lastId.Length >= 3 && int.TryParse(lastId.Substring(2), out int lastNumber))
                {
                    newID = $"PS{(lastNumber + 1).ToString("D3")}";
                }
            }
            person.PersonId = newID;

            _context.Add(person);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            return View(person);
        }
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.People.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PersonId,FullName,Address")] Person person)
        {
            if (id != person.PersonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(person.PersonId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.People
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var student = await _context.People.FindAsync(id);
            if (student != null)
            {
                _context.People.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(string id)
        {
            return _context.People.Any(e => e.PersonId == id);
        }
    }
}