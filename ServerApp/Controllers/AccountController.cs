using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ServerApp.DTOs.Account;
using ServerApp.Models;
using ServerApp.Services;

namespace ServerApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AccountController(JwtService jwtService, SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _jwtService = jwtService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto req)
    {
        Log.Information("Incoming Register user");
        if (await CheckEmailExistsAsync(req.Email!))
        {
            return BadRequest($"An existing email address '{req.Email}' is already in use. Please try with a different email address.");
        }

        if (await CheckPhoneNumberExistsAsync(req.PhoneNumber!))
        {
            return BadRequest($"An phone number '{req.PhoneNumber}' is already in use. Please try with a different phone number.");
        }

        var userToAdd = new User
        {
            FirstName = req.FirstName!.ToLower(),
            LastName = req.LastName!.ToLower(),
            PhoneNumber = req.PhoneNumber!,
            UserName = req.Email!.ToLower(),
            Email = req.Email!.ToLower(),
            EmailConfirmed = true
        };
        var result = await _userManager.CreateAsync(userToAdd, req.Password!);
        if (!result.Succeeded)
        {
            Log.Warning("Register user failed");
            return BadRequest(result.Errors);
        }
        Log.Information("Outgoing Register user success");
        return Ok("Your Account created successfully, you can login");
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto req)
    {
        Log.Information("Incoming Login user");
        var user = await _userManager.FindByNameAsync(req.Username!);
        if (user == null)
        {
            Log.Warning("Invalid username or password");
            return Unauthorized("Invalid username or password");
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, req.Password!, false);
        if (!result.Succeeded)
        {
            Log.Warning("Invalid username or password");
            return Unauthorized("Invalid username or password");
        }
        
        Log.Information("Outgoing User logged in");
        return CreateApplicationUserDto(user);
    }
    
    private UserDto CreateApplicationUserDto(User user)
    {
        Log.Information("Creating new user {@user}", user);
        return new UserDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = _jwtService.GenerateToken(user)
        };
    }

    private async Task<bool> CheckEmailExistsAsync(string email)
    {
        return await _userManager.Users.AnyAsync(e => e.Email == email.ToLower());
    }

    private Task<bool> CheckPhoneNumberExistsAsync(string phoneNumber)
    {
        return _userManager.Users.AnyAsync(p => p.PhoneNumber == phoneNumber);
    }
}