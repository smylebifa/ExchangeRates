using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Model
{
    public class ExchangeRate
    {
        [Key]
        public int Id { get; set; }
        public string CurrencyCode { get; set; }
        public float Rate { get; set; }
        public DateTime Date { get; set; }
    }
}
