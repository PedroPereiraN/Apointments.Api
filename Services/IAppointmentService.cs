using Appointments.Api.Dtos;

namespace Appointments.Api.Services;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetAllAsync();
    Task<AppointmentDto?> GetByIdAsync(int id);
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto);
    Task<AppointmentDto?> UpdateAsync(int id, UpdateAppointmentDto dto);
    Task DeleteAsync(int id);
}
