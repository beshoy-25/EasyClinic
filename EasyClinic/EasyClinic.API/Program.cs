
using EasyClinic.API.Data;
using EasyClinic.API.Endpoints;
using EasyClinic.API.Filters;
using EasyClinic.API.Middlewares;
using EasyClinic.API.Options;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

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
            builder.Services.AddSwaggerGen();

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

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();   // ← لازم يكون من أول حاجة في الـ Pipeline

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {                
                app.UseSwagger();   
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();            

            app.MapControllers();
            app.MapSpecializationEndpoints();
            app.MapAppointmentStatusEndpoints();

            app.Run();
        }
    }
}
