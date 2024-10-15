using Microsoft.AspNetCore.Components;
using HPCTechMovieSite2024.Shared;

namespace HPCTechMovieSite2024.Client.Pages;

public partial class MovieDetails
{
    [Parameter]
    public OMDBMovie? Movie { get; set; }
    [Parameter]
    public EventCallback<OMDBMovie> OnRemoveFavoriteMovie { get; set; }
    [Parameter]
    public bool AllowDelete { get; set; }


    private async Task RemoveFavoriteMovie(OMDBMovie movie)
    {
        await OnRemoveFavoriteMovie.InvokeAsync(movie);
    }
}
