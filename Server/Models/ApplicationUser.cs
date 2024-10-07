using Microsoft.AspNetCore.Identity;
using HPCTechMovieSite2024.Shared;

namespace HPCTechMovieSite2024.Server.Models;

public class ApplicationUser : IdentityUser
{
    // List of favorite movies
    public List<Movie> FavoriteMovies { get; set; } = new();
}
