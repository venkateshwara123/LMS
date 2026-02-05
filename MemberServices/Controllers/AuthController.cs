using MemberServices.Data;
using MemberServices.DTO;
using MemberServices.Interfaces;
using MemberServices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwt;


    public AuthController(AppDbContext context, IJwtService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(Users user)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        _context.User.Add(user);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Users request)
    {
        var member = await _context.User.FirstOrDefaultAsync(x => x.Username == request.Username);

        if (member == null ||
            !BCrypt.Net.BCrypt.Verify(request.PasswordHash, member.PasswordHash))
            return Unauthorized();

        return Ok(_jwt.GenerateToken(member));
    }

}
