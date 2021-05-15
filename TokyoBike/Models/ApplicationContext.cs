using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokyoBike.Models.DbModels;

namespace TokyoBike.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Bike> Bikes { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<Rent> Rents { get; set; }
        public DbSet<Sightseen> Sightseens { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Statistics> Statistics { get; set; }
        public DbSet<User> Users { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Day>().OwnsOne(d => d.UserId, 
            {
                
            });
        }*/
    }
}
