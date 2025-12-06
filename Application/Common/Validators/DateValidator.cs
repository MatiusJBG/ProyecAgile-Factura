using Core.Exceptions;

namespace Application.Common.Validators
{
    /// <summary>
    /// Validador estático para operaciones comunes con fechas
    /// </summary>
    public static class DateValidator
    {
        /// <summary>
        /// Valida que la fecha de expiración sea posterior a la fecha de entrada
        /// </summary>
        /// <param name="entryDate">Fecha de entrada</param>
        /// <param name="expirationDate">Fecha de expiración</param>
        /// <param name="context">Contexto opcional para el mensaje de error</param>
        /// <exception cref="BusinessValidationException">Si la validación falla</exception>
        public static void ValidateExpirationAfterEntry(
            DateTime entryDate,
            DateTime expirationDate,
            string context = "")
        {
            if (expirationDate <= entryDate)
            {
                var message = string.IsNullOrEmpty(context)
                    ? "La fecha de expiración debe ser posterior a la fecha de entrada"
                    : $"{context}: La fecha de expiración debe ser posterior a la fecha de entrada";
                throw new BusinessValidationException(message);
            }
        }

        /// <summary>
        /// Valida que una fecha no esté en el pasado
        /// </summary>
        public static void ValidateNotInPast(DateTime date, string fieldName = "La fecha")
        {
            if (date.Date < DateTime.Today)
            {
                throw new BusinessValidationException($"{fieldName} no puede estar en el pasado");
            }
        }

        /// <summary>
        /// Valida que una fecha esté dentro de un rango válido
        /// </summary>
        public static void ValidateDateRange(
            DateTime date,
            DateTime minDate,
            DateTime maxDate,
            string fieldName = "La fecha")
        {
            if (date < minDate || date > maxDate)
            {
                throw new BusinessValidationException(
                    $"{fieldName} debe estar entre {minDate:dd/MM/yyyy} y {maxDate:dd/MM/yyyy}");
            }
        }
    }
}
