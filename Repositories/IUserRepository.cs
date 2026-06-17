using Appointments.Api.Dtos;

namespace Appointments.Api.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto> AddAsync(CreateUserDto dto);
    Task<UserDto> UpdateAsync(int id, UpdateUserDto dto);
    Task DeleteAsync(int id);
    Task<bool> ExistsByEmailAsync(string email);
}
