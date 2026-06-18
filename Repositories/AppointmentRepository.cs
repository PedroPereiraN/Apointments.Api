using Appointments.Api.Data;
using Appointments.Api.Dtos;
using Appointments.Api.Exceptions;
using Appointments.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Api.Repositories;

public class AppointmentRepository(AppointmentsStoreContext context) : IAppointmentRepository
{
    public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
    {
        return await context
            .Appointments.Select(a => new AppointmentDto(
                a.Id,
                a.Description,
                a.ScheduledAt,
                a.CreatedAt,
                a.UpdatedAt,
                a.DeletedAt,
                a.UserId
            ))
            .ToListAsync();
    }

    public async Task<AppointmentDto?> GetByIdAsync(int id)
    {
        var appointment = await context.Appointments.FindAsync(id);
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

    public async Task<AppointmentDto> AddAsync(CreateAppointmentDto dto)
    {
        var appointment = new Appointment
        {
            UserId = dto.UserId,
            Description = dto.Description,
            ScheduledAt = dto.ScheduledAt,
            CreatedAt = DateTime.UtcNow,
        };

        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

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

    public async Task<AppointmentDto> UpdateAsync(int id, UpdateAppointmentDto dto)
    {
        var appointment = await context.Appointments.FindAsync(id);
        if (appointment is null)
            throw new NotFoundException("Appointment not found.");

        appointment.Description = dto.Description;
        appointment.ScheduledAt = dto.ScheduledAt;
        appointment.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

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

    public async Task DeleteAsync(int id)
    {
        var appointment = await context.Appointments.FindAsync(id);
        if (appointment is null)
            throw new NotFoundException("Appointment not found.");

        context.Appointments.Remove(appointment);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AppointmentReminderDto>> GetForDayAsync(DateTimeOffset date)
    {
        var appointments = await context.Appointments
            .Include(a => a.User)
            .ToListAsync();

        return appointments
            .Where(a => a.ScheduledAt.Date == date.Date)
            .Select(a => new AppointmentReminderDto(a.Description, a.ScheduledAt, a.User.Name, a.User.Email));
    }

    public async Task<IEnumerable<AppointmentReminderDto>> GetUpcomingAsync(DateTimeOffset from, DateTimeOffset to)
    {
        var appointments = await context.Appointments
            .Include(a => a.User)
            .ToListAsync();

        return appointments
            .Where(a => a.ScheduledAt >= from && a.ScheduledAt <= to)
            .Select(a => new AppointmentReminderDto(a.Description, a.ScheduledAt, a.User.Name, a.User.Email));
    }

    public async Task<bool> ExistsByScheduledAtAsync(int userId, DateTimeOffset scheduledAt)
    {
        return await context.Appointments.AnyAsync(a =>
            a.UserId == userId && a.ScheduledAt == scheduledAt
        );
    }
}
