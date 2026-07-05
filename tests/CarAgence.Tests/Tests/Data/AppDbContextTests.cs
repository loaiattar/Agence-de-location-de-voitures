using CarAgence.Data;
using CarAgence.Domain.Entities;
using CarAgence.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CarAgence.Tests.Tests.Data;

public class AppDbContextTests : IDisposable
{
    private readonly AppDbContext _context;

    public AppDbContextTests()
    {
        _context = TestHelper.CreateInMemoryContext();
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public void DbContext_HasAllDbSets()
    {
        Assert.NotNull(_context.Marques);
        Assert.NotNull(_context.Modeles);
        Assert.NotNull(_context.Voitures);
        Assert.NotNull(_context.Clients);
        Assert.NotNull(_context.Reservations);
    }

    [Fact]
    public async Task SeedData_MarquesAreSeeded()
    {
        var marques = await _context.Marques.ToListAsync();

        Assert.Equal(4, marques.Count);
        Assert.Contains(marques, m => m.Nom == "Renault");
        Assert.Contains(marques, m => m.Nom == "Peugeot");
        Assert.Contains(marques, m => m.Nom == "BMW");
        Assert.Contains(marques, m => m.Nom == "Citroën");
    }

    [Fact]
    public async Task SeedData_ModelesAreSeeded()
    {
        var modeles = await _context.Modeles.ToListAsync();

        Assert.Equal(6, modeles.Count);
        Assert.Contains(modeles, m => m.Nom == "Clio" && m.MarqueId == 1);
        Assert.Contains(modeles, m => m.Nom == "Megane" && m.MarqueId == 1);
        Assert.Contains(modeles, m => m.Nom == "208" && m.MarqueId == 2);
        Assert.Contains(modeles, m => m.Nom == "308" && m.MarqueId == 2);
        Assert.Contains(modeles, m => m.Nom == "Série 3" && m.MarqueId == 3);
        Assert.Contains(modeles, m => m.Nom == "C3" && m.MarqueId == 4);
    }

    [Fact]
    public async Task SeedData_VoituresAreSeeded()
    {
        var voitures = await _context.Voitures.ToListAsync();

        Assert.Equal(6, voitures.Count);
        Assert.Contains(voitures, v => v.Immatriculation == "AB-123-CD" && v.TarifJournalier == 35m);
        Assert.Contains(voitures, v => v.Immatriculation == "EF-456-GH" && v.TarifJournalier == 45m);
    }

    [Fact]
    public async Task SeedData_ClientsAreSeeded()
    {
        var clients = await _context.Clients.ToListAsync();

        Assert.Equal(2, clients.Count);
        Assert.Contains(clients, c => c.Nom == "Dupont" && c.Email == "jean.dupont@email.com");
        Assert.Contains(clients, c => c.Nom == "Martin" && c.Email == "sophie.martin@email.com");
    }

    [Fact]
    public async Task SeedData_ReservationsAreSeeded()
    {
        var reservations = await _context.Reservations.ToListAsync();

        Assert.Equal(2, reservations.Count);
        Assert.Contains(reservations, r => r.ClientId == 1 && r.VoitureId == 1);
        Assert.Contains(reservations, r => r.ClientId == 2 && r.VoitureId == 5);
    }

    [Fact]
    public async Task Marque_CanAddAndRetrieve()
    {
        var marque = new Marque { Nom = "TestMarque", PaysOrigine = "TestLand" };
        _context.Marques.Add(marque);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Marques.FindAsync(marque.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("TestMarque", retrieved.Nom);
        Assert.Equal("TestLand", retrieved.PaysOrigine);
    }

    [Fact]
    public async Task Modele_CanAddAndRetrieveWithNavigation()
    {
        var modele = new Modele { Nom = "TestModele", MarqueId = 1 };
        _context.Modeles.Add(modele);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Modeles
            .Include(m => m.Marque)
            .FirstOrDefaultAsync(m => m.Id == modele.Id);

        Assert.NotNull(retrieved);
        Assert.Equal("TestModele", retrieved.Nom);
        Assert.NotNull(retrieved.Marque);
        Assert.Equal("Renault", retrieved.Marque.Nom);
    }

    [Fact]
    public async Task Voiture_CanAddAndRetrieveWithNavigation()
    {
        var voiture = new Voiture
        {
            Immatriculation = "TEST-001",
            Annee = 2024,
            TarifJournalier = 50m,
            NombrePlaces = 5,
            Carburant = "Essence",
            ModeleId = 1
        };
        _context.Voitures.Add(voiture);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Voitures
            .Include(v => v.Modele)
            .FirstOrDefaultAsync(v => v.Id == voiture.Id);

        Assert.NotNull(retrieved);
        Assert.Equal("TEST-001", retrieved.Immatriculation);
        Assert.NotNull(retrieved.Modele);
        Assert.Equal("Clio", retrieved.Modele.Nom);
    }

    [Fact]
    public async Task Client_CanAddAndRetrieve()
    {
        var client = new Client { Nom = "TestClient", Prenom = "Test", Email = "testadd@test.com" };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Clients.FindAsync(client.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("TestClient", retrieved.Nom);
        Assert.Equal("testadd@test.com", retrieved.Email);
    }

    [Fact]
    public async Task Reservation_CanAddAndRetrieveWithNavigations()
    {
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 9, 1),
            DateFin = new DateTime(2025, 9, 5),
            ClientId = 1,
            VoitureId = 2
        };
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Reservations
            .Include(r => r.Client)
            .Include(r => r.Voiture)
            .FirstOrDefaultAsync(r => r.Id == reservation.Id);

        Assert.NotNull(retrieved);
        Assert.Equal("Dupont", retrieved.Client.Nom);
        Assert.Equal("EF-456-GH", retrieved.Voiture.Immatriculation);
    }

    [Fact]
    public async Task Marque_CanUpdate()
    {
        var marque = await _context.Marques.FindAsync(1);
        marque!.Nom = "UpdatedRenault";
        await _context.SaveChangesAsync();

        var updated = await _context.Marques.FindAsync(1);
        Assert.Equal("UpdatedRenault", updated!.Nom);
    }

    [Fact]
    public async Task Client_CanUpdate()
    {
        var client = await _context.Clients.FindAsync(1);
        client!.Nom = "UpdatedDupont";
        await _context.SaveChangesAsync();

        var updated = await _context.Clients.FindAsync(1);
        Assert.Equal("UpdatedDupont", updated!.Nom);
    }

    [Fact]
    public async Task Voiture_CanUpdate()
    {
        var voiture = await _context.Voitures.FindAsync(1);
        voiture!.TarifJournalier = 99m;
        await _context.SaveChangesAsync();

        var updated = await _context.Voitures.FindAsync(1);
        Assert.Equal(99m, updated!.TarifJournalier);
    }
}
