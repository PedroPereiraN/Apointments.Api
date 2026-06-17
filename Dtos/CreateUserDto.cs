using System.ComponentModel.DataAnnotations;

namespace Appointments.Api.Dtos;

public record CreateUserDto(
    [Required] [StringLength(100)] string Name,
    [Required] [StringLength(150)] string Email
);
