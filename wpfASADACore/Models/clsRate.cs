﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfASADACore.Models
{
    public class clsRate
    {
        [Key]
        public int id { get; set; }
        public string? name { get; set; }
        public double rate { get; set; }
        public double rateExc { get; set; }
        public string? description { get; set; }
        public int idTypeClient { get; set; }
    }


}
