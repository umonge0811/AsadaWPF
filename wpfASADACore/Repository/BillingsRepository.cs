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

        public async Task<bool> CreateBilling(DateTime fechaActual, DateTime fechaLecturaAnterior,  double AmountBase,
                        double AmountExc,double AmountTotal,double AmountIva, int LecturaActual, int lecturaAnterior,int IdUser, string Remarks, int idClient)
        {
            using (var db = new ContextDataBase())
            {
                clsBilling nuevaFactura1 = new clsBilling(fechaActual, fechaLecturaAnterior, AmountBase,
                        AmountExc, AmountTotal, AmountIva, LecturaActual, lecturaAnterior, IdUser, Remarks, idClient);
                db.billings.Add(nuevaFactura1);

                await db.SaveChangesAsync();

                return true;

            }
                

            
        }

    }
    
}
