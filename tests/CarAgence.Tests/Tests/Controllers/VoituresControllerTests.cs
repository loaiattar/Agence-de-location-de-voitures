using CarAgence.Data;
using CarAgence.Domain.Entities;
using CarAgence.Tests.Helpers;
using CarAgence.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CarAgence.Tests.Tests.Controllers;

public class VoituresControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly VoituresController _controller;

    public VoituresControllerTests()
    {
        _context = TestHelper.CreateInMemoryContext();
        _context.Database.EnsureCreated();
        _controller = new VoituresController(_context);
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
    public async Task Index_ReturnsViewWithAllVoitures()
    {
        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Voiture>>(viewResult.Model);
        Assert.Equal(6, model.Count);
    }

    [Fact]
    public async Task Index_IncludesModeleAndMarque()
    {
        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Voiture>>(viewResult.Model);
        foreach (var voiture in model)
        {
            Assert.NotNull(voiture.Modele);
            Assert.NotNull(voiture.Modele.Marque);
        }
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithVoiture()
    {
        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Voiture>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.Equal("AB-123-CD", model.Immatriculation);
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
    public async Task Details_IncludesNavigations()
    {
        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Voiture>(viewResult.Model);
        Assert.NotNull(model.Modele);
        Assert.NotNull(model.Modele.Marque);
    }

    [Fact]
    public async Task Create_Get_ReturnsView()
    {
        var result = await _controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.ViewData);
    }

    [Fact]
    public async Task Create_Post_WithValidModel_RedirectsToIndex()
    {
        var voiture = new Voiture
        {
            Immatriculation = "NEW-001-CAR",
            Annee = 2024,
            TarifJournalier = 50m,
            NombrePlaces = 5,
            Carburant = "Essence",
            ModeleId = 1
        };

        var result = await _controller.Create(voiture);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(VoituresController.Index), redirectToActionResult.ActionName);
    }

    [Fact]
    public async Task Create_Post_WithValidModel_AddsToDatabase()
    {
        var voiture = new Voiture
        {
            Immatriculation = "NEW-001-CAR",
            Annee = 2024,
            TarifJournalier = 50m,
            NombrePlaces = 5,
            Carburant = "Essence",
            ModeleId = 1
        };

        await _controller.Create(voiture);

        var dbVoiture = await _context.Voitures.FirstOrDefaultAsync(v => v.Immatriculation == "NEW-001-CAR");
        Assert.NotNull(dbVoiture);
        Assert.Equal(2024, dbVoiture.Annee);
        Assert.Equal(50m, dbVoiture.TarifJournalier);
    }

    [Fact]
    public async Task Edit_Get_WithValidId_ReturnsViewWithVoiture()
    {
        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Voiture>(viewResult.Model);
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
        var voiture = new Voiture
        {
            Immatriculation = "UPD-001-CAR",
            Annee = 2023,
            TarifJournalier = 40m,
            Carburant = "Diesel",
            ModeleId = 1
        };
        _context.Voitures.Add(voiture);
        await _context.SaveChangesAsync();
        voiture.TarifJournalier = 99m;
        _context.Entry(voiture).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var updated = await _context.Voitures.FindAsync(voiture.Id);
        Assert.Equal(99m, updated!.TarifJournalier);
    }

    [Fact]
    public async Task Delete_Get_WithValidId_ReturnsViewWithVoiture()
    {
        var result = await _controller.Delete(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Voiture>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.NotNull(model.Modele);
        Assert.NotNull(model.Modele.Marque);
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
        var voiture = new Voiture
        {
            Immatriculation = "DEL-001-CAR",
            Annee = 2023,
            TarifJournalier = 40m,
            Carburant = "Diesel",
            ModeleId = 1
        };
        _context.Voitures.Add(voiture);
        await _context.SaveChangesAsync();

        var result = await _controller.DeleteConfirmed(voiture.Id);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(VoituresController.Index), redirectToActionResult.ActionName);
        Assert.Null(await _context.Voitures.FindAsync(voiture.Id));
    }

    [Fact]
    public async Task DeleteConfirmed_WithNonExistentId_RedirectsToIndex()
    {
        var result = await _controller.DeleteConfirmed(999);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(VoituresController.Index), redirectToActionResult.ActionName);
    }
}
