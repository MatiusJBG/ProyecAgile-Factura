using System.ComponentModel.DataAnnotations;
namespace Core.Entities;

public class Usuario
{
    [Key]
    public int Id_Usu { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nom_Usu { get; set; }

    [Required]
    [MaxLength(255)]
    public string Contrasena_Usu { get; set; }
}
