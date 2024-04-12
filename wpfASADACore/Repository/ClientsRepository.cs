using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        // Este metodo es para Obtener 
        public ObservableCollection<clsCliente> GetAllClients()
        {
            return new ObservableCollection<clsCliente>(context.clientes.ToList());
        }   
        
        //este es el metodo que se creo para hacer la busqueda en la DB, retorna  un objeto de tipo clase (clsUser)
        public async Task<clsCliente?> FindClientBySubscriberNum(string SubscriberNum)
        {

            //puede que retorne nula si no se encuentra nada
            clsCliente? client = null;
            try
            {

                using (var db = new ContextDataBase())
                {

                    client = await db.clientes.FirstOrDefaultAsync(u => u.SubscriberNum.Equals(SubscriberNum));

                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
                client = null;

            }
            return client;

        }

        //Metodo para modificar un cliente llamado UpdateClient
        public async Task<bool> UpdateClient(int? id, string name, string DNI, string lastName, string secondSurname, string subscribernum, int IdtypeClient)
        {
            bool estado = false;
            try
            {
                using (var db = new ContextDataBase())
                {
                    var client = await db.clientes.FirstOrDefaultAsync(u => u.id == id);
                    if (client != null)
                    {
                        client.name = name;
                        client.DNI = DNI;
                        client.lastName = lastName;
                        client.secondSurname = secondSurname;
                        client.SubscriberNum = subscribernum;
                        client.TypeClientId = IdtypeClient;

                        await db.SaveChangesAsync();
                        estado = true;
                    
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                estado= false;
            }
            return estado;
        }

        public async Task<bool> deleteClient(int? idClient)
        {

            bool estado = false;
            try
            {

                using (var db = new ContextDataBase())
                {

                    var client = await db.clientes.FirstOrDefaultAsync(u => u.id == idClient);

                    if (client != null)
                    {

                        db.clientes.Remove(client);
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
