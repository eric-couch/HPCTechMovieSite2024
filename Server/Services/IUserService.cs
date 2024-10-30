using HPCTechMovieSite2024.Shared;
using Syncfusion.Blazor.Gantt.Internal;

namespace HPCTechMovieSite2024.Server.Services;

public interface IUserService
{
    Task<UserDto>? GetMovies(string userName);
    Task<Movie>? GetMovie(string imdbId, string userName);
    Task<bool> AddMovie(Movie movie, string userName);
    Task<List<MovieStatistic>> GetTopMovies(int countOfMovies);
    Task<List<UserEditDto>> GetUsers();
    Task<bool> UpdateRating(MovieUpdateRating rating);
    Task<bool> UpdateMovie(Movie movie);
    Task<bool> ToggleAdmin(string userId);
    Task<bool> UpdateUser(UserEditDto user);


    Task<bool> ToggleEmailConfirmed(string userId);
}
