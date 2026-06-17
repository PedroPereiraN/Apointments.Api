using Appointments.Api.Dtos;
using Appointments.Api.Exceptions;
using Appointments.Api.Repositories;

namespace Appointments.Api.Services;

public class AppointmentService(IAppointmentRepository appointmentRepository) : IAppointmentService
{
    public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
    {
        var appointments = await appointmentRepository.GetAllAsync();
        return appointments.Select(a => new AppointmentDto(
            a.Id,
            a.Description,
            a.ScheduledAt,
            a.CreatedAt,
            a.UpdatedAt,
            a.DeletedAt,
            a.UserId
        ));
    }

    public async Task<AppointmentDto?> GetByIdAsync(int id)
    {
        var appointment = await appointmentRepository.GetByIdAsync(id);
        if (appointment is null)
            return null;
        return new AppointmentDto(
            appointment.Id,
            appointment.Description,
            appointment.ScheduledAt,
            appointment.CreatedAt,
            appointment.UpdatedAt,
            appointment.DeletedAt,
            appointment.UserId
        );
    }

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto)
    {
        var scheduledAtExists = await appointmentRepository.ExistsByScheduledAtAsync(
            dto.UserId,
            dto.ScheduledAt
        );
        if (scheduledAtExists)
            throw new ConflictException("Scheduled date already in use.");

        return await appointmentRepository.AddAsync(dto);
    }

    public async Task<AppointmentDto?> UpdateAsync(int id, UpdateAppointmentDto dto)
    {
        var appointment = await appointmentRepository.GetByIdAsync(id);
        if (appointment is null)
            return null;

        var scheduledAtExists = await appointmentRepository.ExistsByScheduledAtAsync(
            appointment.UserId,
            dto.ScheduledAt
        );
        if (scheduledAtExists)
            throw new ConflictException("Scheduled date already in use.");

        return await appointmentRepository.UpdateAsync(id, dto);
    }

    public async Task DeleteAsync(int id)
    {
        var appointment = await appointmentRepository.GetByIdAsync(id);
        if (appointment is null)
            throw new NotFoundException("Appointment not found.");

        await appointmentRepository.DeleteAsync(id);
    }
}
