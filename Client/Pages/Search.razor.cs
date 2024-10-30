using HPCTechMovieSite2024.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.PivotView;
using Syncfusion.Blazor.Notifications;
using HPCTechMovieSite2024.Client.HttpRepo;
using Microsoft.AspNetCore.Components.Authorization;

namespace HPCTechMovieSite2024.Client.Pages;

public partial class Search
{
    [Inject]
    public HttpClient _httpClient { get; set; }
    [Inject]
    public IUserMoviesHttpRepo UserMoviesHttpRepo { get; set; }
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    public SfPager? Page;
    public SfGrid<MovieSearchResultItem> movieGrid;
    public SfToast ToastObj;
    private string? toastContent = String.Empty;
    private string? toastSuccess = "e-toast-success";

    public int page { get; set; } = 1;
    public string searchTerm { get; set; }
    private int totalItems { get; set; } = 0;
    private MovieSearchResult searchResult { get; set; } = new();
    private List<MovieSearchResultItem> OMDBMovies { get; set; }
    private List<MovieSearchResultItem> selectedMovies { get; set; }
    private OMDBMovie? omdbMovie { get; set; } = null;
    private string OMDBUrl = "https://www.omdbapi.com/?";
    private string apiKey = "apikey=86c39163";

    private async Task PageClick(PagerItemClickEventArgs args)
    {
        page = args.CurrentPage;
        await SearchOMDB();
    }

    private async Task GetSelectedRows(RowSelectEventArgs<MovieSearchResultItem> args)
    {
        selectedMovies = await movieGrid.GetSelectedRecordsAsync();
        omdbMovie = await _httpClient.GetFromJsonAsync<OMDBMovie>($"{OMDBUrl}{apiKey}&i={args.Data.imdbID}");
    }

    public async Task ToolbarClickHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "GridMovieAdd")
        {
            if (selectedMovies is not null)
            {
                var userAuth = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity;
                if (userAuth is not null && userAuth.IsAuthenticated)
                {
                    foreach (MovieSearchResultItem movie in selectedMovies)
                    {

                        var response = await UserMoviesHttpRepo.AddMovie(movie.imdbID, userAuth.Name);

                        if (response.Success)
                        {
                            toastContent = $"Added {movie.Title} to your favorites.";
                            StateHasChanged();
                            await ToastObj.ShowAsync();
                        }
                        else
                        {
                            toastContent = $"Failed to add movie {movie.Title} to your favorites.";
                            toastSuccess = "e-toast-warning";
                            StateHasChanged();
                            await ToastObj.ShowAsync();
                        }
                    }
                }
            }
        }
    }

    private async Task SearchOMDB()
    {
        searchResult = await _httpClient.GetFromJsonAsync<MovieSearchResult>($"{OMDBUrl}{apiKey}&s={searchTerm}&page={page}");
        if (searchResult?.Search?.Any() ?? false)
        {
            OMDBMovies = searchResult.Search.ToList();
            totalItems = Int32.Parse(searchResult.totalResults);
        }
    }
}
