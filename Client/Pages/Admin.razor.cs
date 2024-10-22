using HPCTechMovieSite2024.Shared;
using HPCTechMovieSite2024.Shared.Wrapper;
using Syncfusion.Blazor.Grids;
using HPCTechMovieSite2024.Client.HttpRepo;
using Microsoft.AspNetCore.Components;

namespace HPCTechMovieSite2024.Client.Pages;

public partial class Admin
{
    [Inject]
    public IUserMoviesHttpRepo UserMoviesHttpRepo { get; set; }
    public List<UserEditDto>? Users { get; set; }
    public SfGrid<UserEditDto> UserGrid;

    protected override async Task OnInitializedAsync()
    {
        DataResponse<List<UserEditDto>> response = await UserMoviesHttpRepo.GetUsers();
        if (response.Success)
        {
            Users = response.Data;
        } else
        {
            // add toast
        }
    }
}
