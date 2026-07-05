using CarAgence.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CarAgence.Tests.Tests.Entities;

public class ModeleTests
{
    [Fact]
    public void Modele_DefaultProperties_AreInitialized()
    {
        var modele = new Modele();

        Assert.Equal(0, modele.Id);
        Assert.Equal(string.Empty, modele.Nom);
        Assert.Equal(0, modele.MarqueId);
        Assert.NotNull(modele.Voitures);
        Assert.Empty(modele.Voitures);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Modele_NomRequired_InvalidWhenEmptyOrMissing(string? nom)
    {
        var modele = new Modele { Nom = nom!, MarqueId = 1 };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(modele);
        var isValid = Validator.TryValidateObject(modele, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage!.Contains("Nom"));
    }

    [Fact]
    public void Modele_MarqueIdRequired_InvalidWhenZero()
    {
        var modele = new Modele { Nom = "Clio", MarqueId = 0 };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(modele);
        var isValid = Validator.TryValidateObject(modele, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage!.Contains("MarqueId"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Modele_MarqueIdRange_InvalidWhenLessThanOne(int marqueId)
    {
        var modele = new Modele { Nom = "Clio", MarqueId = marqueId };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(modele);
        var isValid = Validator.TryValidateObject(modele, context, results, true);

        Assert.False(isValid);
    }

    [Fact]
    public void Modele_ValidModel_PassesValidation()
    {
        var modele = new Modele { Nom = "Clio", MarqueId = 1 };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(modele);
        var isValid = Validator.TryValidateObject(modele, context, results, true);

        Assert.True(isValid);
    }

    [Fact]
    public void Modele_VoituresCollection_CanAddMultiple()
    {
        var modele = new Modele { Nom = "Clio", MarqueId = 1 };
        modele.Voitures.Add(new Voiture { Immatriculation = "AB-123-CD", ModeleId = 1 });
        modele.Voitures.Add(new Voiture { Immatriculation = "EF-456-GH", ModeleId = 1 });

        Assert.Equal(2, modele.Voitures.Count);
    }
}
