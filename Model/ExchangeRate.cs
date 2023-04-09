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
        public double Rate { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
    }
}
