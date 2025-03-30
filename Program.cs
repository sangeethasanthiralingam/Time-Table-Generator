using Microsoft.EntityFrameworkCore;
using Time_Table_Generator.Models;
using Microsoft.OpenApi.Models;

namespace Time_Table_Generator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Register the DbContext with MySQL
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );


            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Time Table Generator API", Version = "v1" });
            });

            // Add CORS policy
       /*     builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.AllowAnyOrigin() // Allow any origin (use only for development!)
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                });
            });*/


            // Add services
           /* builder.Services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 7289; // Ensure HTTPS redirection is configured
            });

            // Add AppDbContext to the service container with MySQL
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 25)))); */// Specify MySQL version

            

            // Apply migrations at startup
          /*  using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate(); // Ensure migrations are applied
            }*/

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            /*      if (app.Environment.IsDevelopment())
                  {
                      app.UseSwagger();
                      app.UseSwaggerUI(c =>
                      {
                          c.SwaggerEndpoint("/swagger/v1/swagger.json", "Time Table Generator API v1");
                          // Set Swagger UI at the root
                          c.RoutePrefix = string.Empty;
                      });
                  }

                  if (!app.Environment.IsDevelopment())
                  {
                      app.UseHsts();
                  }*/

            // Use CORS
            /*   app.UseCors("AllowAllOrigins");*/

            // Add a redirect for the root URL
            /*     app.MapGet("/", () => Results.Redirect("/swagger"));*/

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection(); // Enforce HTTPS redirection
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
