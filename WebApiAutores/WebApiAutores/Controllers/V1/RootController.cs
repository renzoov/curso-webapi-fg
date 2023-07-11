using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOS>>> Get()
        {
            var datosHateoas = new List<DatoHATEOS>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            datosHateoas.Add(new DatoHATEOS(enlace: Url.Link("ObtenerRoot", new { }), descripcion: "self", metodo: "GET"));

            datosHateoas.Add(new DatoHATEOS(enlace: Url.Link("obtenerAutores", new { }), descripcion: "autores", metodo: "GET"));

            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new DatoHATEOS(enlace: Url.Link("crearAutor", new { }), descripcion: "autor-crear", metodo: "POST"));
                datosHateoas.Add(new DatoHATEOS(enlace: Url.Link("crearLibro", new { }), descripcion: "libro-crear", metodo: "POST"));
            }

            return datosHateoas;
        }
    }
}
