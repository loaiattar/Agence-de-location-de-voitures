using CarAgence.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarAgence.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Marque> Marques => Set<Marque>();
    public DbSet<Modele> Modeles => Set<Modele>();
    public DbSet<Voiture> Voitures => Set<Voiture>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Marque
        modelBuilder.Entity<Marque>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Nom).HasMaxLength(100);
            e.HasIndex(m => m.Nom).IsUnique();
        });

        // Modele
        modelBuilder.Entity<Modele>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Nom).HasMaxLength(100);
            e.HasOne(m => m.Marque)
             .WithMany(mar => mar.Modeles)
             .HasForeignKey(m => m.MarqueId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Voiture
        modelBuilder.Entity<Voiture>(e =>
        {
            e.HasKey(v => v.Id);
            e.Property(v => v.Immatriculation).HasMaxLength(20);
            e.Property(v => v.TarifJournalier).HasColumnType("decimal(18,2)");
            e.HasIndex(v => v.Immatriculation).IsUnique();
            e.HasOne(v => v.Modele)
             .WithMany(mod => mod.Voitures)
             .HasForeignKey(v => v.ModeleId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Client
        modelBuilder.Entity<Client>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Email).HasMaxLength(200);
            e.HasIndex(c => c.Email).IsUnique();
        });

        // Reservation
        modelBuilder.Entity<Reservation>(e =>
        {
            e.HasKey(r => r.Id);
            e.HasOne(r => r.Client)
             .WithMany(c => c.Reservations)
             .HasForeignKey(r => r.ClientId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Voiture)
             .WithMany(v => v.Reservations)
             .HasForeignKey(r => r.VoitureId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Marque>().HasData(
            new Marque { Id = 1, Nom = "Renault", PaysOrigine = "France" },
            new Marque { Id = 2, Nom = "Peugeot", PaysOrigine = "France" },
            new Marque { Id = 3, Nom = "BMW", PaysOrigine = "Allemagne" },
            new Marque { Id = 4, Nom = "Citroën", PaysOrigine = "France" }
        );

        modelBuilder.Entity<Modele>().HasData(
            new Modele { Id = 1, Nom = "Clio", MarqueId = 1 },
            new Modele { Id = 2, Nom = "Megane", MarqueId = 1 },
            new Modele { Id = 3, Nom = "208", MarqueId = 2 },
            new Modele { Id = 4, Nom = "308", MarqueId = 2 },
            new Modele { Id = 5, Nom = "Série 3", MarqueId = 3 },
            new Modele { Id = 6, Nom = "C3", MarqueId = 4 }
        );

        modelBuilder.Entity<Voiture>().HasData(
            new Voiture { Id = 1, Immatriculation = "AB-123-CD", Annee = 2022, TarifJournalier = 35m, NombrePlaces = 5, Carburant = "Essence", ModeleId = 1 },
            new Voiture { Id = 2, Immatriculation = "EF-456-GH", Annee = 2023, TarifJournalier = 45m, NombrePlaces = 5, Carburant = "Diesel", ModeleId = 2 },
            new Voiture { Id = 3, Immatriculation = "IJ-789-KL", Annee = 2023, TarifJournalier = 38m, NombrePlaces = 5, Carburant = "Essence", ModeleId = 3 },
            new Voiture { Id = 4, Immatriculation = "MN-012-OP", Annee = 2021, TarifJournalier = 55m, NombrePlaces = 5, Carburant = "Diesel", ModeleId = 4 },
            new Voiture { Id = 5, Immatriculation = "QR-345-ST", Annee = 2023, TarifJournalier = 75m, NombrePlaces = 5, Carburant = "Essence", ModeleId = 5 },
            new Voiture { Id = 6, Immatriculation = "UV-678-WX", Annee = 2022, TarifJournalier = 32m, NombrePlaces = 5, Carburant = "Essence", ModeleId = 6 }
        );

        modelBuilder.Entity<Client>().HasData(
            new Client { Id = 1, Nom = "Dupont", Prenom = "Jean", Email = "jean.dupont@email.com", Telephone = "0612345678" },
            new Client { Id = 2, Nom = "Martin", Prenom = "Sophie", Email = "sophie.martin@email.com", Telephone = "0698765432" }
        );

        modelBuilder.Entity<Reservation>().HasData(
            new Reservation { Id = 1, DateDebut = new DateTime(2025, 7, 1), DateFin = new DateTime(2025, 7, 5), ClientId = 1, VoitureId = 1 },
            new Reservation { Id = 2, DateDebut = new DateTime(2025, 8, 10), DateFin = new DateTime(2025, 8, 15), ClientId = 2, VoitureId = 5 }
        );
    }
}
