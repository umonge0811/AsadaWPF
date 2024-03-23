using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using wpfASADACore.Models;

namespace wpfASADACore.Services
{

    //Es la clase que se encarga de la conexión con la base de datos
   public class ContextDataBase : DbContext
    {
        static string database = "asada.db";
        public DbSet<clsUser> usuarios { get; set; }
        public DbSet<clsCliente> clientes { get; set; }
        public DbSet<clsTypeClient> typeClients { get; set; }


        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString: "Filename=" + database, 
                sqliteOptionsAction: op => {
                    op.MigrationsAssembly(
                            Assembly.GetExecutingAssembly().FullName
                        );
            });
            
            base.OnConfiguring(optionsBuilder);
        }

        //En este Metodo se crean las tablas de la base de datos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<clsUser>().ToTable("User");
            modelBuilder.Entity<clsUser>(entity => {
                entity.HasKey(e => e.Id); //indica que la propiedad Id es la clave principal de la Tabla User.
            });
            // Mapeo de la entidad de cliente a la tabla "Clientes"
            modelBuilder.Entity<clsCliente>().ToTable("Clients");
            modelBuilder.Entity<clsCliente>(entity =>
            {
                entity.HasKey(e => e.id);
                // Define la relación con TypeClient
                entity.HasOne(e => e.TypeClient)
                    .WithMany()  // indica que la relación es muchos a uno (un cliente puede tener un tipo de cliente, pero varios clientes pueden compartir el mismo tipo).
                    .HasForeignKey(e => e.TypeClientId) //especifica que la propiedad TypeClientId en la entidad de cliente es la clave foránea que se relaciona con la tabla de tipos de cliente.
                    .OnDelete(DeleteBehavior.Restrict);  // establece el comportamiento de eliminación. En este caso, restringe la eliminación del tipo de cliente si hay clientes asociados a él.
            });

            // Mapeo de la entidad de tipo de cliente a la tabla "TypeClients"
            modelBuilder.Entity<clsTypeClient>().ToTable("TypeClients");
            modelBuilder.Entity<clsTypeClient>(entity =>
            {
                entity.HasKey(e => e.id);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
