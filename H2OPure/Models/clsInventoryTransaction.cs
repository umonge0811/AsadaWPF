using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2OPure.Models
{
    public class clsInventoryTransaction
    {
        [Key]
        public int Id { get; set; }
        public string MaterialName { get; set; }
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Department { get; set; }
        public string Remarks { get; set; }
        public string Action { get; set; } // "Entrada" o "Salida"
        public int UserId { get; set; }
        public clsUser User { get; set; }


        public clsInventoryTransaction()
        {
        }

        public clsInventoryTransaction(int id, string materialName, int quantity, DateTime transactionDate, string department, string remarks, string action, int userId)
        {
            Id = id;
            MaterialName = materialName;
            Quantity = quantity;
            TransactionDate = transactionDate;
            Department = department;
            Remarks = remarks;
            Action = action;
            UserId = userId;
        }
    }
}

