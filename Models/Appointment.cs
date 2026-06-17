namespace Appointments.Api.Models;

public class Appointment
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public DateTimeOffset ScheduledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Foreign key
    public int UserId { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}
