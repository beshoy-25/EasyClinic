
using EasyClinic.API.Auth;
using EasyClinic.API.Data;
using EasyClinic.API.Endpoints;
using EasyClinic.API.Filters;
using EasyClinic.API.Middlewares;
using EasyClinic.API.Options;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

namespace EasyClinic.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi            
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Paste the JWT token only, without the 'Bearer ' prefix."
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            });

            // ????? ??? DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                );

            builder.Services
                .AddOptions<ClinicSettings>()
                .Bind(builder.Configuration.GetSection(ClinicSettings.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<LoggingActionFilter>();
            });

            builder.Services.AddProblemDetails();


            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            builder.Services
                .AddOptions<JwtSettings>()
                .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var jwtSettings = builder.Configuration
                .GetSection(JwtSettings.SectionName)
                .Get<JwtSettings>()!;

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            //App Full Blocked for anyone has not Authenticate
            //to make controller open in puplec like login use 
            //.AllowAnonymous()      // Minimal API 
            //[AllowAnonymous]       // Controller
            builder.Services.AddAuthorizationBuilder()                  
                .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build())
                .AddPolicy(Policies.AdminOnly, p => p.RequireRole(Roles.Admin))
                .AddPolicy(Policies.StaffOnly, p => p.RequireRole(Roles.Admin, Roles.Receptionist));

            builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

            builder.Services
                .AddOptions<CorsSettings>()
                .Bind(builder.Configuration.GetSection(CorsSettings.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var corsSettings = builder.Configuration
                .GetSection(CorsSettings.SectionName)
                .Get<CorsSettings>()!;

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsSettings.PolicyName, policy =>
                    policy.WithOrigins(corsSettings.AllowedOrigins)
                          .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
                          .WithHeaders("Authorization", "Content-Type"));
            });

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();   // ← لازم يكون من أول حاجة في الـ Pipeline
            app.UseStatusCodePages();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {                
                app.UseSwagger();   
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCors(CorsSettings.PolicyName);

            app.UseAuthentication();            
            app.UseAuthorization();

            app.MapControllers();

            app.MapAuthEndpoints();
            app.MapSpecializationEndpoints();
            app.MapAppointmentStatusEndpoints();
            

            app.Run();
        }
    }
}
