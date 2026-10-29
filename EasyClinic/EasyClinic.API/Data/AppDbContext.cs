using EasyClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace EasyClinic.API.Data
{
    public class AppDbContext : DbContext
    {        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalService> MedicalServices { get; set; }
        public DbSet<DoctorService> DoctorServices { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MedicalService>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Appointment>()
                .Property(a => a.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.AppointmentStatus)
                .WithMany()
                .HasForeignKey(a => a.AppointmentStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.DoctorService)
                .WithMany()
                .HasForeignKey(a => a.DoctorServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DoctorService>()
                .HasIndex(ds => new { ds.DoctorId, ds.MedicalServiceId })
                .IsUnique();
        }
    }
}
