using HPCTechMovieSite2024.Server.Data;
using HPCTechMovieSite2024.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HPCTechMovieSite2024.Shared;
using Microsoft.AspNetCore.Identity;
using HPCTechMovieSite2024.Server.Models;
using System.Security.Claims;

namespace HPCTechMovieSite2024.Server.Controllers;

public class UserController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Route("api/User")]
    public async Task<IActionResult> GetMovies([FromQuery(Name ="userName")] string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);

        UserDto? movies = await _context.Users.Include(u => u.FavoriteMovies)
                                .Select(u => new UserDto
                                {
                                    Id = u.Id,
                                    UserName = u.UserName,
                                    FavoriteMovies = u.FavoriteMovies
                                }).FirstOrDefaultAsync(u => u.Id == user.Id);
        if (movies != null)
        {
            return Ok(movies);
        } else
        {
            return NotFound();
        }
        
    }

    [HttpPost]
    [Route("api/add-movie")]
    public async Task<ActionResult> AddMovie([FromBody] Movie movie)
    {
        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (user is null)
        {
            return NotFound();
        }

        user.FavoriteMovies.Add(movie);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    [Route("api/remove-movie")]
    public async Task<ActionResult> RemoveMovie([FromBody] Movie movie)
    {
        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user is null)
        {
            return NotFound();
        }

        var movieToRemove = _context.Users.Include(u => u.FavoriteMovies)
                                    .FirstOrDefault(u => u.Id == user.Id)
                                    .FavoriteMovies.FirstOrDefault(m => m.imdbId == movie.imdbId);
        _context.Movies.Remove(movieToRemove);
        _context.SaveChangesAsync();
        return Ok();
    }

    [Route("api/Hello/{name}")]
    public IActionResult Hello(string name)
    {
        // this is a server side view state engine (or rendering engine)
        // default view engine for asp mvc is called razor
        //return View();

        // raw content
        //return Content($"hello, {name}");

        // return file
        //var fileBytes = System.IO.File.ReadAllBytes("path/to/filename.pdf");
        //return File(fileBytes, "application/pdf", "downloadedfiled.pdf");

        //return Redirect("api/User");

        if (name == "Eric")
        {
            return Content($"Hello, Eric!");
        } else
        {
            return Ok(new Movie { Id = 1, imdbId = "tt1234567" });
        }

        // return BadRequest() 400
        // return NoContent() 204

    }

}
