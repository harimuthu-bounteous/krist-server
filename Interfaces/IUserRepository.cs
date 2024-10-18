using krist_server.DTO.AuthDTOs;
using krist_server.Models;

namespace krist_server.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(string id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> CreateUserAsync(User user, string UserId);
    }
}