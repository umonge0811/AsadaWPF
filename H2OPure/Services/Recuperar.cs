using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace H2OPure.Services
{
    public class Recuperar
    {
       // private SmtpClient stmpClient;

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
            passwordRemitente = "wchawtlmpgcgmsbu";
            host = "smtp.gmail.com";
            puerto = 587;
            ssl = true;
        }

        public void enviarCorreo(string subject, string body, List<string> destinarioCorreo)
        {
            var smtpClient = new SmtpClient(host)
            {
                Port = puerto,
                Credentials = new NetworkCredential(remitenteCorreo, passwordRemitente),
                EnableSsl = true,
            };

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
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex) { }
            finally
            {
                mailMessage.Dispose();
                smtpClient.Dispose();
            }
        }

        public string recoverPassword(string usuarioSolicitando)
        {
            string contraseñaAleatoria = null; // Declarar aquí

            using (var db = new ContextDataBase())
            {
                var user = db.usuarios.FirstOrDefault(u => u.UserName == usuarioSolicitando);

                if (user != null)
                {
                    user.IsPasswordChangeRequired = true;
                    // Generar una contraseña aleatoria
                    contraseñaAleatoria = Path.GetRandomFileName().Replace(".", "").Substring(0, 8);
                    user.Password = user.EstablecerContraseña(contraseñaAleatoria);
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

                    enviarCorreo(
                        subject: "SISTEMA DE CONTROL DE INGRESO H2OPure: Solicitud de recuperacion de contraseña",
                        body: "Hola, " + nombreUsuario + "\nUsted Solicitó Recuperación de Contraseña.\n" +
                        "\nSin embargo, le pedimos que cambie su contraseña inmediatamente una vez que ingrese al Sistema. Su contraseña temporal es " + contraseñaAleatoria,
                        destinarioCorreo: new List<string> { correoUsuario }
                    );
                    return "Hola, " + nombreUsuario + ", se envió contraseña temporal a " + correoUsuario;
                }
                else
                {
                    return "Lo Sentimos, no tiene una cuenta con ese correo o nombre del usuario";
                }
            }
        }

    }
}
