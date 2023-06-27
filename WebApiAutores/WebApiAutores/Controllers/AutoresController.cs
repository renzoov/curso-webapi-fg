using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
  [ApiController]
  [Route("api/autores")] // o puede ser api/[controller]
  //[Authorize]
  public class AutoresController : ControllerBase
  {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly IServicio servicio;
    private readonly ServicioTransient servicioTransient;
    private readonly ServicioScoped servicioScoped;
    private readonly ServicioSingleton servicioSingleton;
    private readonly ILogger<AutoresController> logger;

    public AutoresController(ApplicationDbContext context, IMapper mapper)
    {
      this.context = context;
      this.mapper = mapper;
    }


    [HttpGet] // api/autores
    //[HttpGet("listado")] // se puede tener 2 rutas api/autores/listado
    //[HttpGet("/listado")] // /listado
    //[Authorize]
    public async Task<ActionResult<List<AutorDTO>>> Get()
    {
      //throw new NotImplementedException();
      //logger.LogInformation("Estamos obteniendo los autores");
      //logger.LogWarning("Este es un mensaje de prueba");
      //servicio.RealizarTarea();
      //return await context.Autores.Include(x => x.Libros).ToListAsync();

      var autores = await context.Autores.ToListAsync();
      return mapper.Map<List<AutorDTO>>(autores);
    }

    //[HttpGet("primero")]
    //// public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int valor, [FromQuery] string nombre) 
    //// valor puede venir de la cabecera o de la ruta - query (api/autores/primero?nombre=juan&apellido=chavez)
    //public async Task<ActionResult<Autor>> PrimerAutor()
    //{
    //  return await context.Autores.FirstOrDefaultAsync();
    //}

    [HttpGet("{id:int}", Name = "obtenerAutor")]
    // [HttpGet("{id:int}/{param2}")] // si no mando 2 parametro se cae, api/autores/1/param2
    // [HttpGet("{id:int}/{param2?}")] // si no mando 2 parametro no se cae porque acepta null, api/autores/1
    // [HttpGet("{id:int}/{param2=default}")] // agrega parametro por defecto, api/autores/1/default
    public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
    {
      var autor = await context.Autores
        .Include(autorDB => autorDB.AutoresLibros)
        .ThenInclude(autorLibroDB => autorLibroDB.Libro)
        .FirstOrDefaultAsync(autorBD => autorBD.Id == id);

      if (autor == null) return NotFound();

      return mapper.Map<AutorDTOConLibros>(autor);
    }

    [HttpGet("{nombre}")]
    // public async Task<ActionResult<Autor>> Get(string nombre)
    public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string nombre)
    {
      var autores = await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

      return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpPost]
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

      return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDTO);
    }

    [HttpPut("{id:int}")]
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

    [HttpDelete("{id:int}")]
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
