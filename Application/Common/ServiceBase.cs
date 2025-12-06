using Core.Exceptions;
using Core.Interfaces.Clientes; using Core.Interfaces.Facturacion; using Core.Interfaces.Inventario; using Core.Interfaces.Auth; using Core.Interfaces.Certificados; using Core.Interfaces.Common;

namespace Application.Common
{
    /// <summary>
    /// Clase base para servicios que proporciona métodos helper comunes
    /// </summary>
    public abstract class ServiceBase<TEntity, TDto> where TEntity : class
    {
        /// <summary>
        /// Obtiene una entidad por ID o lanza EntityNotFoundException si no existe
        /// </summary>
        protected async Task<TEntity> GetEntityOrThrowAsync<TRepository>(
            TRepository repository,
            int id,
            string entityName)
            where TRepository : IRepository<TEntity>
        {
            var entity = await repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new EntityNotFoundException(entityName, id);
            }
            return entity;
        }

        /// <summary>
        /// Verifica si una entidad existe
        /// </summary>
        protected async Task<bool> EntityExistsAsync<TRepository>(
            TRepository repository,
            int id)
            where TRepository : IRepository<TEntity>
        {
            return await repository.ExistsAsync(id);
        }

        /// <summary>
        /// Mapea una entidad a su DTO correspondiente
        /// </summary>
        protected abstract TDto MapToDto(TEntity entity);

        /// <summary>
        /// Mapea un DTO a su entidad correspondiente
        /// </summary>
        protected abstract TEntity MapToEntity(TDto dto);
    }

    /// <summary>
    /// Helper estático para validaciones de entidades en cualquier repositorio
    /// </summary>
    public static class EntityValidator
    {
        /// <summary>
        /// Obtiene una entidad de cualquier repositorio o lanza EntityNotFoundException
        /// </summary>
        public static async Task<TEntity> GetOrThrowAsync<TEntity, TRepository>(
            TRepository repository,
            int id,
            string entityName)
            where TEntity : class
            where TRepository : IRepository<TEntity>
        {
            var entity = await repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new EntityNotFoundException(entityName, id);
            }
            return entity;
        }
    }
}
