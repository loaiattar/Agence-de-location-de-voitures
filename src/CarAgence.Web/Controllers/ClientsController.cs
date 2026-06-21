using CarAgence.Data;
using CarAgence.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAgence.Web.Controllers;

public class ClientsController : Controller
{
    private readonly AppDbContext _context;

    public ClientsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var clients = await _context.Clients.ToListAsync();
        return View(clients);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
            return NotFound();

        return View(client);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Client client)
    {
        if (ModelState.IsValid)
        {
            _context.Add(client);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Client added successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(client);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            return NotFound();

        return View(client);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Client client)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(client);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Clients.Any(c => c.Id == client.Id))
                    return NotFound();
                else
                    throw;
            }

            TempData["Success"] = "Client updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(client);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
            return NotFound();

        return View(client);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client != null)
        {
            try
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Client deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete this client because they have reservations.";
            }
        }
        return RedirectToAction(nameof(Index));
    }
}
