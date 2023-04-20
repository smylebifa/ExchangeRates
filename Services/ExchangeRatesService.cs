using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Model;

namespace WebApplication2.Services
{
    public class ExchangeRatesService
    {
        private readonly ExchangeRatesDbContext _dbContext;
        private readonly ParseExchangeRatesService _parseExchangeRatesService;
        private readonly SheduleService _sheduleService;

        public ExchangeRatesService(ExchangeRatesDbContext dbContext, ParseExchangeRatesService parseExchangeRatesService, SheduleService sheduleService)
        {
            _dbContext = dbContext;
            _parseExchangeRatesService = parseExchangeRatesService;
            _sheduleService = sheduleService;
        }

        public async Task GetUpToDateExchangeRates()
        {
           await _sheduleService.GetUpToDateExchangeRates();
        }

        public Dictionary<string, double> GetDataFromExchangeRate(string currency_code, DateTime first_date, DateTime last_date)
        {
            List<ExchangeRate> exchangeRates = _dbContext.ExchangeRates.ToList();

            var ratesOfCurrency = from exchangeRate in exchangeRates
                                  where exchangeRate.CurrencyCode == currency_code &&
                                  exchangeRate.Date >= first_date && exchangeRate.Date <= last_date
                                  select exchangeRate.Rate;

            if (ratesOfCurrency == null || ratesOfCurrency.Count() == 0)
                return null;

            else
            {
                double min = ratesOfCurrency.Min();
                double max = ratesOfCurrency.Max();
                double average = ratesOfCurrency.Average();

                Dictionary<string, double> result = new Dictionary<string, double>();
                result.Add("Мнимальное значение за период", min);
                result.Add("Максимальное значение за период", max);
                result.Add("Среднее значение за период", average);

                return result;
            }
        }

        public List<ExchangeRate> SaveParsedCurrencies(DateTime first_date, DateTime last_date)
        {
            List<ExchangeRate> exchangeRates = _parseExchangeRatesService.ParseExchangeRatesByPeriod(first_date, last_date);
            SaveExchangeRate(exchangeRates);
            return exchangeRates;
        }

        private void SaveExchangeRate(List<ExchangeRate> exchangeRates)
        {
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
                var foundedExchangeRate = _dbContext.ExchangeRates.FirstOrDefault(x => 
                x.CurrencyCode == exchangeRate.CurrencyCode && 
                x.Rate == exchangeRate.Rate && x.Date == exchangeRate.Date);

                if (foundedExchangeRate != null)
                    continue;

                exchangeRate.Id = _dbContext.ExchangeRates.Max(x => x.Id) + 1;

                _dbContext.ExchangeRates.Add(exchangeRate);
                _dbContext.SaveChanges();
            }
        }
    }

}