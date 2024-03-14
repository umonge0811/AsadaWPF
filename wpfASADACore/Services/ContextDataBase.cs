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
   public class ContextDataBase : DbContext
    {
        static string database = "asada.db";
        public DbSet<clsUser> usuarios { get; set; }

        
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
            base.OnModelCreating(modelBuilder);
        }
    }
}
