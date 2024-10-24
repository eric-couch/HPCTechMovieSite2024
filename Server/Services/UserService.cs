using HPCTechMovieSite2024.Server.Controllers;
using HPCTechMovieSite2024.Server.Data;
using HPCTechMovieSite2024.Server.Models;
using HPCTechMovieSite2024.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;

namespace HPCTechMovieSite2024.Server.Services;


public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserService> _logger;

    public UserService(     ApplicationDbContext context, 
                            UserManager<ApplicationUser> userManager,
                            ILogger<UserService> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<UserDto>? GetMovies(string userName)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(userName);

            UserDto? movies = await _context.Users.Include(u => u.FavoriteMovies)
                                    .Select(u => new UserDto
                                    {
                                        Id = u.Id,
                                        UserName = u.UserName,
                                        FavoriteMovies = u.FavoriteMovies
                                    }).FirstOrDefaultAsync(u => u.Id == user.Id);

            _logger.LogInformation("User {userName} retrieved {Count} movies. Logged at {Placeholder:MMMM dd, yyyy}", userName, movies.FavoriteMovies.Count, DateTimeOffset.UtcNow);
            return movies;
        } catch (Exception ex)
        {
            _logger.LogError("User {userName} encountered error {ex.Message}. Logged at {Placeholder:MMMM dd, yyyy}", userName, ex.Message, DateTimeOffset.UtcNow);
            return null;
        }
    }

    public async Task<List<UserEditDto>> GetUsers()
    {
        var users = (from u in _context.Users
                     let query = (from ur in _context.Set<IdentityUserRole<string>>()
                                  where ur.UserId.Equals(u.Id)
                                  join r in _context.Roles on ur.RoleId equals r.Id
                                  select r.Name).ToList()
                     select new UserEditDto
                     {
                         Id = u.Id,
                         UserName = u.UserName,
                         FirstName = u.FirstName,
                         LastName = u.LastName,
                         Email = u.Email,
                         EmailConfirmed = u.EmailConfirmed,
                         Admin = query.Contains("Admin")
                     }).ToList();
        _logger.LogInformation("Retrieving All Users.");
        return users;
    }

    public async Task<bool> UpdateUser(UserEditDto user)
    {
        var userToUpdate = await _userManager.FindByNameAsync(user.Email);
        if (userToUpdate is null) {
            return false;
        }
        userToUpdate.FirstName = user.FirstName;
        userToUpdate.LastName = user.LastName;
        userToUpdate.Email = user.Email;
        userToUpdate.EmailConfirmed = user.EmailConfirmed;
        await _userManager.UpdateAsync(userToUpdate);
        return true;
    }

    public async Task<bool> ToggleEmailConfirmed(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }
        user.EmailConfirmed = !user.EmailConfirmed;
        await _userManager.UpdateAsync(user);
        return true;
    }

    public async Task<bool> ToggleAdmin(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Admin"))
        {
            //await _userManager.RemoveFromRoleAsync(user, "Admin");
            var admin = await (from ur in _context.UserRoles
                                join r in _context.Roles on ur.RoleId equals r.Id
                                where ur.UserId == userId
                                where r.Name == "Admin"
                                select ur).FirstOrDefaultAsync();
            if (admin is not null)
            {
                _context.UserRoles.Remove(admin);
                _context.SaveChanges();
            }
            
        } else
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }
        return true;
    }
}
