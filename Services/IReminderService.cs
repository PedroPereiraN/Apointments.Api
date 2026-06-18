namespace Appointments.Api.Services;

public interface IReminderService
{
    Task SendDayRemindersAsync(DateTimeOffset date);
    Task SendUpcomingRemindersAsync(DateTimeOffset from, DateTimeOffset to);
}
