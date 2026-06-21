using CarAgence.Data;
using CarAgence.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarAgence.Web.Controllers;

public class ReservationsController : Controller
{
    private readonly AppDbContext _context;

    public ReservationsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var reservations = await _context.Reservations
            .Include(r => r.Client)
            .Include(r => r.Voiture)
                .ThenInclude(v => v.Modele)
                .ThenInclude(m => m.Marque)
            .ToListAsync();
        return View(reservations);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var reservation = await _context.Reservations
            .Include(r => r.Client)
            .Include(r => r.Voiture)
                .ThenInclude(v => v.Modele)
                .ThenInclude(m => m.Marque)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
            return NotFound();

        return View(reservation);
    }

    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Reservation reservation)
    {
        // first check if the dates make sense
        if (reservation.DateFin <= reservation.DateDebut)
        {
            ModelState.AddModelError("", "End date must be after start date.");
            await LoadDropdowns();
            return View(reservation);
        }

        // check if the car is available for the selected dates
        bool isAvailable = !await _context.Reservations.AnyAsync(r =>
            r.VoitureId == reservation.VoitureId &&
            r.DateDebut < reservation.DateFin &&
            r.DateFin > reservation.DateDebut);

        if (!isAvailable)
        {
            ModelState.AddModelError("", "This car is already reserved for the selected period.");
            await LoadDropdowns();
            return View(reservation);
        }

        if (ModelState.IsValid)
        {
            _context.Add(reservation);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Reservation created successfully.";
            return RedirectToAction(nameof(Index));
        }

        await LoadDropdowns();
        return View(reservation);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
            return NotFound();

        await LoadDropdowns();
        return View(reservation);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Reservation reservation)
    {
        if (reservation.DateFin <= reservation.DateDebut)
        {
            ModelState.AddModelError("", "End date must be after start date.");
            await LoadDropdowns();
            return View(reservation);
        }

        bool isAvailable = !await _context.Reservations.AnyAsync(r =>
            r.VoitureId == reservation.VoitureId &&
            r.DateDebut < reservation.DateFin &&
            r.DateFin > reservation.DateDebut &&
            r.Id != reservation.Id);

        if (!isAvailable)
        {
            ModelState.AddModelError("", "This car is already reserved for the selected period.");
            await LoadDropdowns();
            return View(reservation);
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(reservation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Reservations.Any(r => r.Id == reservation.Id))
                    return NotFound();
                else
                    throw;
            }

            TempData["Success"] = "Reservation updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        await LoadDropdowns();
        return View(reservation);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var reservation = await _context.Reservations
            .Include(r => r.Client)
            .Include(r => r.Voiture)
                .ThenInclude(v => v.Modele)
                .ThenInclude(m => m.Marque)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
            return NotFound();

        return View(reservation);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation != null)
        {
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Reservation deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns()
    {
        var clients = await _context.Clients.ToListAsync();
        var voitures = await _context.Voitures
            .Include(v => v.Modele)
            .ThenInclude(m => m.Marque)
            .ToListAsync();

        ViewBag.Clients = clients.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Prenom + " " + c.Nom
        });

        ViewBag.Voitures = voitures.Select(v => new SelectListItem
        {
            Value = v.Id.ToString(),
            Text = v.Immatriculation + " - " + v.Modele.Marque.Nom + " " + v.Modele.Nom + " (" + v.TarifJournalier + "€/j)"
        });
    }
}
