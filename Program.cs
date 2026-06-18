using Appointments.Api.Data;
using Appointments.Api.Repositories;
using Appointments.Api.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var connString = "Data Source=Appointments.db";

builder.Services.AddSqlite<AppointmentsStoreContext>(connString);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddHostedService<AppointmentReminderJob>();
builder.Services.AddControllers();

var app = builder.Build();

var smtp = app.Configuration.GetSection("Smtp");
var smtpConfigured = new[] { smtp["Host"], smtp["Username"], smtp["Password"], smtp["From"] }
    .All(v => !string.IsNullOrEmpty(v));

if (!smtpConfigured)
    app.Logger.LogWarning("SMTP is not configured. The reminder cronjob will not run. Set Smtp__Host, Smtp__Username, Smtp__Password and Smtp__From in your .env file.");

app.MapControllers();
app.Run();
