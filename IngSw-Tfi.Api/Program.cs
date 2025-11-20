
using IngSw_Tfi.Api.Middlewares;
using IngSw_Tfi.Application;
using IngSw_Tfi.Data;
using IngSw_Tfi.Transversal.Services;
using IngSw_Tfi.Domain.Interfaces;

namespace IngSw_Tfi.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            
            // Registrar servicios de las capas
            builder.Services.AddDataServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            
            // Registrar servicios transversales
            builder.Services.AddScoped<ISocialWorkServiceApi, SocialWorkServiceApi>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            // Configurar CORS para permitir peticiones del frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Habilitar CORS
            app.UseCors("AllowFrontend");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
