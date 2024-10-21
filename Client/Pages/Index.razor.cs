using Microsoft.AspNetCore.Components;
using HPCTechMovieSite2024.Shared;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Syncfusion.Blazor.Notifications.Internal;
using Syncfusion.Blazor.Notifications;
using HPCTechMovieSite2024.Client.HttpRepo;
using System.Diagnostics;
using Syncfusion.Blazor.PivotView;
using HPCTechMovieSite2024.Shared.Wrapper;

namespace HPCTechMovieSite2024.Client.Pages;

public partial class Index
{
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject]
    public IUserMoviesHttpRepo UserMoviesHttpRepo { get; set; }
    [Inject]
    public ILogger<Index> Logger { get; set; }

    public List<OMDBMovie> Movies { get; set; } = new();
    public SfToast ToastObj;
    private string? toastContent = String.Empty;
    private string? toastSuccess = "e-toast-success";
    public UserDto? user { get; set; } = null;
    
    protected override async Task OnInitializedAsync()
    {
        var userAuth = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity;
        if (userAuth is not null && userAuth.IsAuthenticated)
        {
            var response = await UserMoviesHttpRepo.GetMovies(userAuth.Name);
            if (response.Success)
            {
                Movies = response.Data;
                Logger.LogInformation("return successful for user {user}", userAuth.Name);
            } else
            {
                // log error
                Logger.LogError(response.Message);
                // show toast to user that something went wrong
                toastContent = $"Error: {response.Message}";
                toastSuccess = "e-toast-warning";
                StateHasChanged();
                await ToastObj.ShowAsync();
            }
        }
    }

    private async Task RemoveFavoriteMovie(OMDBMovie movie)
    {

        Response response = await UserMoviesHttpRepo.RemoveMovie(movie.imdbID);
        if (response.Success)
        {
            Movies.Remove(movie);
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
