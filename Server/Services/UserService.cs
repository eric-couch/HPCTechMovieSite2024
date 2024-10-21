using HPCTechMovieSite2024.Server.Data;
using HPCTechMovieSite2024.Server.Models;
using HPCTechMovieSite2024.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HPCTechMovieSite2024.Server.Services;


public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<UserDto>? GetMovies(string userName)
    {
        //ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _userManager.FindByNameAsync(userName);

        UserDto? movies = await _context.Users.Include(u => u.FavoriteMovies)
                                .Select(u => new UserDto
                                {
                                    Id = u.Id,
                                    UserName = u.UserName,
                                    FavoriteMovies = u.FavoriteMovies
                                }).FirstOrDefaultAsync(u => u.Id == user.Id);
        return movies;
    }
}
