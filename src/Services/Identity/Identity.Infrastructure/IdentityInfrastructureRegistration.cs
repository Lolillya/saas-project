using System.Text;
using Identity.Application.Interfaces;
using Identity.Application.Services;
using Identity.Infrastructure.Auth;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Kernel;

namespace Identity.Infrastructure
{
    public static class IdentityInfrastructureRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind JwtSettings from appsettings/environment variables
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // Register JWT authentication scheme
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();

            // Register your Token Service
            services.AddScoped<ITokenServices, TokenService>();

            // DbContext
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("IdentityDb"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null
            )));

            return services;
        }
    }
}