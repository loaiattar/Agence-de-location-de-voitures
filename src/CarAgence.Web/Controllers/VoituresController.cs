using CarAgence.Data;
using CarAgence.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarAgence.Web.Controllers;

public class VoituresController : Controller
{
    private readonly AppDbContext _context;

    public VoituresController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var voitures = await _context.Voitures
            .Include(v => v.Modele)
            .ThenInclude(m => m.Marque)
            .ToListAsync();
        return View(voitures);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var voiture = await _context.Voitures
            .Include(v => v.Modele)
            .ThenInclude(m => m.Marque)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (voiture == null)
            return NotFound();

        return View(voiture);
    }

    public async Task<IActionResult> Create()
    {
        await LoadModeles();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Voiture voiture)
    {
        if (ModelState.IsValid)
        {
            _context.Add(voiture);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Voiture added successfully.";
            return RedirectToAction(nameof(Index));
        }

        await LoadModeles();
        return View(voiture);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var voiture = await _context.Voitures.FindAsync(id);
        if (voiture == null)
            return NotFound();

        await LoadModeles(voiture.ModeleId);
        return View(voiture);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Voiture voiture)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(voiture);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // concurrency issue, check if the voiture still exists
                if (!_context.Voitures.Any(v => v.Id == voiture.Id))
                    return NotFound();
                else
                    throw;
            }

            TempData["Success"] = "Voiture updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        await LoadModeles(voiture.ModeleId);
        return View(voiture);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var voiture = await _context.Voitures
            .Include(v => v.Modele)
            .ThenInclude(m => m.Marque)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (voiture == null)
            return NotFound();

        return View(voiture);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var voiture = await _context.Voitures.FindAsync(id);
        if (voiture != null)
        {
            try
            {
                _context.Voitures.Remove(voiture);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Voiture deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete this car because it has reservations.";
            }
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadModeles(int? selectedId = null)
    {
        var modeles = await _context.Modeles
            .Include(m => m.Marque)
            .ToListAsync();

        var items = modeles.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = m.Marque.Nom + " - " + m.Nom,
            Selected = m.Id == selectedId
        });

        ViewBag.Modeles = items;
    }
}
