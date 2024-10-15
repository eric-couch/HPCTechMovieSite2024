using Microsoft.AspNetCore.Identity;
using HPCTechMovieSite2024.Shared;
using System.ComponentModel.DataAnnotations;

namespace HPCTechMovieSite2024.Server.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(255)]
    public string FirstName { get; set; }
    [MaxLength(255)]
    public string LastName { get; set; }
    // List of favorite movies
    public List<Movie> FavoriteMovies { get; set; } = new();
}
