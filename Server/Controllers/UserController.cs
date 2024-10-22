using HPCTechMovieSite2024.Server.Data;
using HPCTechMovieSite2024.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HPCTechMovieSite2024.Shared;
using HPCTechMovieSite2024.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using HPCTechMovieSite2024.Server.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using HPCTechMovieSite2024.Shared.Wrapper;
using HPCTechMovieSite2024.Server.Services;

namespace HPCTechMovieSite2024.Server.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(      IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("api/User")]
    public async Task<DataResponse<UserDto>> GetMovies(string? userName = null)
    {
        
        if (userName is null) {
            userName = User.Identity.Name;
        }

        // UserService.GetMovies
        var userDto = await _userService.GetMovies(userName);

        if (userDto != null)
        {
            return new DataResponse<UserDto>() { Data = userDto, Success = true };
        } else
        {
            return new DataResponse<UserDto>() { Data = new UserDto(), Success = false, Message = "user not found." };
        }
        
    }

    //[HttpPost]
    //[Route("api/add-movie")]
    //public async Task<Response> AddMovie([FromBody] Movie movie)
    //{
    //    var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

    //    if (user is null)
    //    {
    //        return new Response(false, "Failed to Add Movie");
    //    }

    //    user.FavoriteMovies.Add(movie);
    //    await _context.SaveChangesAsync();
    //    return new Response(true, "Successfully Added Movie");
    //}

    //[HttpPost]
    //[Route("api/remove-movie")]
    //public async Task<Response> RemoveMovie([FromBody] Movie movie)
    //{
    //    var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
    //    if (user is null)
    //    {
    //        return new Response(false, "Failed to Remove Movie");
    //    }

    //    var movieToRemove = _context.Users.Include(u => u.FavoriteMovies)
    //                                .FirstOrDefault(u => u.Id == user.Id)
    //                                .FavoriteMovies.FirstOrDefault(m => m.imdbId == movie.imdbId);
    //    _context.Movies.Remove(movieToRemove);
    //    _context.SaveChangesAsync();
    //    return new Response(true, "Successfully Removed Movie");
    //}

    [HttpGet]
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
