using Appointments.Api.Dtos;
using Appointments.Api.Exceptions;
using Appointments.Api.Repositories;

namespace Appointments.Api.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await userRepository.GetAllAsync();
        return users.Select(u => new UserDto(
            u.Id,
            u.Name,
            u.Email,
            u.CreatedAt,
            u.UpdatedAt,
            u.DeletedAt
        ));
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
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

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var emailExists = await userRepository.ExistsByEmailAsync(dto.Email);
        if (emailExists)
            throw new ConflictException("Email already in use.");

        return await userRepository.AddAsync(dto);
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user is null)
            return null;

        // just pass id and dto, repository handles the rest
        return await userRepository.UpdateAsync(id, dto);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user is null)
            throw new NotFoundException("User not found.");

        await userRepository.DeleteAsync(id);
    }
}
