using HPCTechMovieSite2024.Server.Controllers;
using HPCTechMovieSite2024.Server.Data;
using HPCTechMovieSite2024.Server.Models;
using HPCTechMovieSite2024.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System.Net;
using System.Net.Http.Json;

namespace HPCTechMovieSite2024.Server.Services;


public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string OMDBUrl = "https://www.omdbapi.com/?";
    private readonly string apiKey = "apikey=86c39163";

    public UserService(     ApplicationDbContext context, 
                            UserManager<ApplicationUser> userManager,
                            ILogger<UserService> logger,
                            HttpClient httpClient)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
        _httpClient = httpClient;
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

            var missingMovies = (from m in movies.FavoriteMovies
                                 where !_context.OMDBMovies.Any(omdb => omdb.imdbID == m.imdbId)
                                 select m).ToList();

            foreach (var movie in missingMovies)
            {
                OMDBMovie omdbMovie = await _httpClient.GetFromJsonAsync<OMDBMovie>($"{OMDBUrl}{apiKey}&i={movie.imdbId}");
                if (omdbMovie is not null)
                {
                    await _context.Rating.AddRangeAsync(omdbMovie.Ratings);
                    await _context.OMDBMovies.AddAsync(omdbMovie);
                    await _context.SaveChangesAsync();
                }
            }

            // grab all of the omdb moves for the user and add it to the user dto
            var omdbMoviesForUser = await (from omdb in _context.OMDBMovies
                                           join m in _context.Movies on omdb.imdbID equals m.imdbId
                                           join u in _context.Users on m.ApplicationUserId equals u.Id
                                           where m.ApplicationUserId == user.Id
                                           select omdb)
                                           .Include(omdb => omdb.Ratings)
                                           .ToListAsync();

            movies.OMDBMovies = omdbMoviesForUser;

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
