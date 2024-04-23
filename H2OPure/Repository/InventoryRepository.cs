using H2OPure.Models;
using H2OPure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2OPure.Repository
{
    class InventoryRepository
    {
        private ContextDataBase context;

        public InventoryRepository()
        {
            context = new ContextDataBase();
            context.Database.EnsureCreatedAsync().Wait();
        }


        public async Task UpdateInventory(clsInventory inventory)
        {
            context.inventories.Update(inventory);
            await context.SaveChangesAsync();
        }

        public async Task<List<clsInventory>> GetAllInventory()
        {
            try
            {
                return await context.inventories.Include(i => i.User).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener todos los inventarios: " + ex.Message);
                throw;
            }
        }

        public async Task AddInventory(clsInventory inventory)
        {
            // Buscar si el material ya existe en el inventario
            var existingInventory = await context.inventories.FirstOrDefaultAsync(i => i.materialName == inventory.materialName);

            if (existingInventory != null)
            {
                // Si el material ya existe, incrementar la cantidad
                existingInventory.quantity += inventory.quantity;
            }
            else
            {
                // Si el material no existe, agregarlo al inventario
                context.inventories.Add(inventory);
            }

            await context.SaveChangesAsync();
        }

        public async Task RemoveInventory(string materialName, int quantity)
        {
            // Buscar el material en el inventario
            var existingInventory = await context.inventories.FirstOrDefaultAsync(i => i.materialName == materialName);

            if (existingInventory != null && existingInventory.quantity >= quantity)
            {
                // Si el material existe y hay suficiente cantidad, decrementar la cantidad
                existingInventory.quantity -= quantity;
                await context.SaveChangesAsync();
            }
            else
            {
                // Si el material no existe o no hay suficiente cantidad, lanzar una excepción
                throw new Exception("Material no disponible o cantidad insuficiente");
            }
        }

        public async Task<int> GetAvailableQuantity(string materialName)
        {
            // Buscar el material en el inventario
            var existingInventory = await context.inventories.FirstOrDefaultAsync(i => i.materialName == materialName);

            if (existingInventory != null)
            {
                // Si el material existe, devolver la cantidad disponible
                return existingInventory.quantity;
            }
            else
            {
                // Si el material no existe, devolver 0
                return 0;
            }
        }

        public async Task<List<clsInventory>> GetAvailableMaterials()
        {
            try
            {
                return await context.inventories.Where(i => i.quantity > 0).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los materiales disponibles: " + ex.Message);
                throw;
            }
        }



    }
}
