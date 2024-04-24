using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace H2OPure.Services
{
    class DCorreoSoporte : Recuperar
    {
        public DCorreoSoporte()
        {
            remitenteCorreo = "umongegds@gmail.com";
            passwordRemitente = "coahckexdtlkmnbs";
            host = "smtp.gmail.com";
            puerto = 587;
            ssl = true;
            initializeSmtpClient();

        }
    }
}
