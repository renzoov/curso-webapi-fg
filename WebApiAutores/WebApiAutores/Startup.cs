using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;
using WebApiAutores.Utilidades;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace WebApiAutores
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      // Limpia el mapeo de los claims para que no se mapeen los claims por defecto
      JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers(opciones =>
      {
        opciones.Filters.Add(typeof(FiltroDeExcepcion));
        opciones.Conventions.Add(new SwaggerAgrupaPorVersion());
      }).AddJsonOptions(x =>
          x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();

      services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
          ClockSkew = TimeSpan.Zero
        });

      services.AddEndpointsApiExplorer();
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { 
          Title = "WebAPIAutores", 
          Version = "v1",
          Description = "Esta es una descripción de la API",
          Contact = new OpenApiContact
          {
            Name = "Pepito Perez",
            Email = "pepito@gmail.com",
            Url = new Uri("https://google.com")
          },
          License = new OpenApiLicense
          {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
          }
        });
        c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebAPIAutores", Version = "v2" });
        c.OperationFilter<AgregarParametroHATEOAS>();
        c.OperationFilter<AgregarParametroXVersion>();

        // Configuración para que Swagger pueda autenticarse
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          Name = "Authorization",
          Type = SecuritySchemeType.ApiKey,
          Scheme = "Bearer",
          BearerFormat = "JWT",
          In = ParameterLocation.Header,
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
          {
            new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference
              {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
              }
            },
            new string[] { }
          }
        });

        var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var rutaXML = Path.Combine(AppContext.BaseDirectory, archivoXML);
        c.IncludeXmlComments(rutaXML);
      });

      services.AddAutoMapper(typeof(Startup));

      services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

      services.AddAuthorization(opciones =>
      {
        opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
      });

      services.AddDataProtection();
      services.AddTransient<HashService>();

      services.AddCors(opciones =>
      {
        opciones.AddDefaultPolicy(builder =>
        {
          builder.WithOrigins("https://www.apirequest.io").AllowAnyMethod().AllowAnyHeader()
          .WithExposedHeaders(new string[] { "cantidadTotalRegistros" });
        });
      });

      services.AddTransient<GeneradorEnlaces>();
      services.AddTransient<HATEOASAutorFilterAttribute>();
      services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

      services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:ConnectionString"]);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
      //app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
      app.UseLoguearRespuestaHTTP();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIAutores v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebAPIAutores v2");
        c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
      });

      app.UseHttpsRedirection();
      app.UseRouting();

      app.UseCors();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

      app.UseStaticFiles();
    }
  }
}
