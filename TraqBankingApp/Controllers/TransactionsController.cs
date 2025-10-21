using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TraqBankingApp.Data;
using TraqBankingApp.Models;

namespace TraqBankingApp.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    private readonly PeopleManagerContext _db;

    public TransactionsController(PeopleManagerContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(int accountId)
    {
        var account = await _db.Accounts.Include(a => a.Status).FirstOrDefaultAsync(a => a.Code == accountId);
        if (account == null) return NotFound();
        ViewBag.Account = account;
        var txs = await _db.Transactions.Where(t => t.AccountCode == accountId).OrderByDescending(t => t.TransactionDate).ToListAsync();
        return View(txs);
    }

    public IActionResult Create(int accountId)
    {
        return View(new TransactionEntry
        {
            AccountCode = accountId,
            TransactionDate = DateTime.Today
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(TransactionEntry model)
    {
        var account = await _db.Accounts.Include(a => a.Status).FirstOrDefaultAsync(a => a.Code == model.AccountCode);
        if (account == null) return NotFound();
        if (account.Status?.Name == "Closed")
        {
            ModelState.AddModelError("", "Cannot add transactions to a closed account.");
        }
        if (model.TransactionDate.Date > DateTime.Today)
        {
            ModelState.AddModelError("TransactionDate", "Transaction date cannot be in the future.");
        }
        if (model.Amount == 0)
        {
            ModelState.AddModelError("Amount", "Amount cannot be zero.");
        }
        if (!ModelState.IsValid) return View(model);

        model.CaptureDate = DateTime.Now;
        _db.Transactions.Add(model);

        account.OutstandingBalance += model.Amount;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { accountId = model.AccountCode });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var tx = await _db.Transactions.FindAsync(id);
        if (tx == null) return NotFound();
        return View(tx);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, TransactionEntry model)
    {
        if (id != model.Code) return BadRequest();
        var account = await _db.Accounts.Include(a => a.Status).FirstOrDefaultAsync(a => a.Code == model.AccountCode);
        if (account == null) return NotFound();
        if (account.Status?.Name == "Closed")
        {
            ModelState.AddModelError("", "Cannot edit transactions on a closed account.");
        }
        if (model.TransactionDate.Date > DateTime.Today)
        {
            ModelState.AddModelError("TransactionDate", "Transaction date cannot be in the future.");
        }
        if (model.Amount == 0)
        {
            ModelState.AddModelError("Amount", "Amount cannot be zero.");
        }
        if (!ModelState.IsValid) return View(model);

        var existing = await _db.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.Code == id);
        if (existing == null) return NotFound();

        var diff = model.Amount - existing.Amount;
        account.OutstandingBalance += diff;

        model.CaptureDate = DateTime.Now; // update capture date on save
        _db.Update(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { accountId = model.AccountCode });
    }

    public async Task<IActionResult> Details(int id)
    {
        var tx = await _db.Transactions
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Code == id);
        if (tx == null) return NotFound();
        return View(tx);
    }
}
