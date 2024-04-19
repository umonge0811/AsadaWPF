using System.ComponentModel.DataAnnotations;

namespace H2OPure.Models
{
    public class clsBilling
    {
        [Key]
        public int id { get; set; } //id del recibo
        public DateTime BillingDate { get; set; } //fecha del recibo X
        public double?  AmountBase { get; set; } //monto base cobrado lo cual es diferente para cada cliente se cobra por cada unidad de consumo M³
        public double?  AmountExc { get; set; } //monto  del excedente es el costo de cada unidad de consumo M³
        public double?  AmountTotal  { get; set; } //monto total del recibo con IVA
        public double?  AmountIva { get; set; } //monto del iva del recibo
        public double? AmountRec { get; set; } //monto por reconexion del recibo
        public int      UserId { get; set; } //id del usuario que realizo el cobro del recibo
        public string?  Remarks { get; set; } //Observaciones del recibo     
        public string? InvoiceNum { get; set; } //Numero de Factura

        public int     idClient { get; set; }  //  La propiedad ClientId es la clave foránea que se relaciona con la tabla de clientes.
        public clsCliente? Client { get; set; }  // La propiedad Client es una propiedad de navegación que  permite acceder al objeto de cliente relacionado.

        public clsBilling()
        {
        }

        public clsBilling(DateTime billingDate,  double? amountBase, double? amountExc, double? amountTotal, double?
            amountIva, double amountRec, int userId,string? remarks, string invoiceNum, int IdClient)
        {
            BillingDate = billingDate;
            AmountBase = amountBase;
            AmountExc = amountExc;
            AmountTotal = amountTotal;
            AmountIva = amountIva;
            AmountRec = amountRec;
            UserId = userId;
            Remarks = remarks;
            InvoiceNum = invoiceNum;
            this.idClient = IdClient;
           
        }
    }
}
