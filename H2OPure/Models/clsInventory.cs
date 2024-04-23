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
        public string Department { get; set; }
        public string Remarks { get; set; } // Agregado

        public int userId { get; set; } // Cambiado de employeeId a userId

        // Relación con la tabla de usuarios
        public clsUser User { get; set; } // Cambiado de Employee a User

        //contructor vacio
        public clsInventory()
        {
        }

        //constructor lleno
        public clsInventory(int id, string materialName, int quantity, DateTime entryDate, DateTime exitDate,string department,string remarks, int userId) // Cambiado de employeeId a userId
        {
            this.id = id;
            this.materialName = materialName;
            this.quantity = quantity;
            this.entryDate = entryDate;
            this.exitDate = exitDate;
            this.Department = department;
            this.Remarks = remarks; // Agregado
            this.userId = userId; // Cambiado de employeeId a userId
        }
    }
}
