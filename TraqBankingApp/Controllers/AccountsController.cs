using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TraqBankingApp.Data;
using TraqBankingApp.Models;

namespace TraqBankingApp.Controllers;

[Authorize]
public class AccountsController : Controller
{
    private readonly PeopleManagerContext _db;

    public AccountsController(PeopleManagerContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(int personId)
    {
        var person = await _db.Persons.FindAsync(personId);
        if (person == null) return NotFound();

        ViewBag.Person = person;
        var accounts = await _db.Accounts
            .Include(a => a.Status)
            .Where(a => a.PersonCode == personId)
            .ToListAsync();
        return View(accounts);
    }

    public IActionResult Create(int personId)
    {
        var model = new Account { PersonCode = personId, OutstandingBalance = 0m };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Account model)
    {
        if (!ModelState.IsValid) return View(model);

        var duplicate = await _db.Accounts.AnyAsync(a => a.AccountNumber == model.AccountNumber);
        if (duplicate)
        {
            ModelState.AddModelError("AccountNumber", "Duplicate account number.");
            return View(model);
        }

        model.StatusCode = await _db.Statuses.Where(s => s.Name == "Open").Select(s => s.Code).FirstOrDefaultAsync();
        model.OutstandingBalance = 0m; // start with zero balance
        _db.Accounts.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = model.Code });
    }

    public async Task<IActionResult> Details(int id)
    {
        var account = await _db.Accounts
            .Include(a => a.Person)
            .Include(a => a.Status)
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Code == id);

        if (account == null) return NotFound();
        return View(account);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var account = await _db.Accounts.FindAsync(id);
        if (account == null) return NotFound();
        return View(account);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Account model)
    {
        if (id != model.Code) return BadRequest();

        // check for existing account
        var existing = await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Code == id);
        if (existing == null) return NotFound();

        // check for duplicate account number
        var duplicate = await _db.Accounts.AnyAsync(a => a.AccountNumber == model.AccountNumber && a.Code != id);
        if (duplicate)
        {
            ModelState.AddModelError("AccountNumber", "Duplicate account number.");
            return View(model);
        }

        if (!ModelState.IsValid) return View(model);

        model.OutstandingBalance = existing.OutstandingBalance;
        model.StatusCode = existing.StatusCode;

        _db.Update(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = model.Code });
    }

    // Change account's status
    [HttpPost]
    public async Task<IActionResult> Toggle(int id)
    {
        var account = await _db.Accounts.Include(a => a.Status).FirstOrDefaultAsync(a => a.Code == id);
        if (account == null) return NotFound();

        var openCode = await _db.Statuses.Where(s => s.Name == "Open").Select(s => s.Code).FirstAsync();
        var closedCode = await _db.Statuses.Where(s => s.Name == "Closed").Select(s => s.Code).FirstAsync();

        // if closing, check balance
        if (account.Status?.Name == "Open")
        {
            if (account.OutstandingBalance != 0)
            {
                TempData["Error"] = "Cannot close account with non-zero balance.";
                return RedirectToAction(nameof(Details), new { id = account.Code });
            }
            account.StatusCode = closedCode;
        }
        else
        {
            account.StatusCode = openCode;
        }

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = account.Code });
    }
}