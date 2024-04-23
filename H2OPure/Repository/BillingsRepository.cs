using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2OPure.Models;
using H2OPure.Services;
using System.Net.Sockets;
using static H2OPure.Repository.ReadingRepository;
using H2OPure.Views;


namespace H2OPure.Repository
{   

    class BillingsRepository
    {
        private ContextDataBase context;

        public BillingsRepository()
        {
            context = new ContextDataBase();
            context.Database.EnsureCreatedAsync().Wait();
        }


        // Método para agregar una factura
        public async Task AddBilling(clsBilling billing)
        {
            context.billings.Add(billing);
            await context.SaveChangesAsync();
        }

        // Método para actualizar la lectura como pagada
        public async Task UpdateReadingAsPaid(clsReading reading)
        {
            var readingToUpdate = context.readings.Find(reading.id);
            if (readingToUpdate != null)
            {
                readingToUpdate.Remarks = "Pagado";
                context.readings.Update(readingToUpdate);
                await context.SaveChangesAsync();
          
            
            }
        }

        public clsReading GetReadingById(int id)
        {
            return context.readings.Find(id);
        }
        public class ClientWithBilling
        {
            public int Id { get; set; }
            public string SubscriberNum { get; set; }
            public string Name { get; set; }
            public string LastName { get; set; }
            public string SecondSurname { get; set; }

            public string DisplayText
            {
                get { return $"{SubscriberNum} - Medidor No : {SubscriberNum} Nombre: {Name} {LastName} {SecondSurname}"; }
            }
        }



        public List<ClientWithBilling> GetClientsWithReadings(string remarks)
        {
            using (var db = new ContextDataBase())
            {
                return db.readings
                    .Where(r => r.Client != null && r.Remarks == remarks)
                    .Select(r => new ClientWithBilling
                    {
                        Id = r.Client.id,
                        SubscriberNum = r.Client.SubscriberNum,
                        Name = r.Client.name,
                        LastName = r.Client.lastName,
                        SecondSurname = r.Client.secondSurname
                    })
                    .Distinct()
                    .ToList();
            }
        }

        public clsTypeClient GetTypeClientByClientId(int clientId)
        {
            using (var db = new ContextDataBase())
            {
                var client = db.clientes.Find(clientId);
                return client != null ? db.typeClients.Find(client.TypeClientId) : null;
            }
        }


        public List<clsReading> GetPendingReadings(int clientiID)
        {
            using (var db = new ContextDataBase())
            {
                return db.readings.Where(r => r.Remarks == "Pendiente de Pago" && r.idClient == clientiID).ToList();
            }
        }

        public class BillingDetails
        {
            public clsBilling Billing { get; set; }
            public clsCliente Client { get; set; }
            public clsReading Reading { get; set; }
        }

        public BillingDetails GetBillingDetails(int billingId)
        {
            var billing = context.billings.Include(b => b.Client).ThenInclude(c => c.TypeClient).FirstOrDefault(b => b.id == billingId);
            var reading = context.readings.FirstOrDefault(r => r.idClient == billing.idClient && r.CurrentReadingDate == billing.BillingDate);

            return new BillingDetails
            {
                Billing = billing,
                Client = billing.Client,
                Reading = reading
            };
        }

        public async Task<List<BillingDetails>> GetAllBillingDetails()
        {
            try
            {
                // Obtener todas las facturas de la base de datos
                var billings = await context.billings.Include(b => b.Client).ThenInclude(c => c.TypeClient).ToListAsync();

                // Crear una lista para almacenar los detalles de facturación
                var billingDetailsList = new List<BillingDetails>();

                // Para cada factura, obtener los detalles de facturación y agregarlos a la lista
                foreach (var billing in billings)
                {
                    var reading = context.readings.FirstOrDefault(r => r.idClient == billing.idClient && r.CurrentReadingDate == billing.BillingDate);
                    var billingDetails = new BillingDetails
                    {
                        Billing = billing,
                        Client = billing.Client,
                        Reading = reading
                    };
                    billingDetailsList.Add(billingDetails);
                }

                return billingDetailsList;
            }
            catch (Exception ex)
            {
                // Imprimir el error y lanzar la excepción
                Console.WriteLine("Error al obtener todos los detalles de facturación: " + ex.Message);
                throw;
            }
        }





    }

}
