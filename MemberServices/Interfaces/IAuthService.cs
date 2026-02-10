using MemberServices.DTOs.Request;
using MemberServices.DTOs.Response;

namespace MemberServices.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserResponse> RegisterAsync(RegisterRequest request);
        Task<UserResponse> GetUserByIdAsync(int id);
    }
}
