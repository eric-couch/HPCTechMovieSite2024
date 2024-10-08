using HPCTechMovieSite2024.Server.Data;
using HPCTechMovieSite2024.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HPCTechMovieSite2024.Shared;

namespace HPCTechMovieSite2024.Server.Controllers;

public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Route("api/User")]
    public async Task<IActionResult> GetMovies()
    {
        UserDto? movies = await _context.Users.Include(u => u.FavoriteMovies)
                                .Select(u => new UserDto
                                {
                                    Id = u.Id,
                                    UserName = u.UserName,
                                    FavoriteMovies = u.FavoriteMovies
                                }).FirstOrDefaultAsync();
        if (movies != null)
        {
            return Ok(movies);
        } else
        {
            return NotFound();
        }
        
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
