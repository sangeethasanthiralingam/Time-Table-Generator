using Microsoft.EntityFrameworkCore;
using Time_Table_Generator.Models;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

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
          builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.AllowAnyOrigin() // Allow any origin (use only for development!)
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                });
            });

        // Load configuration for JWT
        var jwtSettings = builder.Configuration.GetSection("Jwt");

        // Register services
        builder.Services.AddControllers();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 25))));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new ArgumentNullException("JWT Key cannot be null")))
                };
            });


            // Add services
            builder.Services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 7289; // Ensure HTTPS redirection is configured
            });

            // Add AppDbContext to the service container with MySQL
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 25)))); // Specify MySQL version

            var app = builder.Build();

            // Apply migrations at startup
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate(); // Ensure migrations are applied
            }

            // Configure the HTTP request pipeline.
                 if (app.Environment.IsDevelopment())
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
                  }

            // Use CORS
               app.UseCors("AllowAllOrigins");

            // Add a redirect for the root URL
                app.MapGet("/", () => Results.Redirect("/swagger"));

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection(); // Enforce HTTPS redirection
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
