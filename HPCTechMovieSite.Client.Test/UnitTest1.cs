using HPCTechMovieSite2024.Client;
using HPCTechMovieSite2024.Client.HttpRepo;
using RichardSzalay.MockHttp;

namespace HPCTechMovieSite.Client.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Test_GetMovies_ReturnUserDto_Success()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        // Mock api/User end point which returns a UserDto
        // also mock the OMDB api return for individual movies
        string testUserResponse = """
            {
              "data": {
                "id": "f3f68693-628b-4cde-a705-5a681b3ef50f",
                "userName": "eric.couch@example.net",
                "favoriteMovies": [
                  {
                    "id": 1,
                    "imdbId": "tt0816692",
                    "applicationUserId": "f3f68693-628b-4cde-a705-5a681b3ef50f"
                  },
                  {
                    "id": 3,
                    "imdbId": "tt0068646",
                    "applicationUserId": "f3f68693-628b-4cde-a705-5a681b3ef50f"
                  },
                  {
                    "id": 5,
                    "imdbId": "tt0372784",
                    "applicationUserId": "f3f68693-628b-4cde-a705-5a681b3ef50f"
                  }
                ]
              },
              "success": true,
              "message": null,
              "errors": {}
            }
            """;
        string testInterstellarResponse = """
            {
              "Title": "Interstellar",
              "Year": "2014",
              "Rated": "PG-13",
              "Released": "07 Nov 2014",
              "Runtime": "169 min",
              "Genre": "Adventure, Drama, Sci-Fi",
              "Director": "Christopher Nolan",
              "Writer": "Jonathan Nolan, Christopher Nolan",
              "Actors": "Matthew McConaughey, Anne Hathaway, Jessica Chastain",
              "Plot": "When Earth becomes uninhabitable in the future, a farmer and ex-NASA pilot, Joseph Cooper, is tasked to pilot a spacecraft, along with a team of researchers, to find a new planet for humans.",
              "Language": "English",
              "Country": "United States, United Kingdom, Canada",
              "Awards": "Won 1 Oscar. 44 wins & 148 nominations total",
              "Poster": "https://m.media-amazon.com/images/M/MV5BYzdjMDAxZGItMjI2My00ODA1LTlkNzItOWFjMDU5ZDJlYWY3XkEyXkFqcGc@._V1_SX300.jpg",
              "Ratings": [
                {
                  "Source": "Internet Movie Database",
                  "Value": "8.7/10"
                },
                {
                  "Source": "Rotten Tomatoes",
                  "Value": "73%"
                },
                {
                  "Source": "Metacritic",
                  "Value": "74/100"
                }
              ],
              "Metascore": "74",
              "imdbRating": "8.7",
              "imdbVotes": "2,170,072",
              "imdbID": "tt0816692",
              "Type": "movie",
              "DVD": "N/A",
              "BoxOffice": "$188,020,017",
              "Production": "N/A",
              "Website": "N/A",
              "Response": "True"
            }
            """;

        string testTheGodfatherResponse = """
                        {
              "Title": "The Godfather",
              "Year": "1972",
              "Rated": "R",
              "Released": "24 Mar 1972",
              "Runtime": "175 min",
              "Genre": "Crime, Drama",
              "Director": "Francis Ford Coppola",
              "Writer": "Mario Puzo, Francis Ford Coppola",
              "Actors": "Marlon Brando, Al Pacino, James Caan",
              "Plot": "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.",
              "Language": "English, Italian, Latin",
              "Country": "United States",
              "Awards": "Won 3 Oscars. 31 wins & 31 nominations total",
              "Poster": "https://m.media-amazon.com/images/M/MV5BYTJkNGQyZDgtZDQ0NC00MDM0LWEzZWQtYzUzZDEwMDljZWNjXkEyXkFqcGc@._V1_SX300.jpg",
              "Ratings": [
                {
                  "Source": "Internet Movie Database",
                  "Value": "9.2/10"
                },
                {
                  "Source": "Rotten Tomatoes",
                  "Value": "97%"
                },
                {
                  "Source": "Metacritic",
                  "Value": "100/100"
                }
              ],
              "Metascore": "100",
              "imdbRating": "9.2",
              "imdbVotes": "2,057,473",
              "imdbID": "tt0068646",
              "Type": "movie",
              "DVD": "N/A",
              "BoxOffice": "$136,381,073",
              "Production": "N/A",
              "Website": "N/A",
              "Response": "True"
            }
            """;

        string testBatmanBeginsResponse = """
                        {
              "Title": "Batman Begins",
              "Year": "2005",
              "Rated": "PG-13",
              "Released": "15 Jun 2005",
              "Runtime": "140 min",
              "Genre": "Action, Crime, Drama",
              "Director": "Christopher Nolan",
              "Writer": "Bob Kane, David S. Goyer, Christopher Nolan",
              "Actors": "Christian Bale, Michael Caine, Ken Watanabe",
              "Plot": "After witnessing his parents' death, Bruce learns the art of fighting to confront injustice. When he returns to Gotham as Batman, he must stop a secret society that intends to destroy the city.",
              "Language": "English, Mandarin",
              "Country": "United States, United Kingdom",
              "Awards": "Nominated for 1 Oscar. 15 wins & 79 nominations total",
              "Poster": "https://m.media-amazon.com/images/M/MV5BODIyMDdhNTgtNDlmOC00MjUxLWE2NDItODA5MTdkNzY3ZTdhXkEyXkFqcGc@._V1_SX300.jpg",
              "Ratings": [
                {
                  "Source": "Internet Movie Database",
                  "Value": "8.2/10"
                },
                {
                  "Source": "Rotten Tomatoes",
                  "Value": "85%"
                },
                {
                  "Source": "Metacritic",
                  "Value": "70/100"
                }
              ],
              "Metascore": "70",
              "imdbRating": "8.2",
              "imdbVotes": "1,607,409",
              "imdbID": "tt0372784",
              "Type": "movie",
              "DVD": "N/A",
              "BoxOffice": "$206,863,479",
              "Production": "N/A",
              "Website": "N/A",
              "Response": "True"
            }
            """;
        mockHttp.When("https://localhost:7115/api/User")
                .Respond("application/json", testUserResponse);
        mockHttp.When("https://www.omdbapi.com/?apikey=86c39163&i=tt0816692")
                .Respond("application/json", testInterstellarResponse);
        mockHttp.When("https://www.omdbapi.com/?apikey=86c39163&i=tt0068646")
                .Respond("application/json", testTheGodfatherResponse);
        mockHttp.When("https://www.omdbapi.com/?apikey=86c39163&i=tt0372784")
                .Respond("application/json", testBatmanBeginsResponse);

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri("https://localhost:7115/");
        var userMoviesHttpRepo = new UserMoviesHttpRepo(client);

        // Act
        var response = await userMoviesHttpRepo.GetMovies();
        var movies = response.Data;

        // Assert 
        Assert.That(movies.Count(), Is.EqualTo(3));
        Assert.That(movies[0].Title, Is.EqualTo("Interstellar"));
        Assert.That(movies[0].Year, Is.EqualTo("2014"));
        Assert.That(movies[1].Title, Is.EqualTo("The Godfather"));
        Assert.That(movies[1].Year, Is.EqualTo("1972"));
    }
}