using MemberServices.Models;

namespace MemberServices.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Users users);
    }
}
