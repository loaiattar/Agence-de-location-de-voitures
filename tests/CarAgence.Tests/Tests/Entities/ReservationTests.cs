using CarAgence.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CarAgence.Tests.Tests.Entities;

public class ReservationTests
{
    [Fact]
    public void Reservation_DefaultProperties_AreInitialized()
    {
        var reservation = new Reservation();

        Assert.Equal(0, reservation.Id);
        Assert.Equal(default, reservation.DateDebut);
        Assert.Equal(default, reservation.DateFin);
        Assert.Equal(0, reservation.ClientId);
        Assert.Equal(0, reservation.VoitureId);
    }

    [Fact]
    public void Reservation_ClientIdRequired_InvalidWhenZero()
    {
        var reservation = new Reservation { DateDebut = DateTime.Today, DateFin = DateTime.Today.AddDays(3), ClientId = 0, VoitureId = 1 };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(reservation);
        var isValid = Validator.TryValidateObject(reservation, context, results, true);

        Assert.False(isValid);
    }

    [Fact]
    public void Reservation_VoitureIdRequired_InvalidWhenZero()
    {
        var reservation = new Reservation { DateDebut = DateTime.Today, DateFin = DateTime.Today.AddDays(3), ClientId = 1, VoitureId = 0 };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(reservation);
        var isValid = Validator.TryValidateObject(reservation, context, results, true);

        Assert.False(isValid);
    }

    [Fact]
    public void Reservation_ValidReservation_PassesValidation()
    {
        var reservation = new Reservation
        {
            DateDebut = DateTime.Today,
            DateFin = DateTime.Today.AddDays(3),
            ClientId = 1,
            VoitureId = 1
        };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(reservation);
        var isValid = Validator.TryValidateObject(reservation, context, results, true);

        Assert.True(isValid);
    }

    [Fact]
    public void GetMontantTotal_ReturnsCorrectAmount()
    {
        var voiture = new Voiture { TarifJournalier = 50m };
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 7, 1),
            DateFin = new DateTime(2025, 7, 5),
            Voiture = voiture
        };

        var total = reservation.GetMontantTotal();

        Assert.Equal(200m, total);
    }

    [Fact]
    public void GetMontantTotal_SingleDay_ReturnsTarifJournalier()
    {
        var voiture = new Voiture { TarifJournalier = 45m };
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 7, 1),
            DateFin = new DateTime(2025, 7, 2),
            Voiture = voiture
        };

        var total = reservation.GetMontantTotal();

        Assert.Equal(45m, total);
    }

    [Fact]
    public void GetMontantTotal_SameDay_ReturnsZero()
    {
        var voiture = new Voiture { TarifJournalier = 50m };
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 7, 1),
            DateFin = new DateTime(2025, 7, 1),
            Voiture = voiture
        };

        var total = reservation.GetMontantTotal();

        Assert.Equal(0m, total);
    }

    [Fact]
    public void GetMontantTotal_MultiWeek_ReturnsCorrectAmount()
    {
        var voiture = new Voiture { TarifJournalier = 35m };
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 7, 1),
            DateFin = new DateTime(2025, 7, 8),
            Voiture = voiture
        };

        var total = reservation.GetMontantTotal();

        Assert.Equal(245m, total);
    }

    [Fact]
    public void GetMontantTotal_FractionalTarif_ReturnsCorrectAmount()
    {
        var voiture = new Voiture { TarifJournalier = 33.50m };
        var reservation = new Reservation
        {
            DateDebut = new DateTime(2025, 7, 1),
            DateFin = new DateTime(2025, 7, 4),
            Voiture = voiture
        };

        var total = reservation.GetMontantTotal();

        Assert.Equal(100.50m, total);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Reservation_ClientIdRange_InvalidWhenLessThanOne(int clientId)
    {
        var reservation = new Reservation { DateDebut = DateTime.Today, DateFin = DateTime.Today.AddDays(3), ClientId = clientId, VoitureId = 1 };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(reservation);
        var isValid = Validator.TryValidateObject(reservation, context, results, true);

        Assert.False(isValid);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Reservation_VoitureIdRange_InvalidWhenLessThanOne(int voitureId)
    {
        var reservation = new Reservation { DateDebut = DateTime.Today, DateFin = DateTime.Today.AddDays(3), ClientId = 1, VoitureId = voitureId };
        var results = new List<ValidationResult>();
        var context = new ValidationContext(reservation);
        var isValid = Validator.TryValidateObject(reservation, context, results, true);

        Assert.False(isValid);
    }
}
