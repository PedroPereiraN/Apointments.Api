namespace Appointments.Api.Dtos;

public record AppointmentDto(
    int Id,
    string Description,
    DateTimeOffset ScheduledAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt,
    int UserId
);
