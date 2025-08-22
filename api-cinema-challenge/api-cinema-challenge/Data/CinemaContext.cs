using api_cinema_challenge.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace api_cinema_challenge.Data
{
    public class CinemaContext : DbContext
    {
        public CinemaContext(DbContextOptions<CinemaContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Seed data for the database
            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, Name = "Lionel Messi", Email = "messi@messi.messi", Phone = "90121413" },
                new Customer { Id = 2, Name = "Cristiano Ronaldo", Email = "ronaldo@ronaldo.ronaldo", Phone = "90121414" },
                new Customer { Id = 3, Name = "Wayne Rooney", Email = "rooney@rooney.rooney", Phone = "90121415" }
                );

             modelBuilder.Entity<Movie>().HasData(
                new Movie { Id = 1, Title = "Inception", Rating = "PG-13", Description = "A thief who steals corporate secrets through dream-sharing technology.", RuntimeMins = 148 },
                new Movie { Id = 2, Title = "The Matrix", Rating = "R", Description = "A computer hacker learns about the true nature of his reality.", RuntimeMins = 136 },
                new Movie { Id = 3, Title = "Interstellar", Rating = "PG-13", Description = "A team of explorers travel through a wormhole in space.", RuntimeMins = 169 }
            );

        }

        public DbSet<Customer> Customers { get; set; }
    }
}
