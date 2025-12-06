namespace Core.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando una entidad solicitada no existe en el repositorio
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        public string EntityName { get; }
        public int EntityId { get; }

        public EntityNotFoundException(string entityName, int id)
            : base($"{entityName} con ID {id} no encontrado")
        {
            EntityName = entityName;
            EntityId = id;
        }

        public EntityNotFoundException(string entityName, int id, Exception innerException)
            : base($"{entityName} con ID {id} no encontrado", innerException)
        {
            EntityName = entityName;
            EntityId = id;
        }
    }

    /// <summary>
    /// Excepción lanzada cuando se intenta crear una entidad que ya existe
    /// </summary>
    public class DuplicateEntityException : Exception
    {
        public DuplicateEntityException(string message) : base(message)
        {
        }

        public DuplicateEntityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Excepción lanzada cuando una operación de negocio no es válida
    /// </summary>
    public class BusinessValidationException : Exception
    {
        public BusinessValidationException(string message) : base(message)
        {
        }

        public BusinessValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
