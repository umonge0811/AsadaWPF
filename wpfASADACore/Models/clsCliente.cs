using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfASADACore.Models
{
    public class clsCliente
    {
        [Key]
        public int id { get; set; }
        public string? DNI { get; set; }
        public string? name { get; set; }
        public string? lastName { get; set; }
        public string? secondSurname { get; set; }
        public string? AccountNumber { get; set; }
        public int IdtypeClient { get; set; }

    }
}
