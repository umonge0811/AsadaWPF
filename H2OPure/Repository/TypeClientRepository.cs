using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2OPure.Models;
using H2OPure.Services;

namespace H2OPure.Repository
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
            
            return context.typeClients.FirstOrDefault(u => u.name.Equals(name));
        }

        
        public List<clsTypeClient> GetAllTypeCliente()
        {
            
            return context.typeClients.ToList();
        }

        //Metodo para modificar un tipo de cliente llamado 
        public  async Task<bool> UpdateTypeClient(string newNameType, string newDescription, int? newRate, int? ExtRate, int idTypeClient)
        {
            bool estado = false;
            try
            {
                using (var db = new ContextDataBase())
                {
                    var TypeClient = await db.typeClients.FirstOrDefaultAsync(u => u.id == idTypeClient);
                    if (TypeClient != null)
                    {
                        TypeClient.name = newNameType;
                        TypeClient.description = newDescription;
                        TypeClient.rate = newRate ;
                        TypeClient.rateExc = ExtRate;

                        await db.SaveChangesAsync();
                        estado = true;

                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                estado = false;
            }
            return estado;
        }

        public async Task<bool> DeleteTypeClient(int idTypeClient)
        {

            bool estado = false;
            try
            {

                using (var db = new ContextDataBase())
                {

                    var TypeClient = await db.typeClients.FirstOrDefaultAsync(u => u.id == idTypeClient);

                    if (TypeClient != null)
                    {

                        db.typeClients.Remove(TypeClient);
                        await db.SaveChangesAsync();

                        estado = true;
                    }

                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
                //MessageBox.Show(ex.Message);
                estado = false;
            }

            return estado;

        }


    }
}
