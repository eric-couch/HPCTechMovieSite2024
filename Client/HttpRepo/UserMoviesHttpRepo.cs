using Blazored.LocalStorage;
using HPCTechMovieSite2024.Shared;
using HPCTechMovieSite2024.Shared.Wrapper;
using Newtonsoft.Json;
using Syncfusion.Blazor.PivotView;
using System.Net;
using System.Net.Http.Json;


namespace HPCTechMovieSite2024.Client.HttpRepo;

public class UserMoviesHttpRepo : IUserMoviesHttpRepo
{
    public readonly HttpClient _httpClient;
    public readonly ILocalStorageService _localStorage;
    private readonly string OMDBUrl = "https://www.omdbapi.com/?";
    private readonly string apiKey = "apikey=86c39163";

    public UserMoviesHttpRepo(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<DataResponse<List<OMDBMovie>>> GetMovies(string userName)
    {
        try
        {
            var Movies = new List<OMDBMovie>();
            string apiEndPoint = $"api/User?userName={userName}";
            DataResponse<UserDto> response = await _httpClient.GetFromJsonAsync<DataResponse<UserDto>>(apiEndPoint);

            if (response.Success)
            {
                return new DataResponse<List<OMDBMovie>>()
                {
                    Success = true,
                    Data = response.Data.OMDBMovies
                };
            } else
            {
                return new DataResponse<List<OMDBMovie>>()
                {
                    Success = false,
                    Message = "No movies found.",
                    Data = new List<OMDBMovie>()
                };
            }
            
        } catch (HttpRequestException ex)
        {
            if (ex.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
            {
                return new DataResponse<List<OMDBMovie>>
                {
                    Success = false,
                    Message = "Not Authroized",
                    Data = new List<OMDBMovie>()
                };
            } else
            {
                return new DataResponse<List<OMDBMovie>>
                {
                    Success = false,
                    Message = "Http Exception",
                    Data = new List<OMDBMovie>()
                };
            }
        } catch (NotSupportedException ex)
        {
            return new DataResponse<List<OMDBMovie>>
            {
                Success = false,
                Message = "Method not supported.",
                Data = new List<OMDBMovie>()
            };
        } catch(JsonException ex)
        {
            return new DataResponse<List<OMDBMovie>>
            {
                Success = false,
                Message = "Data not returned.",
                Data = new List<OMDBMovie>()
            };
        }
        catch (Exception ex)      // always do this last
        {
            return new DataResponse<List<OMDBMovie>>
            {
                Success = false,
                Message = ex.Message,
                Data = new List<OMDBMovie>()
            };
        }
        
    }

    public async Task<DataResponse<Movie>> GetMovie(string imdbId, string userName)
    {
        var movie = await _httpClient.GetFromJsonAsync<DataResponse<Movie>>($"api/movie?imdbId={imdbId}&userName={userName}");
        if (movie?.Success ?? false)
        {
            return new DataResponse<Movie>() { Success = true, Data = movie.Data };
        }
        else
        {
            return new DataResponse<Movie>() { Success = false, Data = new Movie() };
        }
    }

    public async Task<Response> UpdateRating(MovieUpdateRating rating)
    {
        try
        {
            var responseMsg = await _httpClient.PostAsJsonAsync("api/update-rating", rating);
            Response res = await responseMsg.Content.ReadFromJsonAsync<Response>();
            return res;
        } catch (Exception ex)
        {
            return new Response("Update Rating Failed.");
        }
    }

    public async Task<Response> RemoveMovie(string imdbId)
    {
        try
        {
            Movie newMovie = new Movie() { imdbId = imdbId };
            var responseFromAPI = await _httpClient.PostAsJsonAsync("api/remove-movie", newMovie);
            Response res = await responseFromAPI.Content.ReadFromJsonAsync<Response>();
            return res;
        } catch (Exception ex)
        {
            return new Response("Remove Movie Failed.");
        }
    }

    public async Task<Response> UpdateMovie(Movie movie)
    {
        var responseFromApi = await _httpClient.PostAsJsonAsync("api/update-movie", movie);
        Response res = await responseFromApi.Content.ReadFromJsonAsync<Response>();
        return res;
    }
    public async Task<Response> AddMovie(string imdbId, string userName)
    {
        try
        {

            Movie newMovie = new Movie() { imdbId = imdbId };
            var responseFromAPI = await _httpClient.PostAsJsonAsync($"api/add-movie?userName={userName}", newMovie);
            Response res = await responseFromAPI.Content.ReadFromJsonAsync<Response>();
            return res;
        }
        catch (Exception ex)
        {
            return new Response("Add Movie Failed.");
        }
    }

    public async Task<DataResponse<List<MovieStatistic>>> GetTopMovies(int countOfMovies)
    {
        var movies = await _httpClient.GetFromJsonAsync<DataResponse<List<MovieStatistic>>>($"api/top-movies?countOfMovies={countOfMovies}");
        if (movies?.Success ?? false)
        {
            return new DataResponse<List<MovieStatistic>>()
            {
                Success = true,
                Data = movies.Data
            };
        } else
        {
            return new DataResponse<List<MovieStatistic>>()
            {
                Success = false,
                Data = new List<MovieStatistic>(),
                Message = "No movies found."
            };
        }
    }

    public async Task<DataResponse<List<UserEditDto>>> GetUsers()
    {
        try
        {
            var users = await _httpClient.GetFromJsonAsync<List<UserEditDto>>("api/users");
            if (users is not null)
            {
                return new DataResponse<List<UserEditDto>>()
                {
                    Data = users,
                    Message = "Success",
                    Success = true
                };
            } else
            {
                return new DataResponse<List<UserEditDto>>()
                {
                    Data = new List<UserEditDto>(),
                    Message = "Users not found",
                    Success = false
                };
            }
        } catch (Exception ex)
        {
            return new DataResponse<List<UserEditDto>>()
            {
                Data = new List<UserEditDto>(),
                Message = ex.Message,
                Success = false
            };
        }
    }

    public async Task<Response> UpdateUser(UserEditDto user)
    {
        var response = await _httpClient.PostAsJsonAsync("api/update-user", user);
        if (response.IsSuccessStatusCode)
        {
            return new Response(true, "Success");
        } else
        {
            return new Response(false, "Failed api call");
        }
    }

    public async Task<bool> EmailConfirmUser(string userId)
    {
        var response = await _httpClient.GetFromJsonAsync<bool>($"api/toggle-email-confirmed?userId={userId}");
        return response;
    }

    public async Task<bool> ToggleAdmin(string userId)
    {
        var response = await _httpClient.GetFromJsonAsync<bool>($"api/toggle-admin?userId={userId}");
        return response;
    }
}
