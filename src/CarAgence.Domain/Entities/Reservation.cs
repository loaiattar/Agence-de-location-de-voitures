using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CarAgence.Domain.Entities;

public class Reservation
{
    public int Id { get; set; }

    [Required]
    public DateTime DateDebut { get; set; }

    [Required]
    public DateTime DateFin { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int ClientId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int VoitureId { get; set; }

    [ValidateNever]
    public Client Client { get; set; } = null!;

    [ValidateNever]
    public Voiture Voiture { get; set; } = null!;

    public decimal GetMontantTotal()
    {
        int days = (DateFin - DateDebut).Days;
        return Voiture.TarifJournalier * days;
    }
}
