using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2OPure.Models;
using H2OPure.Services;

namespace H2OPure.Repository
{
    public class UsersRepository
    {
        private ContextDataBase context;
        public string message { get; set; }
        

        public UsersRepository()
        {
            context = new ContextDataBase();
            context.Database.EnsureCreatedAsync().Wait();
            // Verificar si el usuario administrador ya existe
            var adminExists = context.usuarios.Any(u => u.UserName == "Admin");

            // Si el usuario administrador no existe, crearlo
            if (!adminExists)
            {
                var adminUser = new clsUser
                {
                    Name = "Admin",
                    UserName = "Admin",
                    DNI = "N/A",
                    Email = "Sin Email",
                    typeUser = 1, // 1 = admin
                    Puesto = "Administrador",
                    isActive = true,
                    IsPasswordChangeRequired = true,
                };

                // Establecer la contraseña del administrador
                adminUser.Password = adminUser.EstablecerContraseña("admin");

                // Agregar el usuario administrador a la base de datos
                context.usuarios.Add(adminUser);
                context.SaveChanges();
            }
        }

        

        // Este metodo es para Obtener informacion para llena el Datagrid de Usuarios
        public ObservableCollection<clsUser> GetAllUser()
        {
            // Proyectar las propiedades deseadas, excluyendo la contraseña
            var projectedUsers = context.usuarios.Select(u => new clsUser
            {
                DNI = u.DNI,
                Name = u.Name,
                UserName = u.UserName,
                Email = u.Email,
                isActive = u.isActive,
                typeUser = u.typeUser,
                Puesto = u.Puesto,
                // ... Otras propiedades que desees incluir
            });

            // Convertir la proyección a una lista observable
            return new ObservableCollection<clsUser>(projectedUsers.ToList());
        }


        public async Task<bool> CreateUser(string name, string username, string dni, string password, string email, int typeUser, bool isActivo,string puesto)
        {

            try
            {

                using (var db = new ContextDataBase())
                {

                    //await db.Database.EnsureCreatedAsync();

                    clsUser usuario1 = new clsUser(name, email, password, username, dni,typeUser, isActivo, puesto); //Estás creando una instancia de la clase clsUser con los datos proporcionados (nombre, correo electrónico, contraseña, nombre de usuario y DNI).

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
        public async Task<bool> modifyUser(string name, string username, string dni,  string email, string userName, string puesto,int typeuser, bool isActive)
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
                        user.Puesto = puesto;
                        user.isActive = isActive;
                        user.typeUser = typeuser;

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

        #region metodo para validar si el usuario ya se encuentra registrado
        public async Task<bool> ValidatedUserRegister(string dni)
        {

            try
            {
                using (var db = new ContextDataBase())
                {
                    var userExists = await db.usuarios.AnyAsync(u => u.DNI == dni);
                    return userExists;
                }
            }
            catch (Exception ex)
            {
                // Manejar errores de conexión u otras excepciones
                Console.WriteLine("Error al validar usuario: " + ex.Message);
                throw; // Reenviar la excepción para un tratamiento superior
            }
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

        public async Task<bool> deleteUser(string? userName)
        {
            bool estado = false;
            try
            {
                using (var db = new ContextDataBase())
                {
                    var user = await db.usuarios.FirstOrDefaultAsync(u => u.UserName == userName);

                    if (user != null)
                    {
                        // Marca el usuario como inactivo en lugar de eliminarlo
                        user.isActive = false;
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

        // Este metodo es para Obtener  todos los Usuarios  
        public ObservableCollection<clsUser> GetAllUsers()
        {
            return new ObservableCollection<clsUser>(context.usuarios.ToList());
        }
        // Este metodo es para Obtener  todos los Usuarios para el reporte 
        public IEnumerable<clsUser> GetAllUsersAsEnumerable()
        {
            return context.usuarios.ToList();
        }

        //Metodo para validar el inicio de sesion 
        public async Task<(bool, int, int, bool, string)> ValidateUserLogin(string username, string password)
        {
            try
            {
                using (var db = new ContextDataBase())
                {
                    var user = await db.usuarios.FirstOrDefaultAsync(u => u.UserName == username && u.isActive);

                    if (user != null && user.VerificarContraseña(password))
                    {
                        // Devuelve verdadero si el usuario es válido, junto con el Id, el tipo de usuario, el estado de IsPasswordChangeRequired y un mensaje vacío
                        return (true, user.Id, user.typeUser, user.IsPasswordChangeRequired, "");
                    }
                    else if (user != null)
                    {
                        // El usuario es inválido porque la contraseña es incorrecta
                        return (false, default(int), default(int), default(bool), "Contraseña o Usuario incorrecta");
                    }
                    else
                    {
                        // El usuario es inválido porque no existe o no está activo
                        return (false, default(int), default(int), default(bool), "Usuario no existe o no está activo");
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            // Devuelve falso si el usuario no es válido o no existe, junto con valores predeterminados para el Id, el tipo de usuario, IsPasswordChangeRequired y un mensaje de error
            return (false, default(int), default(int), default(bool), "Error al validar el usuario");
        }

        // Cambiar la contraseña de un usuario admin al inicio, y tambien se puede cambiar la contraseña de un usuario
        public async Task<bool> ChangeUserPassword(int userId, string newPassword)
        {
            try
            {
                // Buscar al usuario en la base de datos
                var user = context.usuarios.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    // El usuario no existe
                    return false;
                }

                // Cambiar la contraseña del usuario
                user.Password = user.EstablecerContraseña(newPassword);
                user.IsPasswordChangeRequired = false;

                // Guardar los cambios en la base de datos
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                return false;
            }
        }




    }




}
