using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace wpfASADACore.Models
{
    public class clsUser
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string UserName { get; set; }
        public string DNI { get; set; }

        //contructor lleno
        public clsUser(string? name, string? email, string? password, string userName, string DNI)
        {
            Name = name;
            Email = email;
            Password = EstablecerContraseña(password);
            UserName = userName;
            this.DNI = DNI;
        }

        //constructor vacio
        public clsUser()
        {
        }


        public string EstablecerContraseña(string? contraseña)
        {
            return BCrypt.Net.BCrypt.HashPassword(contraseña);
        }



        public bool VerificarContraseña(string contraseña)
        {
            bool verificar = false;
               try { 
            verificar = BCrypt.Net.BCrypt.Verify(contraseña, this.Password); 
            
            }catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }

            return verificar;
        }
    }
}
