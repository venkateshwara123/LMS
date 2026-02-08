using MemberServices.Data;
using MemberServices.DTO;
using MemberServices.Interfaces;
using MemberServices.Models;
using MemberServices.Services;
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
        _context.User.Add(user);
        await _context.SaveChangesAsync();
        return Ok("User Successfully Registered");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Users request)
    {
        var user = await _context.User
            .FirstOrDefaultAsync(x =>
                x.Username == request.Username &&
                x.PasswordHash == request.PasswordHash);

        if (user == null)
            return Unauthorized();

        var token = _jwt.GenerateToken(user);
        return Ok(new { token });
    }
    //public AuthController(AppDbContext context, IJwtService jwt)
    //{
    //    _context = context;
    //    _jwt = jwt;
    //}

    //[HttpPost("register",Name ="RegisterNewUser")]
    //[ProducesResponseType(StatusCodes.Status200OK)] 
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]   
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<IActionResult> Register(Users user)
    //{
    //    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
    //    _context.User.Add(user);
    //    await _context.SaveChangesAsync();
    //    return Ok($"User Registered successfully.");
    //}

    //[HttpPost("login",Name ="GenerateJWTToken")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //public async Task<IActionResult> Login(Users request)
    //{
    //    var member = await _context.User.FirstOrDefaultAsync(x => x.Username == request.Username);

    //    if (member == null ||
    //        !BCrypt.Net.BCrypt.Verify(request.PasswordHash, member.PasswordHash))
    //        return Unauthorized();

    //    return Ok(_jwt.GenerateToken(member));
    //}

}
