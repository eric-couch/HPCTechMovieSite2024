using Microsoft.AspNetCore.Components;

namespace HPCTechMovieSite2024.Client.Pages;

public partial class SearchFavorites
{
    [Inject]
    public HttpClient _httpClient { get; set; }
    public string searchTerm { get; set; } = String.Empty;
    private string? searchResult { get; set; } = String.Empty;

    private async Task Search()
    {
        searchResult = await _httpClient.GetStringAsync($"api/search-favorites?searchTerm={searchTerm}");
    }
}
