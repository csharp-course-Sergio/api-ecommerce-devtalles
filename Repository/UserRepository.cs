using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository;

public class UserRepository(
    ApplicationDbContext db,
    IConfiguration configuration,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IMapper mapper) : IUserRepository
{
    public readonly ApplicationDbContext _db = db;
    private readonly string? secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IMapper _mapper = mapper;

    public User? GetUser(int id)
    {
        return _db.Users.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<User> GetUsers()
    {
        return [.. _db.Users.OrderBy(u => u.Username)];
    }

    public bool IsUniqueUser(string username)
    {
        return !_db.Users.Any(u => u.Username.ToLower().Trim() == username.ToLower().Trim());
    }

    public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.Username))
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Username is required"
            };
        }

        var user = await _db.ApplicationUsers.FirstOrDefaultAsync<ApplicationUser>(u => u.UserName != null && u.UserName.ToLower().Trim() == userLoginDto.Username.ToLower().Trim());

        if (user == null)
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Username not found"
            };
        }

        if (userLoginDto.Password == null)
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Password is required"
            };
        }

        bool isValidPassword = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);

        if (!isValidPassword)
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Credentials are not valid"
            };
        }

        var handlerToken = new JwtSecurityTokenHandler();
        if (string.IsNullOrEmpty(secretKey)) throw new InvalidOperationException("Secret key is not configured.");

        var roles = await _userManager.GetRolesAsync(user);
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? string.Empty),
            ]
            ),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = handlerToken.CreateToken(tokenDescriptor);

        return new UserLoginResponseDto()
        {
            Token = handlerToken.WriteToken(token),
            User = _mapper.Map<UserDataDto>(user),
            Message = "Login successful"
        };
    }

    public async Task<UserDataDto> Register(CreateUserDto createUserDto)
    {
        if (string.IsNullOrEmpty(createUserDto.Username))
        {
            throw new ArgumentNullException("Username is required");
        }

        if (createUserDto.Password == null)
        {
            throw new ArgumentNullException("Password is required");
        }

        var user = new ApplicationUser
        {
            UserName = createUserDto.Username,
            Email = createUserDto.Username,
            NormalizedEmail = createUserDto.Username.ToUpper(),
            Name = createUserDto.Name,
        };

        var result = await _userManager.CreateAsync(user, createUserDto.Password);

        if (result.Succeeded)
        {
            var userRole = createUserDto.Role ?? "User";
            var roleExists = await _roleManager.RoleExistsAsync(userRole);
            if (!roleExists)
            {
                var identityRole = new IdentityRole(userRole);
                await _roleManager.CreateAsync(identityRole);
            }
            await _userManager.AddToRoleAsync(user, userRole);

            var createdUser = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == createUserDto.Username);
            return _mapper.Map<UserDataDto>(createdUser);
        }
        throw new ApplicationException("User registration failed");
    }
}
