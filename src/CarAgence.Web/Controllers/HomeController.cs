using CarAgence.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAgence.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    // Dashboard with some stats
    public async Task<IActionResult> Index()
    {
        // TODO: maybe add more stats later
        var voitures = await _context.Voitures.CountAsync();
        var clients = await _context.Clients.CountAsync();
        var reservations = await _context.Reservations.CountAsync();

        var today = DateTime.Today;
        var activeRes = await _context.Reservations
            .CountAsync(r => r.DateDebut <= today && r.DateFin >= today);

        ViewBag.NombreVoitures = voitures;
        ViewBag.NombreClients = clients;
        ViewBag.NombreReservations = reservations;
        ViewBag.ReservationsEnCours = activeRes;

        return View();
    }
}
