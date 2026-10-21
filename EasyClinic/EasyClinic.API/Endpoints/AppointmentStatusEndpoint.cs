using EasyClinic.API.Auth;
using EasyClinic.API.Data;
using EasyClinic.API.DTOs.AppointmentStatus;
using EasyClinic.API.Models;
using M04.MinimalFluentValidation.Filters;
using Microsoft.EntityFrameworkCore;


namespace EasyClinic.API.Endpoints
{
    public static class AppointmentStatusEndpoint
    {
        public static void MapAppointmentStatusEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/appointmentStatus")
                            .WithTags("AppointmentStatus")
                            .RequireAuthorization();

            group.MapGet("/", GetAll);
            group.MapGet("/{id:int:min(1)}", GetById).WithName("GetAppointmentStatusById");
            group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateAppointmentStatusDTO>>().RequireAuthorization(Policies.AdminOnly);
            group.MapPut("/{id:int:min(1)}", Update).AddEndpointFilter<ValidationFilter<CreateAppointmentStatusDTO>>().RequireAuthorization(Policies.AdminOnly);
            group.MapDelete("/{id:int:min(1)}", Delete).RequireAuthorization(Policies.AdminOnly);

        }

        private static async Task<IResult> GetAll(AppDbContext context,CancellationToken cancellationToken)
        {
            var appointmentStatus = await context.AppointmentStatuses
            .Select(a => new AppointmentStatusDTO
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
            })
            .ToListAsync(cancellationToken);

            return Results.Ok(appointmentStatus);
        }

        private static async Task<IResult> GetById(int id, AppDbContext context,CancellationToken cancellationToken)
        {
            var appointmentStatus = await context.AppointmentStatuses
          .Where(a => a.Id == id)
          .Select(a => new AppointmentStatusDTO
          {
              Id = a.Id,
              Name = a.Name,
              Description = a.Description,
          }).FirstOrDefaultAsync(cancellationToken);

            if (appointmentStatus is null)
                return Results.NotFound();

            return Results.Ok(appointmentStatus);
        }

        private static async Task<IResult> Create(AppDbContext context,CreateAppointmentStatusDTO dto)
        {
            var appointmentStatus = new AppointmentStatus
            {
                Name = dto.Name,
                Description = dto.Description,
            };

            context.AppointmentStatuses.Add(appointmentStatus);
            await context.SaveChangesAsync();

            var result = new AppointmentStatusDTO
            {
                Id = appointmentStatus.Id,
                Name = appointmentStatus.Name,
                Description = appointmentStatus.Description
            };

            return Results.CreatedAtRoute("GetAppointmentStatusById", new {id =  appointmentStatus.Id}, result);
        }

        private static async Task<IResult> Update(int id,CreateAppointmentStatusDTO dto,AppDbContext context)
        {
            var appointmentStatus = await context.AppointmentStatuses.FindAsync(id);

            if (appointmentStatus is null)
                return Results.NotFound();

            appointmentStatus.Name = dto.Name;
            appointmentStatus.Description = dto.Description;

            await context.SaveChangesAsync();

            return Results.NoContent();
        }
        private static async Task<IResult> Delete(int id, AppDbContext context)
        {
            var appointmentStatus = await context.AppointmentStatuses.FindAsync(id);

            if (appointmentStatus is null)
                return  Results.NotFound();

            var hasAppointments = await context.Appointments.AnyAsync(a => a.AppointmentStatusId == id);
            if (hasAppointments)
                return Results.Problem(
                    detail: "Cannot delete this status because it has appointments linked to it.",
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Cannot Delete Specialization"); 

            context.AppointmentStatuses.Remove(appointmentStatus);
            await context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
