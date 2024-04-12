using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfASADACore.Models
{
    public class clsTypeClient
    {
        [Key]
        public int id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public double? rate { get; set; }
        public double? rateExc { get; set; }

        public clsTypeClient()
        {
        }

        public clsTypeClient(int id, string? name, string? description, double? rate, double? rateExc)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.rate = rate;
            this.rateExc = rateExc;
        }

        public clsTypeClient(string? name, string? description, double? rate, double? rateExc)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.rate = rate;
            this.rateExc = rateExc;
        }
    }
}
