using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Models;
using DemoMVC.Data;

namespace MvcMovie.Controllers
{
    public class PersonController : Controller
    {
        private readonly ApplicationDbContext _context;   
        private readonly AutoGenerateCode _autoGenerateCode;
        public PersonController(ApplicationDbContext context, AutoGenerateCode autoGenerateCode)

        {
            _context = context;
            _autoGenerateCode = autoGenerateCode;
        }

        public async Task<IActionResult> Index()
        {
            var person = await _context.People.ToListAsync();
            return View(person);
        }

        public IActionResult Create()
        {
            var lastPerson = _context.People // lay ma cuoi cung tron csdl
            .OrderByDescending (s => s.PersonId)
            .FirstOrDefault();  

            var PersonId = lastPerson?.PersonId ?? "P000"; 

            var newPersonId = _autoGenerateCode.GenerateCode(PersonId); 
            ViewBag.newPersonId = newPersonId;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonId,FullName,Address")] Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.People == null)
            {
                return NotFound();
            }

            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
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
                    if (!PersonExists(person.PersonId))
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

            return View(person); // Nếu dữ liệu không hợp lệ
        }

public async Task<IActionResult> Delete(string id)
{
    if (id == null || _context.People == null)
    {
        return NotFound();
    }

    var person = await _context.People
        .FirstOrDefaultAsync(m => m.PersonId == id);

    if (person == null)
    {
        return NotFound();
    }

    return View(person);
}


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.People == null)
            {
                return Problem("Entity set 'ApplicationDbContext.People' is null.");
            }

            var person = await _context.People.FindAsync(id);
            if (person != null)
            {
                _context.People.Remove(person);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(string id)
        {
            return _context.People.Any(e => e.PersonId == id);
        }
    }
}