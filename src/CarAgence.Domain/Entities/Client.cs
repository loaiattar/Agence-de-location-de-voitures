using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CarAgence.Domain.Entities;

public class Client
{
    public int Id { get; set; }

    [Required]
    public string Nom { get; set; } = null!;

    [Required]
    public string Prenom { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    public string? Telephone { get; set; }

    [ValidateNever]
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
