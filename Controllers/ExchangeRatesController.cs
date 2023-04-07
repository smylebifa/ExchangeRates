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

        [HttpGet("/get_all_currencies")]
        public async Task<IEnumerable<ExchangeRate>> GetAllCurencies()
        {
            return await _exchangeRatesService.GetExchangeRatesAsync();
        }


        [HttpPost("/save_currencies_by_period/{first_date}/{last_date}")]
        public async Task<IEnumerable<ExchangeRate>>  SaveCurrenciesByPeriod(DateTime first_date, DateTime last_date)
        {
            return await _exchangeRatesService.parseExchangeRatesByPeriodAsync(first_date, last_date); 
        }

    }
}
