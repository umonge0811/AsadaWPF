using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace wpfASADACore.Models
{
    public class clsBilling
    {
        [Key]
        public int id { get; set; } //id del recibo
        public DateTime BillingDate { get; set; } //fecha del recibo
        public DateTime DateLastReading { get; set; } //fecha de la ultima lectura
        public double? AmountBase { get; set; } //monto base cobrado lo cual es diferente para cada cliente
        public double? AmountExc { get; set; } //monto  del excedente es el costo de cada unidad de consumo M³
        public double? AmountTotal  { get; set; } //monto total del recibo SIN IVA
        public double? AmountIva { get; set; } //monto TOTAL con iva del recibo
        public int     CurrentRead { get; set; } //lectura actual del medidor
        public int     lastRead { get; set; } //lectura anterior puede ser  0 o brindado por el usuario
        public int     UserId { get; set; } //id del usuario que realizo el cobro del recibo
        public string? Remarks { get; set; } //Observaciones del recibo
       

        public int     idClient { get; set; }  //  La propiedad ClientId es la clave foránea que se relaciona con la tabla de clientes.
        public clsCliente? Client { get; set; }  // La propiedad Client es una propiedad de navegación que  permite acceder al objeto de cliente relacionado.

        public clsBilling()
        {
        }

        public clsBilling(DateTime billingDate, DateTime dateLastReading, double? amountBase, double? amountExc, double? amountTotal, double? 
            amountIva, int currentRead, int lastRead, int userId, string? remarks, int IdClient)
        {
            BillingDate = billingDate;
            DateLastReading = dateLastReading;
            AmountBase = amountBase;
            AmountExc = amountExc;
            AmountTotal = amountTotal;
            AmountIva = amountIva;
            CurrentRead = currentRead;
            this.lastRead = lastRead;
            UserId = userId;
            Remarks = remarks;
            this.idClient = IdClient;
            
        }
    }
}
