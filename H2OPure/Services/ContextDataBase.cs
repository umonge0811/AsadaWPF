using H2OPure.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace H2OPure.Services
{

    //Es la clase que se encarga de la conexión con la base de datos
    public class ContextDataBase : DbContext
    {
        static string database = "asada.db";
        public DbSet<clsUser> usuarios { get; set; }
        public DbSet<clsCliente> clientes { get; set; }
        public DbSet<clsTypeClient> typeClients { get; set; }
        public DbSet<clsBilling> billings { get; set; }
        public DbSet<clsReading> readings { get; set; }
        public DbSet<clsEmployee> employees { get; set; }
        public DbSet<clsInventory> inventories { get; set; }
        




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


            // Mapeo de la entidad de facturación a la tabla "Billings"
            modelBuilder.Entity<clsBilling>().ToTable("Billings");
            modelBuilder.Entity<clsBilling>(entity =>
            {
                entity.HasKey(e => e.id);
                // Define la relación con Client
                entity.HasOne(e => e.Client)
                    .WithMany()  // indica que la relación es muchos a uno (una factura puede tener un cliente, pero varios clientes pueden tener varias facturas).
                    .HasForeignKey(e => e.idClient) //especifica que la propiedad ClientId en la entidad de facturación es la clave foránea que se relaciona con la tabla de clientes.
                    .OnDelete(DeleteBehavior.Restrict);  // establece el comportamiento de eliminación. En este caso, restringe la eliminación del cliente si hay facturas asociadas a él.
            });

            // Mapeo de la entidad de lectura a la tabla "Readings"
            modelBuilder.Entity<clsReading>().ToTable("Readings");
            modelBuilder.Entity<clsReading>(entity =>
            {
                entity.HasKey(e => e.id);
                // Define la relación con Client
                entity.HasOne(e => e.Client)
                    .WithMany()  // indica que la relación es muchos a uno (una lectura puede tener un cliente, pero varios clientes pueden tener varias lecturas).
                    .HasForeignKey(e => e.idClient) //especifica que la propiedad ClientId en la entidad de lectura es la clave foránea que se relaciona con la tabla de clientes.
                    .OnDelete(DeleteBehavior.Restrict);  // establece el comportamiento de eliminación. En este caso, restringe la eliminación del cliente si hay lecturas asociadas a él.
                // Define la relación con TypeClient
                entity.HasOne(e => e.TypeClient)
                    .WithMany()  // indica que la relación es muchos a uno (una lectura puede tener un tipo de cliente, pero varios tipos de cliente pueden tener varias lecturas).
                    .HasForeignKey(e => e.TypeClientId) //especifica que la propiedad TypeClientId en la entidad de lectura es la clave foránea que se relaciona con la tabla de tipos de cliente.
                    .OnDelete(DeleteBehavior.Restrict);  // establece el comportamiento de eliminación. En este caso, restringe la eliminación del tipo de cliente si hay lecturas asociadas a él.
            });

            modelBuilder.Entity<clsInventory>().ToTable("Inventories");
            modelBuilder.Entity<clsInventory>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.HasOne(e => e.Employee)
                    .WithMany(e => e.Inventories)
                    .HasForeignKey(e => e.employeeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<clsEmployee>().ToTable("Employees");
            modelBuilder.Entity<clsEmployee>(entity =>
            {

                entity.HasKey(e => e.id);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.id)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
