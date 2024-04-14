using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfASADACore.Models;
using wpfASADACore.Services;
using System.Net.Sockets;


namespace wpfASADACore.Repository
{   

    class BillingsRepository
    {
        private ContextDataBase context;

        public BillingsRepository()
        {
            context = new ContextDataBase();
            context.Database.EnsureCreatedAsync().Wait();
        }

        #region Metodo que se utiliza para enviar la factura a la base de datos
        //public async Task<bool> CreateBilling(DateTime fechaActual,   double AmountBase, double AmountExc,double AmountTotal,double AmountIva, int IdUser, string Remarks, int idClient)
        //{
        //    using (var db = new ContextDataBase())
        //    {
        //        clsBilling nuevaFactura1 = new clsBilling(fechaActual,  AmountBase, AmountExc, AmountTotal, AmountIva, IdUser, Remarks, idClient);
        //        db.billings.Add(nuevaFactura1);

        //        await db.SaveChangesAsync();

        //        return true;

        //    }              


        //}
        #endregion

        public List<(int,string, string)> GetClientsForBilling()
        {
            using (var context = new ContextDataBase())
            {
                // se obtiene los clientes que tienen al menos un registro en la tabla de facturación
                var clients = context.billings
                    .Select(b => b.Client)
                    .Distinct()
                    .ToList();

                // se crea una lista de tuplas con el ID y el nombre completo del cliente
                var clientsForBilling = clients.Select(c => (c.id,c.SubscriberNum, $"{c.name} {c.lastName} {c.secondSurname}")).ToList();

                return clientsForBilling;

                /*Aquí está lo que hace este método:

                Crea una nueva instancia de ContextDataBase para acceder a la base de datos.
                Selecciona los clientes que tienen al menos un registro en la tabla de facturación (billings). Usamos Select(b => b.Client)
                para obtener los objetos clsCliente relacionados con cada factura, y Distinct() para asegurarnos de que solo obtenemos cada cliente una vez.
                Crea una lista de tuplas con el ID y el nombre completo de cada cliente ((c.id, $"{c.name} {c.lastName} {c.secondSurname}")).
                Devuelve la lista de tuplas.*/
            }
        }
    }
    
}
