using CarAgence.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CarAgence.Tests.Tests.Entities;

public class MarqueTests
{
    [Fact]
    public void Marque_DefaultProperties_AreInitialized()
    {
        var marque = new Marque();

        Assert.Equal(0, marque.Id);
        Assert.Equal(string.Empty, marque.Nom);
        Assert.Null(marque.PaysOrigine);
        Assert.NotNull(marque.Modeles);
        Assert.Empty(marque.Modeles);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Marque_NomRequired_InvalidWhenEmptyOrMissing(string? nom)
    {
        var marque = new Marque { Nom = nom! };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(marque);
        var isValid = Validator.TryValidateObject(marque, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage!.Contains("Nom"));
    }

    [Fact]
    public void Marque_NomRequired_ValidWhenProvided()
    {
        var marque = new Marque { Nom = "Renault" };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(marque);
        var isValid = Validator.TryValidateObject(marque, context, results, true);

        Assert.True(isValid);
    }

    [Fact]
    public void Marque_PaysOrigine_IsOptional()
    {
        var marque = new Marque { Nom = "Renault" };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(marque);
        var isValid = Validator.TryValidateObject(marque, context, results, true);

        Assert.True(isValid);
        Assert.Null(marque.PaysOrigine);
    }

    [Fact]
    public void Marque_ModelesCollection_CanAddMultiple()
    {
        var marque = new Marque { Nom = "Renault" };
        marque.Modeles.Add(new Modele { Nom = "Clio", MarqueId = 1 });
        marque.Modeles.Add(new Modele { Nom = "Megane", MarqueId = 1 });

        Assert.Equal(2, marque.Modeles.Count);
    }
}
