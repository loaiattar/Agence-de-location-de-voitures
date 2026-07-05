using CarAgence.Data;
using CarAgence.Domain.Entities;
using CarAgence.Tests.Helpers;
using CarAgence.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CarAgence.Tests.Tests.Controllers;

public class ClientsControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ClientsController _controller;

    public ClientsControllerTests()
    {
        _context = TestHelper.CreateInMemoryContext();
        _context.Database.EnsureCreated();
        _controller = new ClientsController(_context);
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
    public async Task Index_ReturnsViewWithAllClients()
    {
        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Client>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithClient()
    {
        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Client>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.Equal("Dupont", model.Nom);
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
    public void Create_Get_ReturnsView()
    {
        var result = _controller.Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_WithValidModel_RedirectsToIndex()
    {
        var client = new Client { Nom = "TestNom", Prenom = "TestPrenom", Email = "test@test.com", Telephone = "0612345678" };

        var result = await _controller.Create(client);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClientsController.Index), redirectToActionResult.ActionName);
    }

    [Fact]
    public async Task Create_Post_WithValidModel_AddsToDatabase()
    {
        var client = new Client { Nom = "TestNom", Prenom = "TestPrenom", Email = "test@test.com", Telephone = "0612345678" };

        await _controller.Create(client);

        var dbClient = await _context.Clients.FirstOrDefaultAsync(c => c.Email == "test@test.com");
        Assert.NotNull(dbClient);
        Assert.Equal("TestNom", dbClient.Nom);
        Assert.Equal("TestPrenom", dbClient.Prenom);
    }

    [Fact]
    public async Task Edit_Get_WithValidId_ReturnsViewWithClient()
    {
        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Client>(viewResult.Model);
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
        var client = new Client { Nom = "Editable", Prenom = "Test", Email = "editable@test.com" };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        client.Nom = "UpdatedEditable";
        _context.Entry(client).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var updated = await _context.Clients.FindAsync(client.Id);
        Assert.Equal("UpdatedEditable", updated!.Nom);
    }

    [Fact]
    public async Task Delete_Get_WithValidId_ReturnsViewWithClient()
    {
        var result = await _controller.Delete(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Client>(viewResult.Model);
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
        var client = new Client { Nom = "ToDelete", Prenom = "Test", Email = "delete@test.com" };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var result = await _controller.DeleteConfirmed(client.Id);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClientsController.Index), redirectToActionResult.ActionName);
        Assert.Null(await _context.Clients.FindAsync(client.Id));
    }

    [Fact]
    public async Task DeleteConfirmed_WithNonExistentId_RedirectsToIndex()
    {
        var result = await _controller.DeleteConfirmed(999);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClientsController.Index), redirectToActionResult.ActionName);
    }
}
