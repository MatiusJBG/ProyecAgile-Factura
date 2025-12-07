using System.Linq.Expressions;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Common
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            try
            {
                Console.WriteLine($"[REPO] AddAsync - INICIO para {typeof(T).Name}");
                await _dbSet.AddAsync(entity);
                
                Console.WriteLine($"[REPO] AddAsync - Llamando SaveChangesAsync...");
                var affected = await _context.SaveChangesAsync();
                Console.WriteLine($"[REPO] AddAsync - SaveChangesAsync COMPLETADO. Filas afectadas: {affected}");
                
                if (affected == 0)
                {
                    Console.WriteLine($"[REPO WARNING] SaveChangesAsync retornó 0 filas afectadas!");
                }
                
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPO ERROR] AddAsync falló: {ex.Message}");
                Console.WriteLine($"[REPO ERROR] InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"[REPO ERROR] Stack: {ex.StackTrace}");
                throw;
            }
        }

        public virtual async Task UpdateAsync(T entity)
        {
            try
            {
                Console.WriteLine($"[REPO] UpdateAsync - INICIO para {typeof(T).Name}");
                _dbSet.Update(entity);
                
                Console.WriteLine($"[REPO] UpdateAsync - Llamando SaveChangesAsync...");
                var affected = await _context.SaveChangesAsync();
                Console.WriteLine($"[REPO] UpdateAsync - SaveChangesAsync COMPLETADO. Filas afectadas: {affected}");
                
                if (affected == 0)
                {
                    Console.WriteLine($"[REPO WARNING] SaveChangesAsync retornó 0 filas afectadas!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPO ERROR] UpdateAsync falló: {ex.Message}");
                Console.WriteLine($"[REPO ERROR] InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"[REPO ERROR] Stack: {ex.StackTrace}");
                throw;
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }
    }
}
