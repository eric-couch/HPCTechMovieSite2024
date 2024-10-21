using HPCTechMovieSite2024.Shared;
using HPCTechMovieSite2024.Shared.Wrapper;

namespace HPCTechMovieSite2024.Client.HttpRepo;

public interface IUserMoviesHttpRepo
{
    // get movies
    Task<DataResponse<List<OMDBMovie>>> GetMovies();
    // add movie
    Task<Response> AddMovie(string imdbId);
    // remove movie
    Task<Response> RemoveMovie(string imdbId);
}
