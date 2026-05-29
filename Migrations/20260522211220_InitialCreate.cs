using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestionStock.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Reference = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    PrixUnitaire = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantiteEnStock = table.Column<int>(type: "INTEGER", nullable: false),
                    SeuilAlerte = table.Column<int>(type: "INTEGER", nullable: false),
                    DateAjout = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Actif = table.Column<bool>(type: "INTEGER", nullable: false),
                    CategorieId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produits_Categories_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MouvementsStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantite = table.Column<int>(type: "INTEGER", nullable: false),
                    Motif = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DateMouvement = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProduitId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MouvementsStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MouvementsStock_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Nom" },
                values: new object[,]
                {
                    { 1, "PC portables et desktops", "Ordinateurs" },
                    { 2, "Souris, claviers, écrans...", "Périphériques" },
                    { 3, "RAM, disques durs, processeurs...", "Composants" },
                    { 4, "Routeurs, switches, câbles...", "Réseaux" }
                });

            migrationBuilder.InsertData(
                table: "Produits",
                columns: new[] { "Id", "Actif", "CategorieId", "DateAjout", "Description", "Nom", "PrixUnitaire", "QuantiteEnStock", "Reference", "SeuilAlerte" },
                values: new object[,]
                {
                    { 1, true, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Intel Core i5, 8GB RAM, 256GB SSD", "Laptop Dell Inspiron 15", 650000m, 10, "PC-001", 3 },
                    { 2, true, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Souris filaire USB", "Souris Logitech M100", 8000m, 50, "PER-001", 10 },
                    { 3, true, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mémoire DDR4 2666MHz", "RAM Kingston 8GB DDR4", 25000m, 30, "COM-001", 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MouvementsStock_ProduitId",
                table: "MouvementsStock",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_Produits_CategorieId",
                table: "Produits",
                column: "CategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_Produits_Reference",
                table: "Produits",
                column: "Reference",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MouvementsStock");

            migrationBuilder.DropTable(
                name: "Produits");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
