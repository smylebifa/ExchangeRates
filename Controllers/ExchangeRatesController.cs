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

        [HttpPost("/save_currencies/{first_date}/{last_date}")]
        public IEnumerable<ExchangeRate> SaveCurrencies(DateTime first_date, DateTime last_date)
        {
            List<ExchangeRate> exchangeRates = _exchangeRatesService.saveParsedCurrencies(first_date, last_date);
            return exchangeRates; 
        }


        [HttpPost("/exchange_rate_statistics/{currency_code}/{first_date}/{last_date}")]
        public Dictionary<string, double> ExchangeRateStatistics(string currency_code, DateTime first_date, DateTime last_date)
        {
            Dictionary<string, double> result = _exchangeRatesService.GetDataFromExchangeRate(currency_code, first_date, last_date);
            return result;
        }


        [HttpGet("/get_up_to_date_rates")]
        public async Task GetUpToDateExchangeRates()
        {
            await _exchangeRatesService.GetUpToDateExchangeRates();
        }

    }
}
