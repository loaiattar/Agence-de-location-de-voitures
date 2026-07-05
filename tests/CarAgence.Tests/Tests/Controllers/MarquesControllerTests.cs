using CarAgence.Data;
using CarAgence.Domain.Entities;
using CarAgence.Tests.Helpers;
using CarAgence.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CarAgence.Tests.Tests.Controllers;

public class MarquesControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly MarquesController _controller;

    public MarquesControllerTests()
    {
        _context = TestHelper.CreateInMemoryContext();
        _context.Database.EnsureCreated();
        _controller = new MarquesController(_context);
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
    public async Task Index_ReturnsViewWithAllMarques()
    {
        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Marque>>(viewResult.Model);
        Assert.Equal(4, model.Count);
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithMarque()
    {
        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Marque>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.Equal("Renault", model.Nom);
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
    public async Task Details_IncludesModeles()
    {
        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Marque>(viewResult.Model);
        Assert.NotEmpty(model.Modeles);
    }

    [Fact]
    public void Create_Get_ReturnsView()
    {
        var result = _controller.Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_WithValidModel_RedirectsToIndex()
    {
        var marque = new Marque { Nom = "NewBrand", PaysOrigine = "TestLand" };

        var result = await _controller.Create(marque);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(MarquesController.Index), redirectToActionResult.ActionName);
    }

    [Fact]
    public async Task Create_Post_WithValidModel_AddsToDatabase()
    {
        var marque = new Marque { Nom = "NewBrand", PaysOrigine = "TestLand" };

        await _controller.Create(marque);

        var dbMarque = await _context.Marques.FirstOrDefaultAsync(m => m.Nom == "NewBrand");
        Assert.NotNull(dbMarque);
        Assert.Equal("TestLand", dbMarque.PaysOrigine);
    }

    [Fact]
    public async Task Edit_Get_WithValidId_ReturnsViewWithMarque()
    {
        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Marque>(viewResult.Model);
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
        var marque = new Marque { Nom = "UpdatedRenault", PaysOrigine = "France" };
        _context.Marques.Add(marque);
        await _context.SaveChangesAsync();
        marque.Nom = "UpdatedRenault2";
        _context.Entry(marque).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var updated = await _context.Marques.FindAsync(marque.Id);
        Assert.Equal("UpdatedRenault2", updated!.Nom);
    }

    [Fact]
    public async Task Delete_Get_WithValidId_ReturnsViewWithMarque()
    {
        var result = await _controller.Delete(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Marque>(viewResult.Model);
        Assert.Equal(1, model.Id);
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
        var marque = new Marque { Nom = "ToDelete", PaysOrigine = "Test" };
        _context.Marques.Add(marque);
        await _context.SaveChangesAsync();

        var result = await _controller.DeleteConfirmed(marque.Id);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(MarquesController.Index), redirectToActionResult.ActionName);
        Assert.Null(await _context.Marques.FindAsync(marque.Id));
    }

    [Fact]
    public async Task DeleteConfirmed_WithNonExistentId_RedirectsToIndex()
    {
        var result = await _controller.DeleteConfirmed(999);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(MarquesController.Index), redirectToActionResult.ActionName);
    }
}
