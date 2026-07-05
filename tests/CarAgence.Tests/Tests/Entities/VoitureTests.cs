using CarAgence.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CarAgence.Tests.Tests.Entities;

public class VoitureTests
{
    [Fact]
    public void Voiture_DefaultProperties_AreInitialized()
    {
        var voiture = new Voiture();

        Assert.Equal(0, voiture.Id);
        Assert.Equal(string.Empty, voiture.Immatriculation);
        Assert.Equal(0, voiture.Annee);
        Assert.Equal(0, voiture.TarifJournalier);
        Assert.Equal(0, voiture.NombrePlaces);
        Assert.Equal(string.Empty, voiture.Carburant);
        Assert.Equal(0, voiture.ModeleId);
        Assert.NotNull(voiture.Reservations);
        Assert.Empty(voiture.Reservations);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Voiture_ImmatriculationRequired_InvalidWhenEmptyOrMissing(string? immat)
    {
        var voiture = new Voiture { Immatriculation = immat!, ModeleId = 1, Carburant = "Essence" };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(voiture);
        var isValid = Validator.TryValidateObject(voiture, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage!.Contains("Immatriculation"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Voiture_CarburantRequired_InvalidWhenEmptyOrMissing(string? carburant)
    {
        var voiture = new Voiture { Immatriculation = "AB-123-CD", ModeleId = 1, Carburant = carburant! };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(voiture);
        var isValid = Validator.TryValidateObject(voiture, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage!.Contains("Carburant"));
    }

    [Fact]
    public void Voiture_ModeleIdRequired_InvalidWhenZero()
    {
        var voiture = new Voiture { Immatriculation = "AB-123-CD", ModeleId = 0, Carburant = "Essence" };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(voiture);
        var isValid = Validator.TryValidateObject(voiture, context, results, true);

        Assert.False(isValid);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Voiture_ModeleIdRange_InvalidWhenLessThanOne(int modeleId)
    {
        var voiture = new Voiture { Immatriculation = "AB-123-CD", ModeleId = modeleId, Carburant = "Essence" };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(voiture);
        var isValid = Validator.TryValidateObject(voiture, context, results, true);

        Assert.False(isValid);
    }

    [Fact]
    public void Voiture_ValidVoiture_PassesValidation()
    {
        var voiture = new Voiture
        {
            Immatriculation = "AB-123-CD",
            Annee = 2022,
            TarifJournalier = 35m,
            NombrePlaces = 5,
            Carburant = "Essence",
            ModeleId = 1
        };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(voiture);
        var isValid = Validator.TryValidateObject(voiture, context, results, true);

        Assert.True(isValid);
    }

    [Fact]
    public void Voiture_TarifJournalier_CanBeDecimal()
    {
        var voiture = new Voiture
        {
            Immatriculation = "AB-123-CD",
            Carburant = "Essence",
            ModeleId = 1,
            TarifJournalier = 35.75m
        };

        Assert.Equal(35.75m, voiture.TarifJournalier);
    }

    [Fact]
    public void Voiture_ReservationsCollection_CanAddMultiple()
    {
        var voiture = new Voiture { Immatriculation = "AB-123-CD", ModeleId = 1, Carburant = "Essence" };
        voiture.Reservations.Add(new Reservation { ClientId = 1, VoitureId = 1, DateDebut = DateTime.Today, DateFin = DateTime.Today.AddDays(3) });
        voiture.Reservations.Add(new Reservation { ClientId = 2, VoitureId = 1, DateDebut = DateTime.Today.AddDays(5), DateFin = DateTime.Today.AddDays(8) });

        Assert.Equal(2, voiture.Reservations.Count);
    }
}
