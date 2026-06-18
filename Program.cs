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

app.MapControllers();
app.Run();
