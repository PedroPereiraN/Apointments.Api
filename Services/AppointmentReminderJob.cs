namespace Appointments.Api.Services;

public class AppointmentReminderJob(
    IServiceScopeFactory scopeFactory,
    ILogger<AppointmentReminderJob> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var now = DateTimeOffset.Now;
        var nextHour = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, now.Offset).AddHours(1);
        var delayUntilNextHour = nextHour - now;

        logger.LogInformation("Job will start at {NextHour} (in {Delay})", nextHour, delayUntilNextHour);
        await Task.Delay(delayUntilNextHour, stoppingToken);

        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            now = DateTimeOffset.Now;
            using var scope = scopeFactory.CreateScope();
            var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();

            if (now.Hour == 0)
            {
                logger.LogInformation("Running day reminders at {Time}", now);
                await reminderService.SendDayRemindersAsync(now);
            }

            logger.LogInformation("Running upcoming reminders at {Time}", now);
            await reminderService.SendUpcomingRemindersAsync(now, now.AddHours(3));
        }
    }
}
