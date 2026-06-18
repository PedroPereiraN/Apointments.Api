using Appointments.Api.Repositories;
using MailKit.Net.Smtp;
using MimeKit;

namespace Appointments.Api.Services;

public class AppointmentReminderJob(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<AppointmentReminderJob> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await DoWork();
        }
    }

    private async Task DoWork()
    {
        logger.LogInformation("AppointmentReminderJob running at {Time}", DateTimeOffset.UtcNow);

        using var scope = scopeFactory.CreateScope();
        var appointmentRepository =
            scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();

        var now = DateTimeOffset.UtcNow;
        var upcoming = await appointmentRepository.GetUpcomingAsync(now, now.AddHours(1));

        logger.LogInformation("Found {Count} upcoming appointment(s)", upcoming.Count());

        var smtp = configuration.GetSection("Smtp");
        var host = smtp["Host"]!;
        var port = int.Parse(smtp["Port"]!);
        var username = smtp["Username"]!;
        var password = smtp["Password"]!;
        var from = smtp["From"]!;

        foreach (var appointment in upcoming)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Appointments App", from));
            message.To.Add(new MailboxAddress(appointment.UserName, appointment.UserEmail));
            message.Subject = "Appointment Reminder";
            message.Body = new TextPart("plain")
            {
                Text =
                    $"Hi {appointment.UserName}, this is a reminder that your appointment \"{appointment.Description}\" is scheduled for {appointment.ScheduledAt:f}.",
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            logger.LogInformation(
                "Reminder sent to {Email} for appointment \"{Description}\"",
                appointment.UserEmail,
                appointment.Description
            );
        }
    }
}
