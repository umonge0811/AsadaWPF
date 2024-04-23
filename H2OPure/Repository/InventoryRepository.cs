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
            // Crear una nueva instancia de la base de datos
            context = new ContextDataBase();
            // Asegurarse de que la base de datos esté creada
            context.Database.EnsureCreatedAsync().Wait();
        }

        public async Task UpdateInventory(clsInventory inventory)
        {
            // Actualizar el inventario en la base de datos
            context.inventories.Update(inventory);
            // Guardar los cambios en la base de datos
            await context.SaveChangesAsync();
        }

        public async Task<List<clsInventory>> GetAllInventory()
        {
            try
            {
                // Obtener todos los inventarios de la base de datos
                return await context.inventories.Include(i => i.User).ToListAsync();
            }
            catch (Exception ex)
            {
                // Imprimir el error y lanzar la excepción
                Console.WriteLine("Error al obtener todos los inventarios: " + ex.Message);
                throw;
            }
        }

        public async Task AddInventory(clsInventory inventory)
        {
            // Buscar si el material ya existe en el inventario y pertenece al mismo departamento
            var existingInventory = await context.inventories.FirstOrDefaultAsync(i => i.materialName == inventory.materialName && i.Department == inventory.Department);

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

            // Guardar los cambios en la base de datos
            await context.SaveChangesAsync();
        }

        public async Task AddInventoryOut(clsInventory inventory)
        {
            // Crear un nuevo registro de salida
            clsInventory newInventoryOut = new clsInventory
            {
                materialName = inventory.materialName,
                Department = inventory.Department,
                quantity = -inventory.quantity, // La cantidad es negativa para indicar una salida
                Remarks = inventory.Remarks,
                exitDate = DateTime.Now,
                userId = inventory.userId,
                action = "Salida"
            };

            // Agregar el nuevo registro a la base de datos
            context.inventories.Add(newInventoryOut);

            // Guardar los cambios en la base de datos
            await context.SaveChangesAsync();
        }

        public async Task RemoveInventory(clsInventory inventory)
        {
            // Buscar si el material ya existe en el inventario
            var existingInventory = await context.inventories.FirstOrDefaultAsync(i => i.materialName == inventory.materialName);

            if (existingInventory != null)
            {
                // Si el material ya existe, decrementar la cantidad
                existingInventory.quantity -= inventory.quantity;
            }
            else
            {
                // Si el material no existe, lanzar una excepción
                throw new Exception("Material no disponible o cantidad insuficiente");
            }

            // Guardar los cambios en la base de datos
            await context.SaveChangesAsync();
        }

        public async Task<List<clsInventory>> GetAllMaterialsIN()
        {
            try
            {
                // Obtener todos los materiales disponibles en el inventario
                return await context.inventories.Where(i => i.quantity > 0).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener todos los materiales: " + ex.Message);
                throw;
            }
        }

        public async Task<List<clsInventory>> GetAvailableMaterialsOUT(string department)
        {
            try
            {
                // Obtener los materiales disponibles en el inventario que pertenecen al departamento dado y tienen una cantidad mayor que 0
                return await context.inventories.Where(i => i.Department == department && i.quantity > 0).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los materiales disponibles: " + ex.Message);
                throw;
            }
        }




        public async Task<int> GetAvailableQuantity(string materialName, string department)
        {
            int totalIn = await context.InventoryTransactions
                .Where(t => t.MaterialName == materialName && t.Department == department && t.Action == "Entrada")
                .SumAsync(t => t.Quantity);

            int totalOut = await context.InventoryTransactions
                .Where(t => t.MaterialName == materialName && t.Department == department && t.Action == "Salida")
                .SumAsync(t => t.Quantity);

            return totalIn - totalOut;
        }


        public async Task AddInventoryTransaction(clsInventoryTransaction transaction)
        {
            context.InventoryTransactions.Add(transaction);
            await context.SaveChangesAsync();
        }

        public async Task<List<clsInventoryTransaction>> GetAllInventoryTransactions()
        {
            try
            {
                // Obtener todas las transacciones de inventario de la base de datos
                return await context.InventoryTransactions.Include(i => i.User).ToListAsync();
            }
            catch (Exception ex)
            {
                // Imprimir el error y lanzar la excepción
                Console.WriteLine("Error al obtener todas las transacciones de inventario: " + ex.Message);
                throw;
            }
        }

        public async Task<List<clsInventoryTransaction>> GetInventoryTransactionsForReport()
        {
            try
            {
                // Obtener todas las transacciones de inventario de la base de datos
                return await context.InventoryTransactions.Include(i => i.User).ToListAsync();
            }
            catch (Exception ex)
            {
                // Imprimir el error y lanzar la excepción
                Console.WriteLine("Error al obtener las transacciones de inventario para el reporte: " + ex.Message);
                throw;
            }
        }



    }
}