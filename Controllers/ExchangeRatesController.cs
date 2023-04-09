using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication2.Data;
using WebApplication2.Model;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly ILogger<ExchangeRatesController> _logger;
        ExchangeRatesService _exchangeRatesService;

        public ExchangeRatesController(ILogger<ExchangeRatesController> logger, ExchangeRatesService exchangeRatesService)
        {
            _logger = logger;
            _exchangeRatesService = exchangeRatesService;
        }

        [HttpPost("/save_currencies_by_period/{first_date}/{last_date}")]
        public IEnumerable<ExchangeRate> SaveCurrenciesByPeriod(DateTime first_date, DateTime last_date)
        {
            List<ExchangeRate> exchangeRates = _exchangeRatesService.parseExchangeRatesByPeriod(first_date, last_date);

            return exchangeRates; 
        }

        [HttpPost("/get_currency_data_by_period/{currency_code}/{first_date}/{last_date}")]
        public Dictionary<string, float> GetDataFromExchangeRate(string currency_code, DateTime first_date, DateTime last_date)
        {
            Dictionary<string, float> result = _exchangeRatesService.GetDataFromExchangeRate(currency_code, first_date, last_date);

            return result;
        }

        [HttpGet("/get_up_to_date_rates/{currency_code}")]
        public async Task<ExchangeRate> GetUpToDateExchangeRates(string currency_code)
        {
            return await _exchangeRatesService.GetUpToDateExchangeRates(currency_code);
        }

    }
}
