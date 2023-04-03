using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Model
{
    public class Currency
    {
        [Key]
        public string Code { get; set; }
        public string Country { get; set; }
    }
}
