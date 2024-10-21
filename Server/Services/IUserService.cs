using HPCTechMovieSite2024.Shared;

namespace HPCTechMovieSite2024.Server.Services;

public interface IUserService
{
    Task<UserDto>? GetMovies(string userName);
}
