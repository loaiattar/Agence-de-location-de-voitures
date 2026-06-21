using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CarAgence.Domain.Entities;

public class Marque
{
    public int Id { get; set; }

    [Required]
    public string Nom { get; set; } = string.Empty;

    public string? PaysOrigine { get; set; }

    [ValidateNever]
    public ICollection<Modele> Modeles { get; set; } = new List<Modele>();
}
