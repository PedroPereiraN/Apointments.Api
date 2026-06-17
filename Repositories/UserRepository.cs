using Appointments.Api.Data;
using Appointments.Api.Dtos;
using Appointments.Api.Exceptions;
using Appointments.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Api.Repositories;

public class UserRepository(AppointmentsStoreContext context) : IUserRepository
{
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        return await context
            .Users.Select(u => new UserDto(
                u.Id,
                u.Name,
                u.Email,
                u.CreatedAt,
                u.UpdatedAt,
                u.DeletedAt
            ))
            .ToListAsync();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user is null)
            return null;
        return new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt,
            user.DeletedAt
        );
    }

    public async Task<UserDto> AddAsync(CreateUserDto dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow,
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt,
            user.DeletedAt
        );
    }

    public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await context.Users.FindAsync(id);
        if (user is null)
            throw new NotFoundException("User not found.");

        user.Name = dto.Name;
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt,
            user.DeletedAt
        );
    }

    public async Task DeleteAsync(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user is null)
            throw new NotFoundException("User not found.");

        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await context.Users.AnyAsync(u => u.Email == email);
    }
}
