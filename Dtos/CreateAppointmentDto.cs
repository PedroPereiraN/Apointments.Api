using System.ComponentModel.DataAnnotations;

namespace Appointments.Api.Dtos;

public record CreateAppointmentDto(
    [Required] [StringLength(500)] string Description,
    [Required] DateTimeOffset ScheduledAt,
    int UserId
);
