using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2OPure.Models
{
    public class clsEmployee
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string position { get; set; }
        public clsUser User { get; set; }
        public List<clsInventory> Inventories { get; set; }

        public clsEmployee()
        {
        }

        public clsEmployee(string name, string position)
        {
            this.name = name;
            this.position = position;
        }
    }



  

    


}
