using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TraqBankingApp.Data;
using TraqBankingApp.Models;

namespace TraqBankingApp.Controllers;

[Authorize]
public class PersonsController : Controller
{
    private readonly PeopleManagerContext _db;

    public PersonsController(PeopleManagerContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        const int pageSize = 10;
        var query = _db.Persons.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.IdNumber.Contains(search) || (p.Surname != null && p.Surname.Contains(search)));
        }

        var total = await query.CountAsync();
        var persons = await query
            .OrderBy(p => p.Surname).ThenBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        ViewBag.CurrentPage = page;
        ViewBag.Search = search;
        return View(persons);
    }

    public IActionResult Create() => View(new Person());

    [HttpPost]
    public async Task<IActionResult> Create(Person model)
    {
        if (!ModelState.IsValid) return View(model);

        var exists = await _db.Persons.AnyAsync(p => p.IdNumber == model.IdNumber);
        if (exists)
        {
            ModelState.AddModelError("IdNumber", "Duplicate ID number.");
            return View(model);
        }

        _db.Persons.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = model.Code });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var person = await _db.Persons.FindAsync(id);
        if (person == null) return NotFound();
        return View(person);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Person model)
    {
        if (id != model.Code) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var duplicate = await _db.Persons.AnyAsync(p => p.IdNumber == model.IdNumber && p.Code != id);
        if (duplicate)
        {
            ModelState.AddModelError("IdNumber", "Duplicate ID number.");
            return View(model);
        }

        _db.Update(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = model.Code });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var person = await _db.Persons.Include(p => p.Accounts).FirstOrDefaultAsync(p => p.Code == id);
        if (person == null) return NotFound();
        return View(person);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var person = await _db.Persons.Include(p => p.Accounts).ThenInclude(a => a.Status).FirstOrDefaultAsync(p => p.Code == id);
        if (person == null) return NotFound();

        var canDelete = person.Accounts.Count == 0 || person.Accounts.All(a => a.Status != null && a.Status.Name == "Closed");
        if (!canDelete)
        {
            ModelState.AddModelError("", "Cannot delete person with open accounts.");
            return View(person);
        }

        _db.Persons.Remove(person);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var person = await _db.Persons.Include(p => p.Accounts).ThenInclude(a => a.Status).FirstOrDefaultAsync(p => p.Code == id);
        if (person == null) return NotFound();
        return View(person);
    }
}