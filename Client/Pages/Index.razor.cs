using Microsoft.AspNetCore.Components;
using HPCTechMovieSite2024.Shared;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Syncfusion.Blazor.Notifications.Internal;
using Syncfusion.Blazor.Notifications;

namespace HPCTechMovieSite2024.Client.Pages;

public partial class Index
{
    [Inject]
    public HttpClient _httpClient {  get; set; }
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    public List<OMDBMovie> Movies { get; set; } = new();
    public SfToast ToastObj;
    private string? toastContent = String.Empty;
    private string? toastSuccess = "e-toast-success";
    public UserDto? user { get; set; } = null;
    private string OMDBUrl = "https://www.omdbapi.com/?";
    private string apiKey = "apikey=86c39163";
    protected override async Task OnInitializedAsync()
    {
        var userAuth = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity;
        if (userAuth is not null && userAuth.IsAuthenticated )
        {
            user = await _httpClient.GetFromJsonAsync<UserDto>($"api/User?userName={userAuth.Name}");

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
        }
    }

    private async Task RemoveFavoriteMovie(OMDBMovie movie)
    {
        Movie newMovie = new Movie() { imdbId = movie.imdbID };
        var response = await _httpClient.PostAsJsonAsync("api/remove-movie", newMovie);
        Movies.Remove(movie);
        if (response.IsSuccessStatusCode)
        {
            toastContent = $"Removed {movie.Title} from your favorites.";
            StateHasChanged();
            await ToastObj.ShowAsync();
        }
        else
        {
            toastContent = $"Failed to remove movie {movie.Title} from your favorites.";
            toastSuccess = "e-toast-warning";
            StateHasChanged();
            await ToastObj.ShowAsync();
        }
    }
}
