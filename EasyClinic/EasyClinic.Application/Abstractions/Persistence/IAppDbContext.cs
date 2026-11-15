using EasyClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyClinic.Application.Abstractions.Persistence;

public interface IAppDbContext
{
    DbSet<Specialization> Specializations { get; }
    DbSet<Doctor> Doctors { get; }
    DbSet<Patient> Patients { get; }
    DbSet<MedicalService> MedicalServices { get; }
    DbSet<DoctorService> DoctorServices { get; }
    DbSet<AppointmentStatus> AppointmentStatuses { get; }
    DbSet<Appointment> Appointments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}