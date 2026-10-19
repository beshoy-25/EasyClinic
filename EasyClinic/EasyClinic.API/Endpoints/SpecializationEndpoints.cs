
using EasyClinic.API.Data;
using EasyClinic.API.DTOs.Specializations;
using EasyClinic.API.Models;
using M04.MinimalFluentValidation.Filters;
using Microsoft.EntityFrameworkCore;

namespace EasyClinic.API.Endpoints
{
    public static class SpecializationEndpoints
    {
        public static void MapSpecializationEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/specializations")
                            .WithTags("Specializations");

            group.MapGet("/", GetAll);
            group.MapGet("/{id:int:min(1)}", GetById);
            group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateSpecializationDTO>>();
            group.MapPut("/{id:int:min(1)}", Update).AddEndpointFilter<ValidationFilter<CreateSpecializationDTO>>();
            group.MapDelete("/{id:int:min(1)}", Delete);

        }

        private static async Task<IResult> GetAll(AppDbContext context, CancellationToken cancellationToken)
        {
            var specializations = await context.Specializations.
                Select(s => new SpecializationDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                })
                .ToListAsync(cancellationToken);

            return Results.Ok(specializations);
        }

        private static async Task<IResult> GetById(int id, AppDbContext context, CancellationToken cancellationToken)
        {
            var specialization = await context.Specializations
            .Where(s => s.Id == id)
            .Select(s => new SpecializationDTO
            {
                Id = s.Id,
                Name = s.Name,
            }).FirstOrDefaultAsync(cancellationToken);

            return specialization is null ? Results.NotFound() : Results.Ok(specialization);
        }

        private static async Task<IResult> Create(CreateSpecializationDTO dto, AppDbContext context)
        {
            var specialization = new Specialization { Name = dto.Name };

            context.Specializations.Add(specialization);
            await context.SaveChangesAsync();

            var result = new SpecializationDTO { Id = specialization.Id, Name = specialization.Name };

            return Results.Created($"/api/specializations/{specialization.Id}", result);

        }


        private static async Task<IResult> Update(int id,CreateSpecializationDTO dto,AppDbContext  context)
        {
            var specialization = await context.Specializations.FindAsync(id);

            if (specialization is null)
                return Results.NotFound();

            specialization.Name = dto.Name;
            await context.SaveChangesAsync();

            return Results.NoContent();
        }

        private static async Task<IResult> Delete(int id ,AppDbContext context)
        {            
            var specialization = await context.Specializations.FindAsync(id);
            if (specialization is null)
                return Results.NotFound();
            var hasDoctors = await context.Doctors.AnyAsync(d => d.SpecializationId == id);
            if (hasDoctors)
                return Results.Problem(
                    detail: "Cannot delete this specialization because it has doctors linked to it.",
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Cannot Delete Specialization");

            context.Specializations.Remove(specialization);
            await context.SaveChangesAsync();

            return Results.NoContent();
        }


    }
}
