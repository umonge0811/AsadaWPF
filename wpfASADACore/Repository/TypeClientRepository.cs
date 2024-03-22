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
 public   class TypeClientRepository
    {
        private ContextDataBase context;
        public string? message { get; set; } = null;

        public TypeClientRepository()
        {
            context = new ContextDataBase();
            context.Database.EnsureCreatedAsync().Wait();
        }

        public async Task<bool> CreateTypeClient(string name, string descripcion, double rate, double rateExc)
        {
            try
            {
                using (var db = new ContextDataBase())
                {
                    // Verificar si el tipo de cliente ya existe en la base de datos
                    bool typeClientExists = await db.typeClients.AnyAsync(tc => tc.name == name);

                    if (typeClientExists)
                    {
                        // Tipo de cliente ya existe, retornar falso
                        message = "El tipo de cliente ya existe.";
                        return false;
                    }

                    // Si el tipo de cliente no existe, agregarlo a la base de datos
                    clsTypeClient typeClient = new clsTypeClient(name, descripcion, rate, rateExc);
                    db.typeClients.Add(typeClient);
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
        public clsTypeClient? GetUserByUserName(string name)
        {
            // u => u  se llama a las creaciones Lambda
            return context.typeClients.FirstOrDefault(u => u.name.Equals(name));
        }

        // Obtener el Usuario por el Nombre
        public List<clsTypeClient> GetAllTypeCliente()
        {
            // u => u  se llama a las creaciones Lambda
            return context.typeClients.ToList();
        }



    }
}
