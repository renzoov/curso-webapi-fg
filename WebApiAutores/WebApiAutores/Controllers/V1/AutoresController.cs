using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Servicios;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V1
{
  [ApiController]
  //[Route("api/v1/autores")] // o puede ser api/[controller]
  [Route("api/autores")]
  [CabeceraEstaPresente("x-version", "1")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
  //[ApiConventionType(typeof(DefaultApiConventions))] // para que se vea la documentacion de swagger
  public class AutoresController : ControllerBase
  {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly IAuthorizationService authorizationService;
    private readonly IServicio servicio;
    private readonly ServicioTransient servicioTransient;
    private readonly ServicioScoped servicioScoped;
    private readonly ServicioSingleton servicioSingleton;
    private readonly ILogger<AutoresController> logger;

    public AutoresController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
    {
      this.context = context;
      this.mapper = mapper;
      this.authorizationService = authorizationService;
    }

    //[HttpGet("configuraciones")]
    //public ActionResult<string> ObtenerConfiguracion()
    //{
    //  return configuration["ConnectionStrings:defaultConnection"];
    //}

    //[HttpGet("listado")] // se puede tener 2 rutas api/autores/listado
    //[HttpGet("/listado")] // /listado
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet(Name = "obtenerAutoresV1")] // api/autores
    [AllowAnonymous]
    [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
    public async Task<ActionResult<List<AutorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
    {
      //throw new NotImplementedException();
      //logger.LogInformation("Estamos obteniendo los autores");
      //logger.LogWarning("Este es un mensaje de prueba");
      //servicio.RealizarTarea();
      //return await context.Autores.Include(x => x.Libros).ToListAsync();

      var queryable = context.Autores.AsQueryable();
      await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
      var autores = await queryable.OrderBy(autor => autor.Nombre).Paginar(paginacionDTO).ToListAsync();
      return mapper.Map<List<AutorDTO>>(autores);
    }

    //[HttpGet("primero")]
    //// public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int valor, [FromQuery] string nombre) 
    //// valor puede venir de la cabecera o de la ruta - query (api/autores/primero?nombre=juan&apellido=chavez)
    //public async Task<ActionResult<Autor>> PrimerAutor()
    //{
    //  return await context.Autores.FirstOrDefaultAsync();
    //}

    // [HttpGet("{id:int}/{param2}")] // si no mando 2 parametro se cae, api/autores/1/param2
    // [HttpGet("{id:int}/{param2?}")] // si no mando 2 parametro no se cae porque acepta null, api/autores/1
    // [HttpGet("{id:int}/{param2=default}")] // agrega parametro por defecto, api/autores/1/default
    [HttpGet("{id:int}", Name = "obtenerAutorV1")]
    [AllowAnonymous]
    [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
    //[ProducesResponseType(404)]
    //[ProducesResponseType(200)]
    public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
    {
      var autor = await context.Autores
        .Include(autorDB => autorDB.AutoresLibros)
        .ThenInclude(autorLibroDB => autorLibroDB.Libro)
        .FirstOrDefaultAsync(autorBD => autorBD.Id == id);

      if (autor == null) return NotFound();

      var dto = mapper.Map<AutorDTOConLibros>(autor);
      return dto;
    }

    [HttpGet("{nombre}", Name = "obtenerAutorPorNombreV1")]
    // public async Task<ActionResult<Autor>> Get(string nombre)
    public async Task<ActionResult<List<AutorDTO>>> GetPorNombre([FromRoute] string nombre)
    {
      var autores = await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

      return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpPost(Name = "crearAutorV1")]
    // public async Task<ActionResult> Post(Autor autor) 
    public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
    {
      var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);

      if (existeAutorConElMismoNombre)
      {
        return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
      }

      var autor = mapper.Map<Autor>(autorCreacionDTO);

      context.Add(autor);
      await context.SaveChangesAsync();

      var autorDTO = mapper.Map<AutorDTO>(autor);

      return CreatedAtRoute("obtenerAutorV1", new { id = autor.Id }, autorDTO);
    }

    [HttpPut("{id:int}", Name = "actualizarAutorV1")]
    public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
    {
      var existe = await context.Autores.AnyAsync(x => x.Id == id);

      if (!existe)
      {
        return NotFound();
      }

      var autor = mapper.Map<Autor>(autorCreacionDTO);
      autor.Id = id;

      context.Update(autor);
      await context.SaveChangesAsync();
      return NoContent();
    }

    /// <summary>
    /// Borra un autor
    /// </summary>
    /// <param name="id">Id del autor a borrar</param>
    /// <returns></returns>
    [HttpDelete("{id:int}", Name = "borrarAutorV1")]
    public async Task<ActionResult> Delete(int id)
    {
      var existe = await context.Autores.AnyAsync(x => x.Id == id);

      if (!existe)
      {
        return NotFound();
      }

      context.Remove(new Autor() { Id = id });
      await context.SaveChangesAsync();
      return NoContent();
    }
  }
}
