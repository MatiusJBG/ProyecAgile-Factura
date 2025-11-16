using System.ComponentModel.DataAnnotations;
namespace Core.Entities;

public class Auditoria
{
    [Key]
    public int Id_Aud { get; set; }

    public string Usuario { get; set; }

    public string Accion { get; set; }

    public string Entidad { get; set; }  

    public int? IdEntidad { get; set; }

    public string Detalle { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
}
