using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace wpfASADACore.Utilities
{
    public class clsUtilities
    {

        public static bool EsCorreoValido(string email)
        {
            // Patrón de expresión regular para un correo electrónico
            const string regexPattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

            // Valida el correo electrónico con la expresión regular
            return Regex.IsMatch(email, regexPattern);
        }


    }
}
