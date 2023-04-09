using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Model;

namespace WebApplication2.Services
{
    public class ExchangeRatesService
    {
        private readonly ExchangeRatesDbContext _dbContext;
        private readonly ParseExchangeRatesService _parseExchangeRatesService;

        public ExchangeRatesService(ExchangeRatesDbContext dbContext, ParseExchangeRatesService parseExchangeRatesService)
        {
            _dbContext = dbContext;
            _parseExchangeRatesService = parseExchangeRatesService;
        }
        public async Task<List<ExchangeRate>> GetExchangeRatesAsync()
        {
            return await _dbContext.ExchangeRates.ToListAsync();
        }

        public Dictionary<string, float> GetDataFromExchangeRate(string currency_code, DateTime first_date, DateTime last_date)
        {
            List<ExchangeRate> exchangeRates = _dbContext.ExchangeRates.ToList();

            var ratesOfCurrency = from exchangeRate in exchangeRates
                                  where exchangeRate.CurrencyCode == currency_code &&
                                  exchangeRate.Date >= first_date && exchangeRate.Date <= last_date
                                  select exchangeRate.Rate;

            if (ratesOfCurrency == null)
                return null;

            else
            {
                float min = ratesOfCurrency.Min();
                float max = ratesOfCurrency.Max();
                float average = ratesOfCurrency.Average();

                Dictionary<string, float> result = new Dictionary<string, float>();
                result.Add("Мнимальное значение за период", min);
                result.Add("Максимальное значение за период", max);
                result.Add("Среднее значение за период", average);

                return result;
            }
        }

        public List<ExchangeRate> parseExchangeRatesByPeriod(DateTime first_date, DateTime last_date)
        {
            List<ExchangeRate> exchangeRates = _parseExchangeRatesService.parseExchangeRatesByPeriod(first_date, last_date);

            var exchangeRateTable = _dbContext.ExchangeRates.FirstOrDefault();
            if (exchangeRateTable == null)
            {
                ExchangeRate exchangeRate = exchangeRates[0];
                exchangeRate.Id = 1;
                _dbContext.ExchangeRates.Add(exchangeRate);
                _dbContext.SaveChanges();
            }

            foreach (ExchangeRate exchangeRate in exchangeRates)
            {
                // Проверка на дубликаты
                var foundedExchangeRate = _dbContext.ExchangeRates.FirstOrDefault(x => x.CurrencyCode == exchangeRate.CurrencyCode && 
                x.Rate == exchangeRate.Rate && x.Date == exchangeRate.Date);
               
                if (foundedExchangeRate != null)
                    continue;

                exchangeRate.Id = _dbContext.ExchangeRates.Max(x => x.Id) + 1;

                _dbContext.ExchangeRates.Add(exchangeRate);
                _dbContext.SaveChanges();
            }
            
            return exchangeRates;
        }
    }
}