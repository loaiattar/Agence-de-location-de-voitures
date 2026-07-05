using CarAgence.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CarAgence.Tests.Tests.Entities;

public class ClientTests
{
    [Fact]
    public void Client_DefaultProperties_AreInitialized()
    {
        var client = new Client();

        Assert.Equal(0, client.Id);
        Assert.Null(client.Nom);
        Assert.Null(client.Prenom);
        Assert.Null(client.Email);
        Assert.Null(client.Telephone);
        Assert.NotNull(client.Reservations);
        Assert.Empty(client.Reservations);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Client_NomRequired_InvalidWhenEmptyOrMissing(string? nom)
    {
        var client = new Client { Nom = nom, Prenom = "Jean", Email = "jean@test.com" };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(client);
        var isValid = Validator.TryValidateObject(client, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage!.Contains("Nom"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Client_PrenomRequired_InvalidWhenEmptyOrMissing(string? prenom)
    {
        var client = new Client { Nom = "Dupont", Prenom = prenom, Email = "jean@test.com" };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(client);
        var isValid = Validator.TryValidateObject(client, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage!.Contains("Prenom"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Client_EmailRequired_InvalidWhenEmptyOrMissing(string? email)
    {
        var client = new Client { Nom = "Dupont", Prenom = "Jean", Email = email! };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(client);
        var isValid = Validator.TryValidateObject(client, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage!.Contains("Email"));
    }

    [Fact]
    public void Client_Telephone_IsOptional()
    {
        var client = new Client { Nom = "Dupont", Prenom = "Jean", Email = "jean@test.com" };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(client);
        var isValid = Validator.TryValidateObject(client, context, results, true);

        Assert.True(isValid);
        Assert.Null(client.Telephone);
    }

    [Fact]
    public void Client_ValidClient_PassesValidation()
    {
        var client = new Client { Nom = "Dupont", Prenom = "Jean", Email = "jean@test.com", Telephone = "0612345678" };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(client);
        var isValid = Validator.TryValidateObject(client, context, results, true);

        Assert.True(isValid);
    }

    [Fact]
    public void Client_ReservationsCollection_CanAddMultiple()
    {
        var client = new Client { Nom = "Dupont", Prenom = "Jean", Email = "jean@test.com" };
        client.Reservations.Add(new Reservation { ClientId = 1, VoitureId = 1, DateDebut = DateTime.Today, DateFin = DateTime.Today.AddDays(3) });
        client.Reservations.Add(new Reservation { ClientId = 1, VoitureId = 2, DateDebut = DateTime.Today.AddDays(5), DateFin = DateTime.Today.AddDays(8) });

        Assert.Equal(2, client.Reservations.Count);
    }
}
