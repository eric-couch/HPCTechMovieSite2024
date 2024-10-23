using HPCTechMovieSite2024.Shared;
using HPCTechMovieSite2024.Shared.Wrapper;
using Syncfusion.Blazor.Gantt.Internal;

namespace HPCTechMovieSite2024.Client.HttpRepo;

public interface IUserMoviesHttpRepo
{
    // get movies
    Task<DataResponse<List<OMDBMovie>>> GetMovies(string userName);
    // add movie
    Task<Response> AddMovie(string imdbId);
    // remove movie
    Task<Response> RemoveMovie(string imdbId);

    Task<DataResponse<List<UserEditDto>>> GetUsers();

    Task<bool> EmailConfirmUser(string userId);

    Task<bool> ToggleAdmin(string userId);
}
