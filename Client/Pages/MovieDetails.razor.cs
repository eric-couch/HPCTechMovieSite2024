using Microsoft.AspNetCore.Components;
using HPCTechMovieSite2024.Shared;

namespace HPCTechMovieSite2024.Client.Pages;

public partial class MovieDetails
{
    [Parameter]
    public OMDBMovie? Movie { get; set; }
}
