using Appointments.Api.Dtos;
using Appointments.Api.Repositories;
using MailKit.Net.Smtp;
using MimeKit;

namespace Appointments.Api.Services;

public class ReminderService(
    IAppointmentRepository appointmentRepository,
    IConfiguration configuration,
    ILogger<ReminderService> logger
) : IReminderService
{
    public async Task SendDayRemindersAsync(DateTimeOffset date)
    {
        var appointments = await appointmentRepository.GetForDayAsync(date);
        var byUser = appointments.GroupBy(a => a.UserEmail);

        foreach (var group in byUser)
        {
            var user = group.First();
            var lines = group.Select(a => $"- {a.Description} at {a.ScheduledAt:t}");
            var body = $"Hi {user.UserName}, here are your appointments for today:\n\n{string.Join("\n", lines)}";

            await SendEmail(user.UserName, user.UserEmail, "Your appointments for today", body);
            logger.LogInformation("Day reminder sent to {Email} ({Count} appointment(s))", user.UserEmail, group.Count());
        }
    }

    public async Task SendUpcomingRemindersAsync(DateTimeOffset from, DateTimeOffset to)
    {
        var appointments = await appointmentRepository.GetUpcomingAsync(from, to);
        var byUser = appointments.GroupBy(a => a.UserEmail);

        foreach (var group in byUser)
        {
            var user = group.First();
            var lines = group.Select(a => $"- {a.Description} at {a.ScheduledAt:t}");
            var body = $"Hi {user.UserName}, you have upcoming appointments in the next 3 hours:\n\n{string.Join("\n", lines)}";

            await SendEmail(user.UserName, user.UserEmail, "Upcoming appointments reminder", body);
            logger.LogInformation("Upcoming reminder sent to {Email} ({Count} appointment(s))", user.UserEmail, group.Count());
        }
    }

    private async Task SendEmail(string userName, string userEmail, string subject, string body)
    {
        var smtp = configuration.GetSection("Smtp");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Appointments App", smtp["From"]));
        message.To.Add(new MailboxAddress(userName, userEmail));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(smtp["Host"], int.Parse(smtp["Port"]!));
        await client.AuthenticateAsync(smtp["Username"], smtp["Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
