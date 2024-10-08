using Microsoft.AspNetCore.Components;
using HPCTechMovieSite2024.Shared;
using System.Net.Http.Json;

namespace HPCTechMovieSite2024.Client.Pages;

public partial class Index
{
    [Inject]
    public HttpClient _httpClient {  get; set; }

    public List<OMDBMovie> Movies { get; set; } = new();

    public UserDto user { get; set; }
    private string OMDBUrl = "https://www.omdbapi.com/?";
    private string apiKey = "apikey=86c39163";
    protected override async Task OnInitializedAsync()
    {
        user = await _httpClient.GetFromJsonAsync<UserDto>($"api/User");

        if (user is not null)
        {
            foreach(Movie movie in user.FavoriteMovies) {
                OMDBMovie omdbMovie = await _httpClient.GetFromJsonAsync<OMDBMovie>($"{OMDBUrl}{apiKey}&i={movie.imdbId}");
                if (movie is not null)
                {
                    Movies.Add(omdbMovie);
                }
            }
        }
    }
}
