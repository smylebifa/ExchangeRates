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

        private List<ExchangeRate> exchangeRatesObj;

        private int firstYearInt, lastYearInt, firstMonth, lastMonth, firstDay, lastDay;

        public ExchangeRatesService(ExchangeRatesDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<ExchangeRate>> GetExchangeRatesAsync()
        {
            return await _dbContext.ExchangeRates.ToListAsync();
        }

        public List<ExchangeRate> parseExchangeRatesByPeriod(DateTime first_date, DateTime last_date)
        {
            parseAndSaveDates(first_date, last_date);

            exchangeRatesObj = new List<ExchangeRate>();

            WebClient web = new WebClient();
            string downloadedString;
            string[] currencies = new string[35];

            int countOfYears = lastYearInt - firstYearInt;
            for (int i = 0; i <= countOfYears; i++)
            {
                string yearToParseStr = (firstYearInt + i).ToString();
                downloadedString = web.DownloadString(WebPathOfExchangeRatesByPeriod + yearToParseStr);

                string[] stringsWithExchangeRates = downloadedString.Split('\n');

                currencies = parseParams(stringsWithExchangeRates[0]);

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
                        for (int j = 1; j < stringsWithExchangeRates.Length; j++)
                        {
                            string stringWithExchangeRates = stringsWithExchangeRates[j];

                            string[] exchangeRates = stringWithExchangeRates.Split('|');
                            string dateStr = exchangeRates[0];

                            if (dateActionsForFirstDate(dateStr, firstMonth, firstDay)) 
                                createExchangeRatesObj(exchangeRates, dateStr, currencies);

                        }
                    }

                    // 2 вариант
                    else if (i == countOfYears)
                    {
                        for (int j = 1; j < stringsWithExchangeRates.Length; j++)
                        {
                            string stringWithExchangeRates = stringsWithExchangeRates[j];

                            string[] exchangeRates = stringWithExchangeRates.Split('|');
                            string dateStr = exchangeRates[0];

                            if (dateActionsForLastDate(dateStr, lastMonth, lastDay)) 
                                createExchangeRatesObj(exchangeRates, dateStr, currencies);
                        }
                    }

                    // 3 вариант
                    else
                    {
                        for (int j = 1; j < stringsWithExchangeRates.Length; j++)
                        {
                            string stringWithExchangeRates = stringsWithExchangeRates[j];

                            string[] exchangeRates = stringWithExchangeRates.Split('|');
                            string dateStr = exchangeRates[0];

                            if (dateStr != "") 
                                createExchangeRatesObj(exchangeRates, dateStr, currencies);
                        }
                    }
                }

                // Если год начала и конца совпадают, то сравниваем текущую дату с первой и последней датой 
                // (нужно дойти построчно до начальной и конечной даты периода)
                else
                {
                    for (int j = 1; j < stringsWithExchangeRates.Length; j++)
                    {
                        string stringWithExchangeRates = stringsWithExchangeRates[j];

                        string[] exchangeRates = stringWithExchangeRates.Split('|');
                        string dateStr = exchangeRates[0];

                        if (dateActionsForTwoDates(dateStr, firstMonth, firstDay, lastMonth, lastDay))
                            createExchangeRatesObj(exchangeRates, dateStr, currencies);
                    }
                }
            }

            return exchangeRatesObj;
        }

        private void parseAndSaveDates(DateTime first_date, DateTime last_date)
        {
            string firstDateStr = first_date.ToString("yyyy-MM-dd");
            string lastDateStr = last_date.ToString("yyyy-MM-dd");

            string firstYearStr = firstDateStr.Substring(0, 4);
            string lastYearStr = lastDateStr.Substring(0, 4);

            string firstMonthStr = firstDateStr.Substring(5, 2);
            string lastMonthStr = lastDateStr.Substring(5, 2);

            string firstDayStr = firstDateStr.Substring(8, 2);
            string lastDayStr = lastDateStr.Substring(8, 2);

            firstYearInt = Int32.Parse(firstYearStr);
            lastYearInt = Int32.Parse(lastYearStr);

            firstMonth = Int32.Parse(firstMonthStr);
            lastMonth = Int32.Parse(lastMonthStr);

            firstDay = Int32.Parse(firstDayStr);
            lastDay = Int32.Parse(lastDayStr);
        }

        private string[] parseParams(string stringWithExchangeRates)
        {
            string[] exchangeRates = stringWithExchangeRates.Split('|');
            string[] currencies = new string[35];
            if (exchangeRates[0] == "Date")
            {
                // Обход курсов валют по строке - дате
                for (int ic = 1; ic < exchangeRates.Length; ic++)
                {
                    currencies[ic - 1] = exchangeRates[ic];
                }
            }

            return currencies;
        }


        private bool dateActionsForFirstDate(string dateStr, int firstMonth, int firstDay)
        {
            if (dateStr != "")
            {
                string dayStr = dateStr.Substring(0, 2);
                string monthStr = dateStr.Substring(3, 2);

                int day = Int32.Parse(dayStr);
                int month = Int32.Parse(monthStr);

                if (month >= firstMonth && day >= firstDay) return true;
            }

            return false;
        }
        
        private bool dateActionsForLastDate(string dateStr, int lastMonth, int lastDay)
        {
            if (dateStr != "")
            {
                string dayStr = dateStr.Substring(0, 2);
                string monthStr = dateStr.Substring(3, 2);

                int day = Int32.Parse(dayStr);
                int month = Int32.Parse(monthStr);

                if (month <= lastMonth && day <= lastDay) return true;
            }

            return false;
        }

        private bool dateActionsForTwoDates(string dateStr, int firstMonth, int firstDay, int lastMonth, int lastDay)
        {
            if (dateStr != "")
            {
                string dayStr = dateStr.Substring(0, 2);
                string monthStr = dateStr.Substring(3, 2);

                int day = Int32.Parse(dayStr);
                int month = Int32.Parse(monthStr);

                if (month >= firstMonth && month <= lastMonth && day >= firstDay && day <= lastDay) return true;
            }

            return false;
        }


        //Обход курсов валют и возврат по строке - дате
        private void createExchangeRatesObj(string[] exchangeRates, string dateStr, string[] currencies)
        {
            string exchangeRate;
            float rate;
            DateTime date;
            for (int i = 1; i < exchangeRates.Length; i++)
            {
                exchangeRate = exchangeRates[i];
                rate = float.Parse(exchangeRate, CultureInfo.InvariantCulture.NumberFormat);
                date = DateTime.Parse(dateStr);

                ExchangeRate exchangeRateObj = new ExchangeRate()
                {
                    Id = i,
                    Date = date,
                    CurrencyCode = currencies[i - 1],
                    Rate = rate
                };

                exchangeRatesObj.Add(exchangeRateObj);
            }
        }
    }
}
