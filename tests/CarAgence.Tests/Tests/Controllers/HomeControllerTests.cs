using CarAgence.Data;
using CarAgence.Domain.Entities;
using CarAgence.Tests.Helpers;
using CarAgence.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAgence.Tests.Tests.Controllers;

public class HomeControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _context = TestHelper.CreateInMemoryContext();
        _context.Database.EnsureCreated();
        _controller = new HomeController(_context);
    }

    public void Dispose()
    {
        _controller.Dispose();
        _context.Dispose();
    }

    [Fact]
    public async Task Index_ReturnsView()
    {
        var result = await _controller.Index();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Index_ReturnsViewResult()
    {
        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.ViewData);
    }

    [Fact]
    public async Task Index_WithSeededData_HasCorrectCounts()
    {
        var voituresCount = await _context.Voitures.CountAsync();
        var clientsCount = await _context.Clients.CountAsync();
        var reservationsCount = await _context.Reservations.CountAsync();

        Assert.Equal(6, voituresCount);
        Assert.Equal(2, clientsCount);
        Assert.Equal(2, reservationsCount);
    }

    [Fact]
    public async Task Index_WithEmptyDatabase_ReturnsZeroCounts()
    {
        _context.Reservations.RemoveRange(_context.Reservations);
        _context.Voitures.RemoveRange(_context.Voitures);
        _context.Clients.RemoveRange(_context.Clients);
        _context.Modeles.RemoveRange(_context.Modeles);
        _context.Marques.RemoveRange(_context.Marques);
        await _context.SaveChangesAsync();

        var voituresCount = await _context.Voitures.CountAsync();
        var clientsCount = await _context.Clients.CountAsync();
        var reservationsCount = await _context.Reservations.CountAsync();

        Assert.Equal(0, voituresCount);
        Assert.Equal(0, clientsCount);
        Assert.Equal(0, reservationsCount);
    }

    [Fact]
    public async Task Index_CalculatesActiveReservations()
    {
        var today = DateTime.Today;
        var activeCount = await _context.Reservations
            .CountAsync(r => r.DateDebut <= today && r.DateFin >= today);

        Assert.IsType<int>(activeCount);
    }
}
