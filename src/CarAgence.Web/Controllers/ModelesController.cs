using CarAgence.Data;
using CarAgence.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarAgence.Web.Controllers;

public class ModelesController : Controller
{
    private readonly AppDbContext _context;

    public ModelesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var modeles = await _context.Modeles.Include(m => m.Marque).ToListAsync();
        return View(modeles);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var modele = await _context.Modeles
            .Include(m => m.Marque)
            .Include(m => m.Voitures)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (modele == null)
            return NotFound();

        return View(modele);
    }

    public async Task<IActionResult> Create()
    {
        var marques = await _context.Marques.ToListAsync();
        ViewBag.Marques = new SelectList(marques, "Id", "Nom");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Modele modele)
    {
        if (ModelState.IsValid)
        {
            _context.Add(modele);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Modele added successfully.";
            return RedirectToAction(nameof(Index));
        }

        var marques = await _context.Marques.ToListAsync();
        ViewBag.Marques = new SelectList(marques, "Id", "Nom");
        return View(modele);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var modele = await _context.Modeles.FindAsync(id);
        if (modele == null)
            return NotFound();

        var marques = await _context.Marques.ToListAsync();
        ViewBag.Marques = new SelectList(marques, "Id", "Nom");
        return View(modele);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Modele modele)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(modele);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Modeles.Any(m => m.Id == modele.Id))
                    return NotFound();
                else
                    throw;
            }

            TempData["Success"] = "Modele updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        var marques = await _context.Marques.ToListAsync();
        ViewBag.Marques = new SelectList(marques, "Id", "Nom");
        return View(modele);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var modele = await _context.Modeles
            .Include(m => m.Marque)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (modele == null)
            return NotFound();

        return View(modele);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var modele = await _context.Modeles.FindAsync(id);
        if (modele != null)
        {
            try
            {
                _context.Modeles.Remove(modele);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Modele deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete this model because it has associated cars.";
            }
        }
        return RedirectToAction(nameof(Index));
    }
}
