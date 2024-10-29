using HPCTechMovieSite2024.Shared;
using HPCTechMovieSite2024.Shared.Wrapper;
using Syncfusion.Blazor.Gantt.Internal;

namespace HPCTechMovieSite2024.Client.HttpRepo;

public interface IUserMoviesHttpRepo
{
    // get movies
    Task<DataResponse<List<OMDBMovie>>> GetMovies(string userName);
    Task<DataResponse<Movie>> GetMovie(string imdbId, string userName);
    // add movie
    Task<Response> AddMovie(string imdbId);
    Task<Response> UpdateMovie(Movie movie);
    // remove movie
    Task<Response> RemoveMovie(string imdbId);

    Task<DataResponse<List<UserEditDto>>> GetUsers();
    Task<Response> UpdateRating(MovieUpdateRating rating);
    Task<Response> UpdateUser(UserEditDto user);
    Task<bool> EmailConfirmUser(string userId);

    Task<bool> ToggleAdmin(string userId);
}
