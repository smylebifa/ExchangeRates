using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Model;

namespace WebApplication2.Data
{
    public class ExchangeRatesDbContext : DbContext
    {
        public ExchangeRatesDbContext(DbContextOptions<ExchangeRatesDbContext> options) : base(options)
        {
        }
        public DbSet<ExchangeRate> ExchangeRates { get; set; } = null!;
        public DbSet<Currency> Currencies { get; set; } = null!;
        
    }
}
