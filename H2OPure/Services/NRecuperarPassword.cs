using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2OPure.Services
{
    class NRecuperarPassword
    {
        public string recoverPassword(string userRequesting)
        {
            return new Recuperar().recoverPassword(userRequesting);
        }
    }
}
