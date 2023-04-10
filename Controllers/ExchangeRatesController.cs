using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Model;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly ExchangeRatesService _exchangeRatesService;

        public ExchangeRatesController(ExchangeRatesService exchangeRatesService)
        {
            _exchangeRatesService = exchangeRatesService;
        }
        /// <summary>
        /// Сохранение в базе данных курсов валют за период
        /// </summary>
        [HttpPost("/save_currencies/{first_date}/{last_date}")]
        public IEnumerable<ExchangeRate> SaveCurrencies(DateTime first_date, DateTime last_date)
        {
            List<ExchangeRate> exchangeRates = _exchangeRatesService.SaveParsedCurrencies(first_date, last_date);
            return exchangeRates; 
        }

        /// <summary>
        /// Получение минимального, максимального и среднего значение курса валюты за период
        /// </summary>
        [HttpPost("/exchange_rate_statistics/{currency_code}/{first_date}/{last_date}")]
        public Dictionary<string, double> ExchangeRateStatistics(string currency_code, DateTime first_date, DateTime last_date)
        {
            Dictionary<string, double> result = _exchangeRatesService.GetDataFromExchangeRate(currency_code, first_date, last_date);
            return result;
        }

        /// <summary>
        /// Добавление текущих курсов валют в базу данных по расписанию
        /// </summary>
        [HttpPost("/get_up_to_date_rates")]
        public async Task<ActionResult> GetUpToDateExchangeRates()
        {
            await _exchangeRatesService.GetUpToDateExchangeRates();
            return Ok();
        }

    }
}
