using CarAgence.Data;
using CarAgence.Domain.Entities;
using CarAgence.Tests.Helpers;
using CarAgence.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CarAgence.Tests.Tests.Controllers;

public class ModelesControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ModelesController _controller;

    public ModelesControllerTests()
    {
        _context = TestHelper.CreateInMemoryContext();
        _context.Database.EnsureCreated();
        _controller = new ModelesController(_context);
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
    public async Task Index_ReturnsViewWithAllModeles()
    {
        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Modele>>(viewResult.Model);
        Assert.Equal(6, model.Count);
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithModele()
    {
        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Modele>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.Equal("Clio", model.Nom);
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
    public async Task Details_IncludesMarqueAndVoitures()
    {
        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Modele>(viewResult.Model);
        Assert.NotNull(model.Marque);
        Assert.NotNull(model.Voitures);
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
        var modele = new Modele { Nom = "NewModel", MarqueId = 1 };

        var result = await _controller.Create(modele);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ModelesController.Index), redirectToActionResult.ActionName);
    }

    [Fact]
    public async Task Create_Post_WithValidModel_AddsToDatabase()
    {
        var modele = new Modele { Nom = "NewModel", MarqueId = 1 };

        await _controller.Create(modele);

        var dbModele = await _context.Modeles.FirstOrDefaultAsync(m => m.Nom == "NewModel");
        Assert.NotNull(dbModele);
        Assert.Equal(1, dbModele.MarqueId);
    }

    [Fact]
    public async Task Edit_Get_WithValidId_ReturnsViewWithModele()
    {
        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Modele>(viewResult.Model);
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
        var modele = new Modele { Nom = "UpdatedModele", MarqueId = 1 };
        _context.Modeles.Add(modele);
        await _context.SaveChangesAsync();
        modele.Nom = "UpdatedModele2";
        _context.Entry(modele).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var updated = await _context.Modeles.FindAsync(modele.Id);
        Assert.Equal("UpdatedModele2", updated!.Nom);
    }

    [Fact]
    public async Task Delete_Get_WithValidId_ReturnsViewWithModele()
    {
        var result = await _controller.Delete(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Modele>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.NotNull(model.Marque);
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
        var modele = new Modele { Nom = "ToDelete", MarqueId = 1 };
        _context.Modeles.Add(modele);
        await _context.SaveChangesAsync();

        var result = await _controller.DeleteConfirmed(modele.Id);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ModelesController.Index), redirectToActionResult.ActionName);
        Assert.Null(await _context.Modeles.FindAsync(modele.Id));
    }

    [Fact]
    public async Task DeleteConfirmed_WithNonExistentId_RedirectsToIndex()
    {
        var result = await _controller.DeleteConfirmed(999);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ModelesController.Index), redirectToActionResult.ActionName);
    }
}
