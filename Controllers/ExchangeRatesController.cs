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
        private readonly ExchangeRatesDbContext _dbContext;

        //ExchangeRatesService _exchangeRatesService;


        public ExchangeRatesController(ILogger<ExchangeRatesController> logger, ExchangeRatesDbContext dbContext)
        {
            _logger = logger;
            //_exchangeRatesService = exchangeRatesService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExchangeRate>>> Get()
        {
            return await _dbContext.ExchangeRates.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
            WebClient web = new WebClient();
            string htmlStr = web.DownloadString("https://www.cnb.cz/en/financial_markets/foreign_exchange_market/exchange_rate_fixing/year.txt?year=2019");

            //_exchangeRatesService.parseStringOfRatesByYear(htmlStr);
                
            //ExchangeRate exchangeRate = new ExchangeRate { Id = 2, CurrencyUSD = "USD", Rate = 200f, Date = DateTime.Now };

            //_dbContext.ExchangeRates.Add(exchangeRate);
           await _dbContext.SaveChangesAsync();
            return Ok();
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
    }
}
