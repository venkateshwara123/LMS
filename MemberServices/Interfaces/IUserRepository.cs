using MemberServices.Models;

namespace MemberServices.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(int id);
        Task UpdateAsync(User user);
        Task<bool> UserExistsAsync(string email);
    }
}
