using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Model
{
    public class ExchangeRate
    {
        //private string Currency;
        public int Id { get; set; }
        public string CurrencyUSD { get; set; }
        public float Rate { get; set; }
        public DateTime Date { get; set; }

    }
}
