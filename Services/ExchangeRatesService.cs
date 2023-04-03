using System.Collections.Generic;
using WebApplication2.Data;
using WebApplication2.Model;

namespace WebApplication2.Services
{
    public class ExchangeRatesService
    {
        ExchangeRatesDbContext _dbContext;

        public ExchangeRatesService (ExchangeRatesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void parseStringOfRatesByYear(string stringToParse)
        {
            int a = 0;            
        }
    }
}
