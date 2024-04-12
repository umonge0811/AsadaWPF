using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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



        // Este metodo es para Obtener 
        public ObservableCollection<clsUser> GetAllUser()
        {
            // Proyectar las propiedades deseadas, excluyendo la contraseña
            var projectedUsers = context.usuarios.Select(u => new clsUser
            {
                DNI = u.DNI,
                Name = u.Name,
                UserName = u.UserName,
                Email = u.Email,
                // ... Otras propiedades que desees incluir
            });

            // Convertir la proyección a una lista observable
            return new ObservableCollection<clsUser>(projectedUsers.ToList());
        }


        public async Task<bool> CreateUser(string name, string username, string dni, string password, string email)
        {

            try
            {

                using (var db = new ContextDataBase())
                {

                    //await db.Database.EnsureCreatedAsync();

                    clsUser usuario1 = new clsUser(name, email, password, username, dni); //Estás creando una instancia de la clase clsUser con los datos proporcionados (nombre, correo electrónico, contraseña, nombre de usuario y DNI).

                    db.usuarios.Add(usuario1); //Luego, se agrega este usuario al conjunto de datos (DbSet) de usuarios en el contexto de base de datos (ContextDataBase).

                    await db.SaveChangesAsync();//se usa SaveChangesAsync para guardar los cambios en la base de datos.

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


        // Obtener el Usuario por el Id
        public clsUser? GetUserByID(int id)
        {
            // u => u  se llama a las creaciones Lambda
            return context.usuarios.FirstOrDefault(u => u.Id == id);
        }


        #region Modificar Usuarios
        public async Task<bool> modifyUser(string name, string username, string dni,  string email, string userName)
        {

            bool estado = false;
            try
            {

                using (var db = new ContextDataBase())
                {

                    var user = await db.usuarios.FirstOrDefaultAsync(u => u.UserName == userName);

                    if (user != null)
                    {
                        user.Name = name;
                        user.UserName = username;
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
        #endregion


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

        public async Task<bool> deleteUser( string? userName)
        {

            bool estado = false;
            try
            {

                using (var db = new ContextDataBase())
                {

                    var user = await db.usuarios.FirstOrDefaultAsync(u => u.UserName == userName);

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

        // Este metodo es para Obtener 
        public ObservableCollection<clsUser> GetAllUsers()
        {
            return new ObservableCollection<clsUser>(context.usuarios.ToList());
        }


    }




}
