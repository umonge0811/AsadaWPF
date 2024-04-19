using System.ComponentModel.DataAnnotations;
using System.Windows;


namespace H2OPure.Models
{
    public class clsInventory
    {

        [Key]
        public int id { get; set; }
        public string materialName { get; set; }
        public int quantity { get; set; }
        public DateTime entryDate { get; set; }
        public DateTime exitDate { get; set; }
        public int employeeId { get; set; }

        // Relación con la tabla de empleados
        public clsEmployee Employee { get; set; }



        //contructor vacio
        public clsInventory()
        {
        }

        //constructor lleno
        public clsInventory(int id, string materialName, int quantity, DateTime entryDate, DateTime exitDate, int employeeId)
        {
            this.id = id;
            this.materialName = materialName;
            this.quantity = quantity;
            this.entryDate = entryDate;
            this.exitDate = exitDate;
            this.employeeId = employeeId;
        }
    }





   
}
