using CarAgence.Data;
using CarAgence.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAgence.Web.Controllers;

public class MarquesController : Controller
{
    private readonly AppDbContext _context;

    public MarquesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var marques = await _context.Marques.Include(m => m.Modeles).ToListAsync();
        return View(marques);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var marque = await _context.Marques
            .Include(m => m.Modeles)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (marque == null)
            return NotFound();

        return View(marque);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Marque marque)
    {
        if (ModelState.IsValid)
        {
            _context.Add(marque);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Marque added successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(marque);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var marque = await _context.Marques.FindAsync(id);
        if (marque == null)
            return NotFound();

        return View(marque);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Marque marque)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(marque);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Marques.Any(m => m.Id == marque.Id))
                    return NotFound();
                else
                    throw;
            }

            TempData["Success"] = "Marque updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(marque);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var marque = await _context.Marques
            .Include(m => m.Modeles)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (marque == null)
            return NotFound();

        return View(marque);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var marque = await _context.Marques.FindAsync(id);
        if (marque != null)
        {
            try
            {
                _context.Marques.Remove(marque);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Marque deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete this marque because it has associated models.";
            }
        }
        return RedirectToAction(nameof(Index));
    }
}
