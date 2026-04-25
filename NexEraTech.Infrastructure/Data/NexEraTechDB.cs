using Microsoft.EntityFrameworkCore;
using NexEraTech.Domain.Models;
using NexEraTech.Infrastructure.Data.Interface;
using System.Data;

namespace NexEraTech.Infrastructure.Data
{
    public class NexEraTechDB : DbContext, INexEraTechDB
    {
        public NexEraTechDB(DbContextOptions<NexEraTechDB> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().HasData(
                new Users { UserId = 1, FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", PhoneNumber = "+911234567890", CompanyName = "Alice Co", ProjectDescription = "Project A" },
                new Users { UserId = 2, FirstName = "Bob", LastName = "Johnson", Email = "bob@example.com", PhoneNumber = "+911234567891", CompanyName = "Bob Co", ProjectDescription = "Project B" }
            );

            base.OnModelCreating(modelBuilder);
        }
        public IDbConnection Connection => Database.GetDbConnection();

        public DbSet<Users> Users { get; set; }
    }
}
