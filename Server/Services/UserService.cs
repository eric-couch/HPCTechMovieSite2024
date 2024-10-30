using HPCTechMovieSite2024.Server.Controllers;
using HPCTechMovieSite2024.Server.Data;
using HPCTechMovieSite2024.Server.Models;
using HPCTechMovieSite2024.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using Syncfusion.Blazor.Inputs;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices;

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

            // Query Movies for the user, including the related movies with user review and user rating
            var userWithFavorites = await _context.Users.Include(u => u.FavoriteMovies)
                                                        .FirstOrDefaultAsync(u => u.Id == user.Id);

            // get the omdbids as a list
            var favoriteOmdbIds = userWithFavorites.FavoriteMovies.Select(m => m.imdbId).ToList();

            // Retrieve the omdb movies that match the favorites
            var omdbMoviesForUser = await _context.OMDBMovies
                .Where(omdb => favoriteOmdbIds.Contains(omdb.imdbID))
                .Include(omdb => omdb.Ratings)
                .ToListAsync();

            // create a dictionary for lookup of userRatings by omdbid
            var userRatingsByOmdbId = userWithFavorites.FavoriteMovies.ToDictionary(m => m.imdbId, m => m.userRating);

            // assign the user rating to the omdb movie
            foreach (var omdbMovie in omdbMoviesForUser)
            {
                if (userRatingsByOmdbId.TryGetValue(omdbMovie.imdbID, out var userRating))
                {
                    omdbMovie.Ratings.Add(new Rating { Source = "User", Value = userRating.ToString() });
                }
            }

            movies.OMDBMovies = omdbMoviesForUser;

            _logger.LogInformation("User {userName} retrieved {Count} movies. Logged at {Placeholder:MMMM dd, yyyy}", userName, movies.FavoriteMovies.Count, DateTimeOffset.UtcNow);
            return movies;
        } catch (Exception ex)
        {
            _logger.LogError("User {userName} encountered error {ex.Message}. Logged at {Placeholder:MMMM dd, yyyy}", userName, ex.Message, DateTimeOffset.UtcNow);
            return null;
        }
    }

    public async Task<Movie>? GetMovie(string imdbId, string userName)
    {
        return await (from m in _context.Movies
                              join u in _context.Users on m.ApplicationUserId equals u.Id
                              where m.imdbId == imdbId
                              where u.UserName == userName
                              select m).FirstOrDefaultAsync();
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

    public async Task<List<MovieStatistic>> GetTopMovies(int countOfMovies)
    {
        // join movies and omdbmovies
        var joinedData = from m in _context.Movies
                         join omdb in _context.OMDBMovies on m.imdbId equals omdb.imdbID
                         select new { m, omdb };

        // group by imdbid and avg the ratings
        var groupedData = from data in joinedData
                          group data by data.omdb into g
                          select new MovieStatistic
                          {
                              Title = g.Key.Title,
                              AverageRating = g.Average(x => x.m.userRating),
                              NumberOfRatings = g.Count()
                          };

        var topMovies = groupedData.OrderByDescending(m => m.AverageRating).Take(countOfMovies).ToList();

        return topMovies;
    }

    public async Task<bool> AddMovie(Movie movie, string userName)
    {
        //var userToUpdate = await _userManager.FindByNameAsync(userName);

        var user = await (from u in _context.Users 
                          where u.UserName == userName
                          select u).FirstOrDefaultAsync();



        _context.Movies.Add(new Movie() { ApplicationUserId = user.Id, imdbId = movie.imdbId, userRating = movie.userRating, userReview = movie.userReview });
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> UpdateMovie(Movie movie)
    {
        Movie? movieToUpdate = _context.Movies.Find(movie.Id);

        if (movieToUpdate is null) {
            return false;
        }

        movieToUpdate.userReview = movie.userReview;
        movieToUpdate.userRating = movie.userRating;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateRating(MovieUpdateRating rating)
    {
        var userToUpdate = await _userManager.FindByNameAsync(rating.UserName);
        if (userToUpdate is null)
        {
            return false;
        }

        Movie movieToUpdate = (from m in _context.Movies
                               where m.imdbId == rating.ImdbId
                               where m.ApplicationUserId == userToUpdate.Id
                               select m).FirstOrDefault();

        movieToUpdate.userRating = rating.Rating;
        await _context.SaveChangesAsync();
        return true;
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
