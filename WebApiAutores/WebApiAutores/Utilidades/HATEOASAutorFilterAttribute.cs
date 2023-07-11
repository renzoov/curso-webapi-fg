using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAutores.DTOs;
using WebApiAutores.Servicios;

namespace WebApiAutores.Utilidades
{
  public class HATEOASAutorFilterAttribute : HATEOASFiltroAttribute
  {
    private readonly GeneradorEnlaces generadorEnlaces;

    public HATEOASAutorFilterAttribute(GeneradorEnlaces generadorEnlaces)
    {
      this.generadorEnlaces = generadorEnlaces;
    }

    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
      var debeIncluir = DebeIncluirHATEOAS(context);

      if (!debeIncluir)
      {
        await next();
        return;
      }

      var resultado = context.Result as ObjectResult;

      var autorDTO = resultado.Value as AutorDTO;
      if(autorDTO == null)
      {
        var autoresDTO = resultado.Value as List<AutorDTO> ?? 
          throw new ArgumentNullException("Se esperaba una instancia de AutorDTO o List<AutorDTO>");
  
        autoresDTO.ForEach(async autor => await generadorEnlaces.GenerarEnlaces(autor));
        resultado.Value = autoresDTO;
      }
      else
      {
        await generadorEnlaces.GenerarEnlaces(autorDTO);
      }

      await next();
    }
  }
}
