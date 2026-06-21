using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CarAgence.Domain.Entities;

public class Voiture
{
    public int Id { get; set; }

    [Required]
    public string Immatriculation { get; set; } = string.Empty;

    public int Annee { get; set; }

    public decimal TarifJournalier { get; set; }

    public int NombrePlaces { get; set; }

    [Required]
    public string Carburant { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int ModeleId { get; set; }

    [ValidateNever]
    public Modele Modele { get; set; } = null!;

    [ValidateNever]
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
