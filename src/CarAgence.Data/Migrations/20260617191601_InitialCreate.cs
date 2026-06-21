using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarAgence.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Telephone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Marques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PaysOrigine = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marques", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modeles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MarqueId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modeles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modeles_Marques_MarqueId",
                        column: x => x.MarqueId,
                        principalTable: "Marques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Voitures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Immatriculation = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Annee = table.Column<int>(type: "INTEGER", nullable: false),
                    TarifJournalier = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NombrePlaces = table.Column<int>(type: "INTEGER", nullable: false),
                    Carburant = table.Column<string>(type: "TEXT", nullable: false),
                    ModeleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voitures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Voitures_Modeles_ModeleId",
                        column: x => x.ModeleId,
                        principalTable: "Modeles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateDebut = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateFin = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    VoitureId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_Voitures_VoitureId",
                        column: x => x.VoitureId,
                        principalTable: "Voitures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Email", "Nom", "Prenom", "Telephone" },
                values: new object[,]
                {
                    { 1, "jean.dupont@email.com", "Dupont", "Jean", "0612345678" },
                    { 2, "sophie.martin@email.com", "Martin", "Sophie", "0698765432" }
                });

            migrationBuilder.InsertData(
                table: "Marques",
                columns: new[] { "Id", "Nom", "PaysOrigine" },
                values: new object[,]
                {
                    { 1, "Renault", "France" },
                    { 2, "Peugeot", "France" },
                    { 3, "BMW", "Allemagne" },
                    { 4, "Citroën", "France" }
                });

            migrationBuilder.InsertData(
                table: "Modeles",
                columns: new[] { "Id", "MarqueId", "Nom" },
                values: new object[,]
                {
                    { 1, 1, "Clio" },
                    { 2, 1, "Megane" },
                    { 3, 2, "208" },
                    { 4, 2, "308" },
                    { 5, 3, "Série 3" },
                    { 6, 4, "C3" }
                });

            migrationBuilder.InsertData(
                table: "Voitures",
                columns: new[] { "Id", "Annee", "Carburant", "Immatriculation", "ModeleId", "NombrePlaces", "TarifJournalier" },
                values: new object[,]
                {
                    { 1, 2022, "Essence", "AB-123-CD", 1, 5, 35m },
                    { 2, 2023, "Diesel", "EF-456-GH", 2, 5, 45m },
                    { 3, 2023, "Essence", "IJ-789-KL", 3, 5, 38m },
                    { 4, 2021, "Diesel", "MN-012-OP", 4, 5, 55m },
                    { 5, 2023, "Essence", "QR-345-ST", 5, 5, 75m },
                    { 6, 2022, "Essence", "UV-678-WX", 6, 5, 32m }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "Id", "ClientId", "DateDebut", "DateFin", "VoitureId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 2, new DateTime(2025, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Email",
                table: "Clients",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Marques_Nom",
                table: "Marques",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modeles_MarqueId",
                table: "Modeles",
                column: "MarqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ClientId",
                table: "Reservations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_VoitureId",
                table: "Reservations",
                column: "VoitureId");

            migrationBuilder.CreateIndex(
                name: "IX_Voitures_Immatriculation",
                table: "Voitures",
                column: "Immatriculation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voitures_ModeleId",
                table: "Voitures",
                column: "ModeleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Voitures");

            migrationBuilder.DropTable(
                name: "Modeles");

            migrationBuilder.DropTable(
                name: "Marques");
        }
    }
}
