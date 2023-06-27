using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
  public class AutorDTO
  {
    public int Id { get; set; }
    public string Nombre { get; set; }
  }
}
