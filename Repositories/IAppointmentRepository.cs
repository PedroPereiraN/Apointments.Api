using Appointments.Api.Dtos;

namespace Appointments.Api.Repositories;

public interface IAppointmentRepository
{
    Task<IEnumerable<AppointmentDto>> GetAllAsync();
    Task<AppointmentDto?> GetByIdAsync(int id);
    Task<AppointmentDto> AddAsync(CreateAppointmentDto dto);
    Task<AppointmentDto> UpdateAsync(int id, UpdateAppointmentDto dto);
    Task DeleteAsync(int id);
    Task<bool> ExistsByScheduledAtAsync(int userId, DateTimeOffset scheduledAt);
    Task<IEnumerable<AppointmentReminderDto>> GetForDayAsync(DateTimeOffset date);
    Task<IEnumerable<AppointmentReminderDto>> GetUpcomingAsync(DateTimeOffset from, DateTimeOffset to);
}
