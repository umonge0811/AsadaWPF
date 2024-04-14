using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace wpfASADACore.Models
{
    public class clsReading
    {
        [Key]
        public int id { get; set; }
        public DateTime DateLastReading { get; set; } //fecha de la ultima lectura
        public DateTime CurrentReadingDate { get; set; } //fecha de la lectura actual
        public int TotalConsumption { get; set; } // Consumo total en m3
        public int lastRead { get; set; } //lectura anterior puede ser  0 o brindado por el usuario
        public int CurrentRead { get; set; } //lectura actual del medidor
        public string? Remarks { get; set; } //Observaciones del recibo
        public bool ReadActiva { get; set; } //0 si no esta activa la lectura 1 si esta activa (falta de Pago)
        public int UserId { get; set; } //id del usuario que realizo el ingreso del de la primera lectura
        public int idClient { get; set; }  //  La propiedad ClientId es la clave foránea que se relaciona con la tabla de clientes.
        public int TypeClientId { get; set; }  //  La propiedad TypeClientId es la clave foránea que se relaciona con la tabla de tipos de cliente.

        public clsCliente? Client { get; set; }  // La propiedad Client es una propiedad de navegación que  permite acceder al objeto de cliente relacionado.
        public clsTypeClient? TypeClient { get; set; }  // La propiedad TypeClient es una propiedad de navegación que  permite acceder al objeto de tipo de cliente relacionado.

        public clsReading()
        {
        }

        public clsReading(DateTime dateLastReading, DateTime dateCurrentReading, int totalConsumption, int lastRead, int currentRead, string? remarks, bool readActiva,int IdUser, int idClient, int typeclientId)
        {           
            DateLastReading = dateLastReading;
            CurrentReadingDate = dateCurrentReading;
            TotalConsumption = totalConsumption;
            this.lastRead = lastRead;
            CurrentRead = currentRead;
            Remarks = remarks;
            ReadActiva = readActiva;
            UserId = IdUser;
            this.idClient = idClient;
            TypeClientId = typeclientId;    
        }
    }
}
