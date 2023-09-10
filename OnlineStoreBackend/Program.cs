using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace OnlineStoreBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            string avatar = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/avatar");

            try
            {
                if (!Directory.Exists(avatar))
                {
                    Directory.CreateDirectory(avatar);
                    Console.WriteLine("avatar folder created");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when creating folder: {ex.Message}");
            }

            var connectionString = builder.Configuration.GetSection("AppSettings:DbConnect");
            DataContext.ConnectionString = connectionString.Value;

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("PublicCors", builder =>
                {
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowAnyOrigin();
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = builder.Configuration.GetSection("AppSettings:Issuer").Value,
                                ValidateAudience = true,
                                ValidAudience = builder.Configuration.GetSection("AppSettings:Audience").Value,
                                ValidateLifetime = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
                                ValidateIssuerSigningKey = true
                            };
                    });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseCors("PublicCors");

            app.MapControllers();

            app.Run();
        }
    }
}