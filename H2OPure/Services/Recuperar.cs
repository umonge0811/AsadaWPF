using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace H2OPure.Services
{
    public class Recuperar
    {
        private SmtpClient stmpClient;

        protected string remitenteCorreo { get; set; }

        protected string passwordRemitente { get; set; }

        protected string host { get; set; }

        protected int puerto { get; set; }

        protected bool ssl { get; set; }

        public Recuperar()
        {
            initializeSmtpClient();
        }
        protected void initializeSmtpClient()
        {
            remitenteCorreo = "umongegds@gmail.com";
            passwordRemitente = "coahckexdtlkmnbs";
            host = "smtp.gmail.com";
            puerto = 587;
            ssl = true;
            stmpClient = new SmtpClient();
            //stmpClient.Credentials = new System.Net.NetworkCredential(remitenteCorreo, passwordRemitente);
            //stmpClient.Host = host;
            //stmpClient.Port = puerto;
            //stmpClient.EnableSsl = ssl;
        }

        public void enviarCorreo(string subject, string body, List<string> destinarioCorreo)
        {
            var mailMessage = new MailMessage();
            try
            {
                mailMessage.From = new MailAddress(remitenteCorreo);
                foreach (string mail in destinarioCorreo)
                {
                    mailMessage.To.Add(mail);
                }
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.Priority = MailPriority.Normal;
                stmpClient.Send(mailMessage);
            }
            catch (Exception ex) { }
            finally
            {
                mailMessage.Dispose();
                stmpClient.Dispose();
            }
        }

        public string recoverPassword(string usuarioSolicitando)
        {
            using (var db = new ContextDataBase())
            {

                var user = db.usuarios.FirstOrDefault(u => u.UserName == usuarioSolicitando);

                if (user != null)
                {

                    user.IsPasswordChangeRequired = true;
                    user.Password = user.EstablecerContraseña("1234");

                    db.SaveChangesAsync();
                }

            }

            using (var context = new ContextDataBase())
            {
                var user = context.usuarios.FirstOrDefault(u => u.UserName == usuarioSolicitando || u.Email == usuarioSolicitando);
                if (user != null)
                {
                    string nombreUsuario = user.Name;
                    string correoUsuario = user.Email;


                    //string password = user.Password;

                    enviarCorreo(
                        subject: "SISTEMA DE CONTROL DE INVENTARIOS TI: Solicitud de recuperacion de contraseña",
                        body: "Hola, " + nombreUsuario + "\nUsted Solicitó Recuperación de Contraseña.\n" +
                        "\nSin embargo, le pedimos que cambie su contraseña inmediatamente una vez que ingrese al Sistema...",
                        destinarioCorreo: new List<string> { correoUsuario }
                    );
                    return "Hola, " + nombreUsuario + "\nUsted Solicitó Recupeación de Contraseña.\n" +
                        "Por favor revise su correo: " + correoUsuario +
                        "\nSin embargo, le pedimos que cambie su contraseña inmediatamente una vez que ingrese al Sistema...";
                }
                else
                {
                    return "Lo Sentimos, no tiene una cuenta con ese correo o nombre del usuario";
                }
            }
        }
    }
}
