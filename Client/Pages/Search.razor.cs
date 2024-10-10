using HPCTechMovieSite2024.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.PivotView;

namespace HPCTechMovieSite2024.Client.Pages;

public partial class Search
{
    [Inject]
    public HttpClient _httpClient { get; set; }
    public SfPager? Page;
    public int page { get; set; } = 1;
    public string searchTerm { get; set; }
    private int totalItems { get; set; } = 0;
    private MovieSearchResult searchResult { get; set; } = new();
    private List<MovieSearchResultItem> OMDBMovies { get; set; }
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
        omdbMovie = await _httpClient.GetFromJsonAsync<OMDBMovie>($"{OMDBUrl}{apiKey}&i={args.Data.imdbID}");
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
