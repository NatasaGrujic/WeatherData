using Microsoft.EntityFrameworkCore; 
using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherData.Models
{
   public class WeatherContext : DbContext
    {
        //Connectionssträng (lokal databas)
        const string connectionString = @"Server=(localdb)\MsSqlLocalDb;Database=TemperaturData;Trusted_Connection=True";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        public DbSet<Measurement> Measurements { get; set; }
    }
}
