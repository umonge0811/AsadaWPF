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
    public class UsersRepository
    {
        private ContextDataBase context;

        public UsersRepository()
        {
            context = new ContextDataBase();
            context.Database.EnsureCreatedAsync().Wait();
        }


        //Metodo para Crear Usuarios
        public void CreateUsers(clsUser user)
        {
            try
            {
                context.usuarios.Add(user);
                context.SaveChanges();

            }
            catch (DbUpdateException ex)
            {
                //Captura la exepcion y muestra los detalles
                Exception? innerException = ex.InnerException;

                while (innerException != null)
                {
                    Console.WriteLine($"innerException.{innerException.Message}");
                    innerException = innerException.InnerException;
                }

            }

        }


        // Obtener el Usuario por el Nombre
        public clsUser? GetUserByUserName(string nameUser)
        {
            // u => u  se llama a las creaciones Lambda
            return context.usuarios.FirstOrDefault(u => u.UserName == nameUser);
        }






    }




}
