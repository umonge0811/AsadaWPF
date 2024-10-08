﻿using H2OPure.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace H2OPure.Services
{
    public class ContextDataBase : DbContext
    {
        static string database = "asada.db";
        public DbSet<clsUser> usuarios { get; set; }
        public DbSet<clsCliente> clientes { get; set; }
        public DbSet<clsTypeClient> typeClients { get; set; }
        public DbSet<clsBilling> billings { get; set; }
        public DbSet<clsReading> readings { get; set; }
        public DbSet<clsInventory> inventories { get; set; }
        public DbSet<clsInventoryTransaction> InventoryTransactions { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<clsUser>().ToTable("User");
            modelBuilder.Entity<clsUser>(entity => {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<clsCliente>().ToTable("Clients");
            modelBuilder.Entity<clsCliente>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.HasOne(e => e.TypeClient)
                    .WithMany()
                    .HasForeignKey(e => e.TypeClientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<clsTypeClient>().ToTable("TypeClients");
            modelBuilder.Entity<clsTypeClient>(entity =>
            {
                entity.HasKey(e => e.id);
            });

            modelBuilder.Entity<clsBilling>().ToTable("Billings");
            modelBuilder.Entity<clsBilling>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.HasOne(e => e.Client)
                    .WithMany()
                    .HasForeignKey(e => e.idClient)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<clsReading>().ToTable("Readings");
            modelBuilder.Entity<clsReading>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.HasOne(e => e.Client)
                    .WithMany()
                    .HasForeignKey(e => e.idClient)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.TypeClient)
                    .WithMany()
                    .HasForeignKey(e => e.TypeClientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<clsInventory>().ToTable("Inventories");
            modelBuilder.Entity<clsInventory>(entity =>
            {
                entity.HasKey(e => e.id); // llave Primaria
                entity.HasOne(e => e.User) // Relación con la tabla de usuarios de 1:N
                    .WithMany() // Relación de 1:N
                    .HasForeignKey(e => e.userId) // Llave foránea que se relaciona con la tabla User.
                    .OnDelete(DeleteBehavior.Restrict); // Restricción de eliminación impide que se elimine el User si tiene Inventories asociados.
            });

            modelBuilder.Entity<clsInventoryTransaction>().ToTable("InventoryTransactions");
            modelBuilder.Entity<clsInventoryTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
