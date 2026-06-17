using Appointments.Api.Dtos;
using Appointments.Api.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("appointments")]
public class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var appointments = await appointmentService.GetAllAsync();
        return Ok(appointments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var appointment = await appointmentService.GetByIdAsync(id);
        if (appointment is null)
            return NotFound("Appointment not found.");
        return Ok(appointment);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppointmentDto dto)
    {
        var appointment = await appointmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateAppointmentDto dto)
    {
        var appointment = await appointmentService.UpdateAsync(id, dto);
        if (appointment is null)
            return NotFound("Appointment not found.");
        return Ok(appointment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await appointmentService.DeleteAsync(id);
        return NoContent();
    }
}
