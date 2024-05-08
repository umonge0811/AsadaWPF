using MahApps.Metro.IconPacks;
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
        public string? SubscriberNum { get; set; }
        public string? Direction { get; set; }
        //public int IdtypeClient { get; set; }

        public int TypeClientId { get; set; }  //  La propiedad TypeClientId es la clave foránea que se relaciona con la tabla de tipos de cliente.
        public clsTypeClient? TypeClient { get; set; }  // La propiedad TypeClient es una propiedad de navegación que  permite acceder al objeto de tipo de cliente relacionado.

        public clsCliente()
        {
        }

        public clsCliente(string name, string lastName, string secondSurname, string dNI, string subscribernum, int typeClientId, string direction)
        {
            this.name = name;
            this.lastName = lastName;
            this.secondSurname = secondSurname;
            this.DNI = dNI;
            this.SubscriberNum = subscribernum;
            this.TypeClientId = typeClientId;
            this.Direction = direction;
        }
           


       

    }
}
