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
        public string message { get; set; }

        public UsersRepository()
        {
            context = new ContextDataBase();
            context.Database.EnsureCreatedAsync().Wait();
        }



        public async Task<bool> createUser(string name, string username, string dni, string password, string email)
        {

            try
            {

                using (var db = new ContextDataBase())
                {

                    //await db.Database.EnsureCreatedAsync();

                    clsUser usuario1 = new clsUser(name, email, password, username, dni);

                    db.usuarios.Add(usuario1);

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
        public clsUser? GetUserByUserName(string nameUser)
        {
            // u => u  se llama a las creaciones Lambda
            return context.usuarios.FirstOrDefault(u => u.UserName.Equals(nameUser));
        }


        // Obtener el Usuario por el Nombre
        public clsUser? GetUserByID(int id)
        {
            // u => u  se llama a las creaciones Lambda
            return context.usuarios.FirstOrDefault(u => u.Id == id);
        }

        public async Task<bool> modifyUser(string name, string username, string dni, string password, string email, int? idUser)
        {

            bool estado = false;
            try
            {

                using (var db = new ContextDataBase())
                {

                    var user = await db.usuarios.FirstOrDefaultAsync(u => u.Id == idUser);

                    if (user != null)
                    {
                        user.Name = name;
                        user.UserName = username;
                        user.Password = user.EstablecerContraseña(password);
                        user.Email = email;
                        user.DNI = dni;

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


        //este es el metodo que se creo para hacer la busqueda en la DB, retorna  un objeto de tipo clase (clsUser)
        public async Task<clsUser?> FindClientByDNI(string dni)
        {

            //puede que retorne nula si no se encuentra nada
            clsUser? user = null;
            try
            {

                using (var db = new ContextDataBase())
                {

                    user = await db.usuarios.FirstOrDefaultAsync(u => u.DNI.Equals(dni));

                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
                user = null;

            }
            return user;

        }

        public async Task<bool> deleteUser( int? idUser)
        {

            bool estado = false;
            try
            {

                using (var db = new ContextDataBase())
                {

                    var user = await db.usuarios.FirstOrDefaultAsync(u => u.Id == idUser);

                    if (user != null)
                    {
                      
                        db.usuarios.Remove(user);
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
