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

        }

        public DbSet<Customer> Customers { get; set; }
    }
}
