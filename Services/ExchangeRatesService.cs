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
        private string WebPathOfExchangeRatesByPeriod =
            "https://www.cnb.cz/en/financial_markets/foreign_exchange_market/exchange_rate_fixing/year.txt?year=";

        public ExchangeRatesService(ExchangeRatesDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<ExchangeRate>> GetExchangeRatesAsync()
        {
            return await _dbContext.ExchangeRates.ToListAsync();
        }

        public async Task<List<ExchangeRate>> parseExchangeRatesByPeriodAsync(DateTime first_date, DateTime last_date)
        {
            string firstDateStr = first_date.ToString("yyyy-MM-dd");
            string lastDateStr = last_date.ToString("yyyy-MM-dd");

            string firstYearStr = firstDateStr.Substring(0, 4);
            string lastYearStr = lastDateStr.Substring(0, 4);

            string firstMonthStr = firstDateStr.Substring(5, 2);
            string lastMonthStr = lastDateStr.Substring(5, 2);

            string firstDayStr = firstDateStr.Substring(8, 2);
            string lastDayStr = lastDateStr.Substring(8, 2);


            int firstYearInt = Int32.Parse(firstYearStr);
            int lastYearInt = Int32.Parse(lastYearStr);

            int firstMonth = Int32.Parse(firstMonthStr);
            int lastMonth = Int32.Parse(lastMonthStr);

            int firstDay = Int32.Parse(firstDayStr);
            int lastDay = Int32.Parse(lastDayStr);

            List<ExchangeRate> exchangeRatesObj = new List<ExchangeRate>();

            WebClient web = new WebClient();
            string downloadedString;
            string[] currencies = new string[35];

            int countOfYears = lastYearInt - firstYearInt;

            for (int i = 0; i <= countOfYears; i++)
            {
                string yearToParseStr = (firstYearInt + i).ToString();
                downloadedString = web.DownloadString(WebPathOfExchangeRatesByPeriod + yearToParseStr);

                string[] stringsWithExchangeRates = downloadedString.Split('\n');


                // Если заданные года начала и конца периода разные, то есть два варианта проверки
                // 1 вариант - текущий год совпадает с начальным годом, тогда будет сравнивать текущую дату с начальной датой
                // 2 вариант - текущий год совпадает с конечным годом, тогда будет сравнивать текущую дату с конечной датой
                // 3 вариант - текущий год совпадает находится между начальным и конечным годом, тогда не сравниваем текущую дату, 
                // а просто записываем все данные за весь год
                if (countOfYears > 0)
                {
                    // 1 вариант
                    if (i == 0)
                    {

                        // Обход строк по датам
                        foreach (string stringWithExchangeRates in stringsWithExchangeRates)
                        {
                            string[] exchangeRates = stringWithExchangeRates.Split('|');
                            if (exchangeRates[0] == "Date")
                            {
                                // Обход курсов валют по строке - дате
                                for (int ic = 1; ic < exchangeRates.Length; ic++)
                                {
                                    currencies[ic - 1] = exchangeRates[ic];
                                }
                            }

                            else
                            {
                                string dateStr = exchangeRates[0];

                                if (dateStr != "")
                                {
                                    string dayStr = dateStr.Substring(0, 2);
                                    string monthStr = dateStr.Substring(3, 2);

                                    int day = Int32.Parse(dayStr);
                                    int month = Int32.Parse(monthStr);

                                    if (month >= firstMonth && day >= firstDay)
                                    {
                                        //Обход курсов валют по строке - дате
                                        string exchangeRate;
                                        float rate;
                                        DateTime date;
                                        for (int ic = 1; ic < exchangeRates.Length; ic++)
                                        {
                                            exchangeRate = exchangeRates[ic];
                                            rate = float.Parse(exchangeRate, CultureInfo.InvariantCulture.NumberFormat);
                                            date = DateTime.Parse(dateStr);

                                            ExchangeRate exchangeRateObj = new ExchangeRate()
                                            {
                                                Id = ic,
                                                Date = date,
                                                CurrencyCode = currencies[ic - 1],
                                                Rate = rate
                                            };

                                            exchangeRatesObj.Add(exchangeRateObj);
                                        }
                                    }

                                }

                            }

                        }
                    }

                    // 2 вариант
                    else if (i == countOfYears)
                    {
                        // Обход строк по датам
                        foreach (string stringWithExchangeRates in stringsWithExchangeRates)
                        {
                            string[] exchangeRates = stringWithExchangeRates.Split('|');
                            if (exchangeRates[0] == "Date")
                            {
                                // Обход курсов валют по строке - дате
                                for (int ic = 1; ic < exchangeRates.Length; ic++)
                                {
                                    currencies[ic - 1] = exchangeRates[ic];
                                }
                            }

                            else
                            {
                                string dateStr = exchangeRates[0];

                                if (dateStr != "")
                                {
                                    string dayStr = dateStr.Substring(0, 2);
                                    string monthStr = dateStr.Substring(3, 2);

                                    int day = Int32.Parse(dayStr);
                                    int month = Int32.Parse(monthStr);

                                    if (month <= lastMonth && day <= lastDay)
                                    {
                                        //Обход курсов валют по строке - дате
                                        string exchangeRate;
                                        float rate;
                                        DateTime date;
                                        for (int ic = 1; ic < exchangeRates.Length; ic++)
                                        {
                                            exchangeRate = exchangeRates[ic];
                                            rate = float.Parse(exchangeRate, CultureInfo.InvariantCulture.NumberFormat);
                                            date = DateTime.Parse(dateStr);

                                            ExchangeRate exchangeRateObj = new ExchangeRate()
                                            {
                                                Id = ic,
                                                Date = date,
                                                CurrencyCode = currencies[ic - 1],
                                                Rate = rate
                                            };

                                            exchangeRatesObj.Add(exchangeRateObj);
                                        }
                                    }

                                    else break;
                                }

                            }

                        }
                    }

                    // 3 вариант
                    else
                    {
                        // Обход строк по датам
                        foreach (string stringWithExchangeRates in stringsWithExchangeRates)
                        {
                            string[] exchangeRates = stringWithExchangeRates.Split('|');
                            if (exchangeRates[0] == "Date")
                            {
                                // Обход курсов валют по строке - дате
                                for (int ic = 1; ic < exchangeRates.Length; ic++)
                                {
                                    currencies[ic - 1] = exchangeRates[ic];
                                }
                            }

                            else
                            {
                                string dateStr = exchangeRates[0];

                                if (dateStr != "")
                                {
                                    //Обход курсов валют по строке - дате
                                    string exchangeRate;
                                    float rate;
                                    DateTime date;
                                    for (int ic = 1; ic < exchangeRates.Length; ic++)
                                    {
                                        exchangeRate = exchangeRates[ic];
                                        rate = float.Parse(exchangeRate, CultureInfo.InvariantCulture.NumberFormat);
                                        date = DateTime.Parse(dateStr);

                                        ExchangeRate exchangeRateObj = new ExchangeRate()
                                        {
                                            Id = ic,
                                            Date = date,
                                            CurrencyCode = currencies[ic - 1],
                                            Rate = rate
                                        };

                                        //2019.12.20

                                        exchangeRatesObj.Add(exchangeRateObj);
                                    }
                                }

                            }
                        }
                    }

                }

                // Если год начала и конца совпадают, то сравниваем текущую дату с первой и последней датой 
                // (нужно дойти построчно до начальной и конечной даты периода)
                else
                {
                    // Обход строк по датам
                    foreach (string stringWithExchangeRates in stringsWithExchangeRates)
                    {
                        string[] exchangeRates = stringWithExchangeRates.Split('|');
                        if (exchangeRates[0] == "Date")
                        {
                            // Обход курсов валют по строке - дате
                            for (int ic = 1; ic < exchangeRates.Length; ic++)
                            {
                                currencies[ic - 1] = exchangeRates[ic];
                            }
                        }

                        else
                        {
                            string dateStr = exchangeRates[0];

                            if (dateStr != "")
                            {
                                string dayStr = dateStr.Substring(0, 2);
                                string monthStr = dateStr.Substring(3, 2);

                                int day = Int32.Parse(dayStr);
                                int month = Int32.Parse(monthStr);

                                if (month >= firstMonth && month <= lastMonth && day >= firstDay && day <= lastDay)
                                {
                                    //Обход курсов валют по строке - дате
                                    string exchangeRate;
                                    float rate;
                                    DateTime date;
                                    for (int ic = 1; ic < exchangeRates.Length; ic++)
                                    {
                                        exchangeRate = exchangeRates[ic];
                                        rate = float.Parse(exchangeRate, CultureInfo.InvariantCulture.NumberFormat);
                                        date = DateTime.Parse(dateStr);

                                        ExchangeRate exchangeRateObj = new ExchangeRate()
                                        {
                                            Id = ic,
                                            Date = date,
                                            CurrencyCode = currencies[ic - 1],
                                            Rate = rate
                                        };

                                        exchangeRatesObj.Add(exchangeRateObj);
                                    }
                                }
                            }

                        }

                    }

                }
            }

            return exchangeRatesObj;
        }
    }
}
