using Appointments.Api.Data;
using Appointments.Api.Repositories;
using Appointments.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var connString = "Data Source=Appointments.db";

builder.Services.AddSqlite<AppointmentsStoreContext>(connString);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();
