using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;

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
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIAutores", Version = "v1" });

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
          builder.WithOrigins("https://www.apirequest.io").AllowAnyMethod().AllowAnyHeader();
        });
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
      //app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
      app.UseLoguearRespuestaHTTP();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIAutores");
          c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
        });
      }

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
