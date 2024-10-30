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
using Syncfusion.Blazor.Inputs;

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
    public bool IsMovieModalVisible { get; set; } = false;
    public Movie movieEdit { get; set; } = new();


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

    private async Task MovieRatingChanged(MovieUpdateRating rating)
    {
        var userAuth = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity;
        if (userAuth is not null && userAuth.IsAuthenticated)
        {
            rating.UserName = userAuth.Name;
            Response response = await UserMoviesHttpRepo.UpdateRating(rating);
            if (response.Success)
            {
                toastContent = $"Update Rating Successfully";
                toastSuccess = "e-toast-success";
                StateHasChanged();
                await ToastObj.ShowAsync();
            }
            else
            {
                toastContent = $"Failed to update Rating.";
                toastSuccess = "e-toast-warning";
                StateHasChanged();
                await ToastObj.ShowAsync();
            }

        }
    }

    private async Task EditFavoriteMovie(OMDBMovie movie)
    {
        // go get the movie (not omdbmovie) info from the server because we need the userid and movie.id
        var userAuth = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity;
        if (userAuth is not null && userAuth.IsAuthenticated)
        {
            DataResponse<Movie> response = await UserMoviesHttpRepo.GetMovie(movie.imdbID, userAuth.Name);

            if (response.Success)
            {
                movieEdit = response.Data;
                IsMovieModalVisible = true;
            } else
            {
                toastContent = $"User Movie not found!";
                toastSuccess = "e-toast-warning";
                StateHasChanged();
                await ToastObj.ShowAsync();
            }
        }
    }

    private async Task UpdateMovieOnSubmit()
    {
        var userAuth = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity;
        if (userAuth is not null && userAuth.IsAuthenticated)
        {
            Response response = await UserMoviesHttpRepo.UpdateMovie(movieEdit);
            if (response.Success)
            {
                var omdbMovie = (from o in Movies
                                 where o.imdbID == movieEdit.imdbId
                                 select o).FirstOrDefault();

                var ratingObj = (from r in omdbMovie.Ratings
                                 where r.Source == "User"
                select r).FirstOrDefault();

                ratingObj.Value = movieEdit.userRating.ToString();
                StateHasChanged();
                movieEdit = new();
                IsMovieModalVisible = false;
                toastContent = $"User Movie Updated!";
                toastSuccess = "e-toast-success";
                StateHasChanged();
                await ToastObj.ShowAsync();
            }
            else
            {
                toastContent = $"User Movie Update Failed!";
                toastSuccess = "e-toast-warning";
                StateHasChanged();
                await ToastObj.ShowAsync();
            }
        } else
        {
            toastContent = $"Can't find user!";
            toastSuccess = "e-toast-warning";
            StateHasChanged();
            await ToastObj.ShowAsync();
        }
    }

    public async Task UpdateRating(double rating)
    {
        movieEdit.userRating = rating;
    }

    private async Task RemoveFavoriteMovie(OMDBMovie movie)
    {

        Response response = await UserMoviesHttpRepo.RemoveMovie(movie.imdbID);
        if (response.Success)
        {
            Movies.Remove(movie);
            toastContent = $"Removed {movie.Title} from your favorites.";
            toastSuccess = "e-toast-success";
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

    public void Reset()
    {
        movieEdit = new();
        IsMovieModalVisible = false;
    }
}
