using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2OPure.Models;
using H2OPure.Services;
using H2OPure.Views;
using System.Collections.ObjectModel;

namespace H2OPure.Repository
{
    public class ReadingRepository
    {
        private ContextDataBase context;

        public ReadingRepository()
        {
            context = new ContextDataBase();
            context.Database.EnsureCreatedAsync().Wait();
        }

        public async Task<bool> CreateReading(DateTime fechaLecturaAnterior, DateTime dateCurrentReading, int totalConsumption, int lecturaAnterior, int LecturaActual, string Remarks, bool LecturaActiva, int IdUser, int idClient, int typeclientId, bool IsChecked)
        {
            using (var db = new ContextDataBase())
            {
                clsReading LecturaInicial = new clsReading(fechaLecturaAnterior, dateCurrentReading, totalConsumption, lecturaAnterior, LecturaActual, Remarks, LecturaActiva, IdUser, idClient, typeclientId, IsChecked);
                db.readings.Add(LecturaInicial);

                await db.SaveChangesAsync();

                return true;

            }
        }

        public async Task UpdateReading(clsReading reading)
        {
            using (var db = new ContextDataBase())
            {
                db.readings.Update(reading);
                await db.SaveChangesAsync();
            }
        }

        public clsReading GetReadingById(int id)
        {
            return context.readings.Find(id);
        }


        public class ClientWithReading
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


        public List<ClientWithReading> GetClientsWithReadings()
        {
            using (var db = new ContextDataBase())
            {
                return db.readings
                    .Where(r => r.Client != null)
                    .Select(r => new ClientWithReading
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

        public clsReading? GetLastReading(int clientId)
        {
            using (var db = new ContextDataBase())
            {
                return db.readings
                    .Where(r => r.idClient == clientId)
                    .OrderByDescending(r => r.id)
                    .FirstOrDefault();
            }
        }

        // En ReadingRepository
        public List<clsReading> GetReadingsByClient(int clientId)
        {
            using (var db = new ContextDataBase())
            {
                return db.readings
                    .Where(r => r.idClient == clientId && (r.CurrentRead != null || r.ReadActiva))
                    .ToList();
            }
        }
    }
}
