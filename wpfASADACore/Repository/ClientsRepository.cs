using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfASADACore.Models;
using wpfASADACore.Services;

namespace wpfASADACore.Repository
{
    class ClientsRepository
    {
        private ContextDataBase context;
        public string message { get; set; }

        public ClientsRepository()
        {
            context = new ContextDataBase();
            context.Database.EnsureCreatedAsync().Wait();
        }

        public async Task<bool> CreateClient(string name, string DNI, string lastName, string secondSurname, string subscribernum, int IdtypeClient)
        {

            try
            {

                using (var db = new ContextDataBase())
                {

                    

                    clsCliente client1 = new clsCliente(name, lastName, secondSurname, DNI, subscribernum, IdtypeClient);

                    db.clientes.Add(client1);

                    await db.SaveChangesAsync();

                    return true;

                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }

        }

        // Obtener el Usuario por el Nombre
        public List<clsCliente> GetAllClients()
        {
            // u => u  se llama a las creaciones Lambda
            return context.clientes.ToList();
        }
    }
}
