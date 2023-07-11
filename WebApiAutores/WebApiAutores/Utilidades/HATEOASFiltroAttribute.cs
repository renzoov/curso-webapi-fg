using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Utilidades
{
  public class HATEOASFiltroAttribute : ResultFilterAttribute
  {
    protected bool DebeIncluirHATEOAS(ResultExecutingContext context)
    {
      var resultado = context.Result as ObjectResult;

      if(!EsRespuestaExitosa(resultado)) return false;

      var cabecera = context.HttpContext.Request.Headers["IncluirHATEOAS"];
      if (cabecera.Count == 0) return false;

      var valor = cabecera[0];
      if(!valor.Equals("Y", StringComparison.InvariantCultureIgnoreCase)) return false;

      return true;
    }

    private bool EsRespuestaExitosa(ObjectResult resultado)
    {
      if (resultado == null || resultado.Value == null) return false;
      if(resultado.StatusCode.HasValue && !resultado.StatusCode.Value.ToString().StartsWith("2")) return false;
      return true;
    }
  }
}
