namespace WebApiAutores.DTOs
{
  public class DatoHATEOS
  {
    public string Enlace { get; private set; }
    public string Descripcion { get; private set; }
    public string Metodo { get; private set; }

    public DatoHATEOS(string enlace, string descripcion, string metodo)
    {
      Enlace = enlace;
      Descripcion = descripcion;
      Metodo = metodo;
    }
  }
}
