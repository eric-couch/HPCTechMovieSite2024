using HPCTechMovieSite2024.Client.HttpRepo;
using HPCTechMovieSite2024.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Syncfusion.Blazor.Notifications;

namespace HPCTechMovieSite2024.Client.Pages;

public partial class TopMovies
{
    public SfToast ToastObj;
    private string? toastContent = String.Empty;
    private string? toastSuccess = "e-toast-success";
    [Inject]
    public IUserMoviesHttpRepo UserMoviesHttpRepo { get; set; }
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    public List<MovieStatistic> Movies { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var response = await UserMoviesHttpRepo.GetTopMovies(10);
        if (response.Success)
        {
            Movies = response.Data;
        }
        else
        {
            toastContent = $"Error: {response.Message}";
            toastSuccess = "e-toast-warning";
            StateHasChanged();
            await ToastObj.ShowAsync();
        }
    }
}
