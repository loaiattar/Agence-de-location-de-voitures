using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CarAgence.Domain.Entities;

public class Modele
{
    public int Id { get; set; }

    [Required]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int MarqueId { get; set; }

    [ValidateNever]
    public Marque Marque { get; set; } = null!;

    [ValidateNever]
    public ICollection<Voiture> Voitures { get; set; } = new List<Voiture>();
}
