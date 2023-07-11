using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
  public class AutorDTO : Recurso
  {
    public int Id { get; set; }
    public string Nombre { get; set; }
  }
}
