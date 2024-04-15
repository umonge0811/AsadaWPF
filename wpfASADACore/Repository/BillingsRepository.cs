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
using static wpfASADACore.Repository.ReadingRepository;
using wpfASADACore.Views;


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

        public class ClientWithBilling
        {
            public int Id { get; set; }
            public string SubscriberNum { get; set; }
            public string Name { get; set; }
            public string LastName { get; set; }
            public string SecondSurname { get; set; }

            public string DisplayText
            {
                get { return $"{SubscriberNum} - Abonado No : {SubscriberNum} Nombre: {Name} {LastName} {SecondSurname}"; }
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




    }
    
}
