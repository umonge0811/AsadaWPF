using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H2OPure.Models;
using H2OPure.Repository;

namespace H2OPure.Services
{
    public class AutenticacionService
    {
        private UsersRepository userRepo;

        public AutenticacionService()
        {
            userRepo = new UsersRepository();
        }

        public bool AutenticarUsuario(string nombreUsuario, string contraseña)
        {
            clsUser? usuario = userRepo.GetUserByUserName(nombreUsuario);

            if (usuario != null)
            {
                return usuario.VerificarContraseña(contraseña);
            }

            return false;
        }
    }
}
