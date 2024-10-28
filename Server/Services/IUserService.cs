using HPCTechMovieSite2024.Shared;

namespace HPCTechMovieSite2024.Server.Services;

public interface IUserService
{
    Task<UserDto>? GetMovies(string userName);
    Task<List<UserEditDto>> GetUsers();
    Task<bool> UpdateRating(MovieUpdateRating rating);
    Task<bool> ToggleAdmin(string userId);
    Task<bool> UpdateUser(UserEditDto user);


    Task<bool> ToggleEmailConfirmed(string userId);
}
