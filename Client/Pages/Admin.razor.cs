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
    public bool IsUserModalVisible { get; set; } = false;
    public UserEditDto userEditDto { get; set; }

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

    public async Task ToggleEnableUser(ChangeEventArgs args, string userId)
    {
        bool response = await UserMoviesHttpRepo.EmailConfirmUser(userId);
        if (!response)
        {
            // toast response to user
        }
    }

    public async Task ToggleAdmin(ChangeEventArgs args, string userId)
    {
        bool response = await UserMoviesHttpRepo.ToggleAdmin(userId);
        if (!response)
        {
            // toast response to user
        }
    }

    public async Task UserDoubleClickHandler(RecordDoubleClickEventArgs<UserEditDto> args)
    {
        userEditDto = args.RowData;
        IsUserModalVisible = true;
    }

    public async Task AddUserOnSubmit()
    {
        //var response = await UserMoviesHttpRepo.UserUser(userEditDto);
    }

    public void Reset()
    {
        userEditDto = new();
        IsUserModalVisible = false;
    }
}
