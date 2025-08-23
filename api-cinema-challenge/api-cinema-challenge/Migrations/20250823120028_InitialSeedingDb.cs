using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api_cinema_challenge.Migrations
{
    /// <inheritdoc />
    public partial class InitialSeedingDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerName = table.Column<string>(type: "text", nullable: false),
                    CustomerEmail = table.Column<string>(type: "text", nullable: false),
                    CustomerPhone = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MovieTitle = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<string>(type: "text", nullable: false),
                    MovieDescription = table.Column<string>(type: "text", nullable: false),
                    RuntimeMinutes = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.MovieId);
                });

            migrationBuilder.CreateTable(
                name: "Screenings",
                columns: table => new
                {
                    ScreeningId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScreenNumber = table.Column<int>(type: "integer", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MovieId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Screenings", x => x.ScreeningId);
                    table.ForeignKey(
                        name: "FK_Screenings_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumsSeats = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScreeningId = table.Column<int>(type: "integer", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_Tickets_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_Screenings_ScreeningId",
                        column: x => x.ScreeningId,
                        principalTable: "Screenings",
                        principalColumn: "ScreeningId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "CreatedAt", "CustomerEmail", "CustomerName", "CustomerPhone", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc), "messi@messi.messi", "Lionel Messi", "90121413", new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc) },
                    { 2, new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc), "ronaldo@ronaldo.ronaldo", "Cristiano Ronaldo", "90121414", new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc) },
                    { 3, new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc), "rooney@rooney.rooney", "Wayne Rooney", "90121415", new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "MovieId", "CreatedAt", "MovieDescription", "Rating", "RuntimeMinutes", "MovieTitle", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2010, 7, 16, 11, 1, 56, 633, DateTimeKind.Utc), "A thief who steals corporate secrets through dream-sharing technology.", "PG-13", 148, "Inception", new DateTime(2010, 7, 16, 11, 1, 56, 633, DateTimeKind.Utc) },
                    { 2, new DateTime(1999, 3, 31, 11, 1, 56, 633, DateTimeKind.Utc), "A computer hacker learns about the true nature of his reality.", "R", 136, "The Matrix", new DateTime(1999, 3, 31, 11, 1, 56, 633, DateTimeKind.Utc) },
                    { 3, new DateTime(2014, 11, 7, 11, 1, 56, 633, DateTimeKind.Utc), "A team of explorers travel through a wormhole in space.", "PG-13", 169, "Interstellar", new DateTime(2014, 11, 7, 11, 1, 56, 633, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Screenings",
                columns: new[] { "ScreeningId", "Capacity", "CreatedAt", "MovieId", "ScreenNumber", "StartsAt", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 40, new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc), 1, 5, new DateTime(2023, 3, 19, 11, 30, 0, 0, DateTimeKind.Utc), new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc) },
                    { 2, 60, new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc), 2, 3, new DateTime(2023, 3, 20, 15, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc) },
                    { 3, 30, new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc), 3, 7, new DateTime(2023, 3, 21, 18, 45, 0, 0, DateTimeKind.Utc), new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "TicketId", "CreatedAt", "CustomerId", "NumsSeats", "ScreeningId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc), 1, 2, 1, new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc) },
                    { 2, new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc), 2, 4, 1, new DateTime(2023, 3, 14, 11, 1, 56, 633, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Screenings_MovieId",
                table: "Screenings",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CustomerId",
                table: "Tickets",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ScreeningId",
                table: "Tickets",
                column: "ScreeningId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Screenings");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
