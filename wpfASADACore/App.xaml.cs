using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore.Infrastructure;
using wpfASADACore.Services;

namespace wpfASADACore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            /*Esto garantiza de que la base de datos para el contexto existente exista, si existiera, no se realiza ninguna acción , si no existe
            entonces se crea la base de datos on todo su esquema, Entonces significa que se crearan todas las tablas en esa base de datos*/
            DatabaseFacade facade = new DatabaseFacade(new ContextDataBase());
            facade.EnsureCreated();
        }
    }

}
