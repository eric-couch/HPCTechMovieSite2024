using HPCTechMovieSite2024.Shared;
using HPCTechMovieSite2024.Server.Services;
using Moq;
using HPCTechMovieSite2024.Server.Controllers;
using HPCTechMovieSite2024.Shared.Wrapper;

namespace HPCTechMovieSite2024.Server.Test;

public class Tests
{
    private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task GetMovies_ShouldReturnUserDto_WhereUserExists()
    {
        // Arrange
        string userId = "f3f68693-628b-4cde-a705-5a681b3ef50f";
        string userName = "eric.couch@example.net";
        UserDto returnUser = new UserDto
        {
            Id = userId,
            UserName = userName,
            FavoriteMovies = new List<Movie>()
            {
                new Movie()
                {
                    Id = 1,
                    imdbId = "tt0816692"
                },
                new Movie()
                {
                    Id = 3,
                    imdbId = "tt0068646"
                },
                new Movie()
                {
                    Id = 5,
                    imdbId = "tt0372784"
                }
            }
        };
        
        _userServiceMock.Setup(x => x.GetMovies(userName)).ReturnsAsync(returnUser);

        UserController userController = new UserController(_userServiceMock.Object);

        // Act
        var response = await userController.GetMovies(userName);
        var userDto = (UserDto)response.Data;


        // Assert 
        Assert.That(userDto, Is.TypeOf<UserDto>());
        Assert.That(userDto.UserName, Is.EqualTo("eric.couch@example.net"));
        Assert.That(userDto.FavoriteMovies[0].imdbId, Is.EqualTo("tt0816692"));
    }
}