
using EasyClinic.API.Auth;
using EasyClinic.API.Data;
using EasyClinic.API.DTOs.Specializations;
using EasyClinic.API.Extensions;
using EasyClinic.Domain.Entities;
using EasyClinic.Domain.Errors;
using M04.MinimalFluentValidation.Filters;
using Microsoft.EntityFrameworkCore;

namespace EasyClinic.API.Endpoints
{
    public static class SpecializationEndpoints
    {
        public static void MapSpecializationEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/specializations")
                            .WithTags("Specializations")
                            .RequireAuthorization();

            group.MapGet("/", GetAll);
            group.MapGet("/{id:int:min(1)}", GetById)
                  .WithName("GetSpecializationById")
                 .WithSummary("Gets a specialization by id")
                 .WithDescription("Returns a single specialization, or 404 if it does not exist.")
                 .Produces<SpecializationDTO>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status401Unauthorized)
                 .ProducesProblem(StatusCodes.Status404NotFound);
            group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateSpecializationDTO>>().RequireAuthorization(Policies.AdminOnly)
                .AddEndpointFilter<ValidationFilter<CreateSpecializationDTO>>()
                .RequireAuthorization(Policies.AdminOnly)
                .WithName("CreateSpecialization")
                .WithSummary("Creates a new specialization")
                .Produces<SpecializationDTO>(StatusCodes.Status201Created)
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status409Conflict);
            group.MapPut("/{id:int:min(1)}", Update).AddEndpointFilter<ValidationFilter<CreateSpecializationDTO>>()
                .RequireAuthorization(Policies.AdminOnly)
                .ProducesProblem(StatusCodes.Status409Conflict);
            group.MapDelete("/{id:int:min(1)}", Delete).RequireAuthorization(Policies.AdminOnly);

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

            return specialization is null
                ? SpecializationErrors.NotFound(id).ToProblem()
                : Results.Ok(specialization);

        }

        private static async Task<IResult> Create(CreateSpecializationDTO dto, AppDbContext context)
        {
            var name = dto.Name.Trim();

            if (await context.Specializations.AnyAsync(s => s.Name == name))
                return SpecializationErrors.NameAlreadyExists.ToProblem();

            var specialization = Specialization.Create(name);

            context.Specializations.Add(specialization);
            await context.SaveChangesAsync();

            var result = new SpecializationDTO { Id = specialization.Id, Name = specialization.Name };

            return Results.Created($"/api/specializations/{specialization.Id}", result);

        }


        private static async Task<IResult> Update(int id,CreateSpecializationDTO dto,AppDbContext  context)
        {
            var specialization = await context.Specializations.FindAsync(id);
            if (specialization is null)
                return SpecializationErrors.NotFound(id).ToProblem();

            var name = dto.Name.Trim();

            if (await context.Specializations.AnyAsync(s => s.Id != id && s.Name == name))
                return SpecializationErrors.NameAlreadyExists.ToProblem();

            specialization.Rename(name);
            await context.SaveChangesAsync();

            return Results.NoContent();
        }

        private static async Task<IResult> Delete(int id ,AppDbContext context)
        {            
            var specialization = await context.Specializations.FindAsync(id);
            if (specialization is null)
                return Results.NotFound();

            if (await context.Doctors.AnyAsync(d => d.SpecializationId == id))
                    return SpecializationErrors.HasDoctors.ToProblem();

            context.Specializations.Remove(specialization);
            await context.SaveChangesAsync();

            return Results.NoContent();
        }


    }
}
