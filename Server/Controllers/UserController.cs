using HPCTechMovieSite2024.Server.Data;
using HPCTechMovieSite2024.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HPCTechMovieSite2024.Shared;

namespace HPCTechMovieSite2024.Server.Controllers;

public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Route("api/User")]
    public async Task<UserDto> GetMovies()
    {
        var movies = await _context.Users.Include(u => u.FavoriteMovies)
                                .Select(u => new UserDto
                                {
                                    Id = u.Id,
                                    UserName = u.UserName,
                                    FavoriteMovies = u.FavoriteMovies
                                }).FirstOrDefaultAsync();

        return movies;
    }
}
