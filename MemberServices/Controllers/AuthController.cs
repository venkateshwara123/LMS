using MemberServices.Data;
using MemberServices.DTOs.Request;
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
    
    private readonly IAuthService _auth;

    public AuthController( IAuthService auth)
    {
        _auth = auth;
    }
    [HttpPost("register", Name = "RegisterNewUser")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _auth.RegisterAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _auth.LoginAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }

    }
    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var response = await _auth.GetUserByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
