namespace Appointments.Api.Dtos;

public record UserDto(
    int Id,
    string Name,
    string Email,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt
);
