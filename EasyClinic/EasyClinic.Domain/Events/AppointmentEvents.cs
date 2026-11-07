using EasyClinic.Domain.Abstractions;
using EasyClinic.Domain.Entities;

namespace EasyClinic.Domain.Events;

public sealed record AppointmentCreatedEvent(Appointment Appointment) : IDomainEvent;

public sealed record AppointmentRescheduledEvent(Appointment Appointment, DateTimeOffset OldDate) : IDomainEvent;

public sealed record AppointmentStatusChangedEvent(Appointment Appointment, int OldStatusId) : IDomainEvent;