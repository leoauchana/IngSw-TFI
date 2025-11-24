
using IngSw_Tfi.Api.Middlewares;
using IngSw_Tfi.Application;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Application.Services;
using IngSw_Tfi.Data;
using IngSw_Tfi.Transversal;

namespace IngSw_Tfi.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddApplicationServices();

            builder.Services.AddDataServices(builder.Configuration);

            builder.Services.AddTransversalServices();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins(
                            "http://localhost:5173"
                        );
                });
            });

            var app = builder.Build();

            app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
