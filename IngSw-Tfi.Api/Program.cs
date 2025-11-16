
using IngSw_Tfi.Api.Middlewares;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Application.Services;

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
