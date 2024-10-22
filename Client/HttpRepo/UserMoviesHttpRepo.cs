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
    private readonly string OMDBUrl = "https://www.omdbapi.com/?";
    private readonly string apiKey = "apikey=86c39163";

    public UserMoviesHttpRepo(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
                UserDto user = response.Data;
                if (user is not null)
                {
                    foreach (Movie movie in user.FavoriteMovies)
                    {
                        OMDBMovie omdbMovie = await _httpClient.GetFromJsonAsync<OMDBMovie>($"{OMDBUrl}{apiKey}&i={movie.imdbId}");
                        if (movie is not null)
                        {
                            Movies.Add(omdbMovie);
                        }
                    }
                }
                return new DataResponse<List<OMDBMovie>>()
                {
                    Success = true,
                    Data = Movies
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

    public async Task<Response> AddMovie(string imdbId)
    {
        try
        {
            Movie newMovie = new Movie() { imdbId = imdbId };
            var responseFromAPI = await _httpClient.PostAsJsonAsync("api/add-movie", newMovie);
            Response res = await responseFromAPI.Content.ReadFromJsonAsync<Response>();
            return res;
        }
        catch (Exception ex)
        {
            return new Response("Add Movie Failed.");
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
}
