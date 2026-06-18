namespace Appointments.Api.Dtos;

public record AppointmentReminderDto(
    string Description,
    DateTimeOffset ScheduledAt,
    string UserName,
    string UserEmail
);
