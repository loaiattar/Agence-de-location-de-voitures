using CarAgence.Data;
using CarAgence.Domain.Entities;
using CarAgence.Tests.Helpers;
using CarAgence.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CarAgence.Tests.Tests.Controllers;

public class ReservationsControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ReservationsController _controller;

    public ReservationsControllerTests()
    {
        _context = TestHelper.CreateInMemoryContext();
        _context.Database.EnsureCreated();
        _controller = new ReservationsController(_context);
        SetupTempData();
    }

    public void Dispose()
    {
        _controller.Dispose();
        _context.Dispose();
    }

    private void SetupTempData()
    {
        var tempDataMock = new Mock<ITempDataDictionary>();
        _controller.TempData = tempDataMock.Object;
    }

    [Fact]
    public async Task Index_ReturnsViewWithAllReservations()
    {
        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Reservation>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task Index_IncludesClientAndVoitureNavigations()
    {
        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Reservation>>(viewResult.Model);
        foreach (var reservation in model)
        {
            Assert.NotNull(reservation.Client);
            Assert.NotNull(reservation.Voiture);
            Assert.NotNull(reservation.Voiture.Modele);
            Assert.NotNull(reservation.Voiture.Modele.Marque);
        }
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithReservation()
    {
        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Reservation>(viewResult.Model);
        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task Details_WithNullId_ReturnsNotFound()
    {
        var result = await _controller.Details(null);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_WithInvalidId_ReturnsNotFound()
    {
        var result = await _controller.Details(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_IncludesAllNavigations()
    {
        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Reservation>(viewResult.Model);
        Assert.NotNull(model.Client);
        Assert.NotNull(model.Voiture);
        Assert.NotNull(model.Voiture.Modele);
        Assert.NotNull(model.Voiture.Modele.Marque);
    }

    [Fact]
    public async Task Create_Get_ReturnsView()
    {
        var result = await _controller.Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_WithValidReservation_RedirectsToIndex()
    {
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 9, 10),
            DateFin = new DateTime(2025, 9, 15),
            ClientId = 1,
            VoitureId = 3
        };

        var result = await _controller.Create(reservation);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ReservationsController.Index), redirectToActionResult.ActionName);
    }

    [Fact]
    public async Task Create_Post_WithValidReservation_AddsToDatabase()
    {
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 9, 10),
            DateFin = new DateTime(2025, 9, 15),
            ClientId = 1,
            VoitureId = 3
        };

        await _controller.Create(reservation);

        var dbReservation = await _context.Reservations.FirstOrDefaultAsync(r =>
            r.ClientId == 1 && r.VoitureId == 3 &&
            r.DateDebut == new DateTime(2025, 9, 10));
        Assert.NotNull(dbReservation);
    }

    [Fact]
    public async Task Create_Post_WithOverlappingDates_ReturnsViewWithError()
    {
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 7, 2),
            DateFin = new DateTime(2025, 7, 4),
            ClientId = 2,
            VoitureId = 1
        };

        var result = await _controller.Create(reservation);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(_controller.ModelState[""]!.Errors, e => e.ErrorMessage!.Contains("already reserved"));
    }

    [Fact]
    public async Task Create_Post_WithInvalidDates_EndBeforeStart_ReturnsViewWithError()
    {
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 9, 10),
            DateFin = new DateTime(2025, 9, 5),
            ClientId = 1,
            VoitureId = 1
        };

        var result = await _controller.Create(reservation);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(_controller.ModelState[""]!.Errors, e => e.ErrorMessage!.Contains("End date must be after start date"));
    }

    [Fact]
    public async Task Create_Post_WithSameDates_EndEqualsStart_ReturnsViewWithError()
    {
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 9, 10),
            DateFin = new DateTime(2025, 9, 10),
            ClientId = 1,
            VoitureId = 1
        };

        var result = await _controller.Create(reservation);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Create_Post_WithNonOverlappingDates_Success()
    {
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 12, 1),
            DateFin = new DateTime(2025, 12, 5),
            ClientId = 1,
            VoitureId = 1
        };

        var result = await _controller.Create(reservation);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ReservationsController.Index), redirectToActionResult.ActionName);
    }

    [Fact]
    public async Task Create_Post_WithAdjacentDates_Success()
    {
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 7, 5),
            DateFin = new DateTime(2025, 7, 10),
            ClientId = 2,
            VoitureId = 1
        };

        var result = await _controller.Create(reservation);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ReservationsController.Index), redirectToActionResult.ActionName);
    }

    [Fact]
    public async Task Edit_Get_WithValidId_ReturnsViewWithReservation()
    {
        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Reservation>(viewResult.Model);
        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task Edit_Get_WithNullId_ReturnsNotFound()
    {
        var result = await _controller.Edit((int?)null);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Get_WithInvalidId_ReturnsNotFound()
    {
        var result = await _controller.Edit(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_WithValidModel_UpdatesDatabase()
    {
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 11, 1),
            DateFin = new DateTime(2025, 11, 5),
            ClientId = 1,
            VoitureId = 3
        };
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
        reservation.DateFin = new DateTime(2025, 11, 10);
        _context.Entry(reservation).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var updated = await _context.Reservations.FindAsync(reservation.Id);
        Assert.Equal(new DateTime(2025, 11, 10), updated!.DateFin);
    }

    [Fact]
    public async Task Edit_Post_WithOverlappingDates_ReturnsViewWithError()
    {
        var reservation = new Reservation
        {
            Id = 1,
            DateDebut = new DateTime(2025, 8, 11),
            DateFin = new DateTime(2025, 8, 14),
            ClientId = 2,
            VoitureId = 5
        };

        var result = await _controller.Edit(reservation);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Edit_Post_WithInvalidDates_EndBeforeStart_ReturnsViewWithError()
    {
        var reservation = new Reservation
        {
            Id = 1,
            DateDebut = new DateTime(2025, 7, 5),
            DateFin = new DateTime(2025, 7, 1),
            ClientId = 1,
            VoitureId = 1
        };

        var result = await _controller.Edit(reservation);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Delete_Get_WithValidId_ReturnsViewWithReservation()
    {
        var result = await _controller.Delete(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Reservation>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.NotNull(model.Client);
        Assert.NotNull(model.Voiture);
    }

    [Fact]
    public async Task Delete_Get_WithNullId_ReturnsNotFound()
    {
        var result = await _controller.Delete(null);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Get_WithInvalidId_ReturnsNotFound()
    {
        var result = await _controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_WithValidId_RemovesFromDatabase()
    {
        var result = await _controller.DeleteConfirmed(1);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ReservationsController.Index), redirectToActionResult.ActionName);
        Assert.Null(await _context.Reservations.FindAsync(1));
    }

    [Fact]
    public async Task DeleteConfirmed_WithNonExistentId_RedirectsToIndex()
    {
        var result = await _controller.DeleteConfirmed(999);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ReservationsController.Index), redirectToActionResult.ActionName);
    }
}
