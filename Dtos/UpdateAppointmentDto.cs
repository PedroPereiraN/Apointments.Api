using System.ComponentModel.DataAnnotations;

namespace Appointments.Api.Dtos;

public record UpdateAppointmentDto(
    [Required] [StringLength(500)] string Description,
    [Required] DateTimeOffset ScheduledAt
);
