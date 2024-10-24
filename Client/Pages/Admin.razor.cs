using HPCTechMovieSite2024.Shared;
using HPCTechMovieSite2024.Shared.Wrapper;
using Syncfusion.Blazor.Grids;
using HPCTechMovieSite2024.Client.HttpRepo;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Notifications;

namespace HPCTechMovieSite2024.Client.Pages;

public partial class Admin
{
    [Inject]
    public IUserMoviesHttpRepo UserMoviesHttpRepo { get; set; }
    public List<UserEditDto>? Users { get; set; }
    public SfGrid<UserEditDto> UserGrid;
    public bool IsUserModalVisible { get; set; } = false;
    public UserEditDto userEditDto { get; set; }
    public SfToast ToastObj;
    private string? toastContent = String.Empty;
    private string? toastSuccess = "e-toast-success";

    protected override async Task OnInitializedAsync()
    {
        await ReloadGrid();
    }

    public async Task ReloadGrid()
    {
        DataResponse<List<UserEditDto>> response = await UserMoviesHttpRepo.GetUsers();
        if (response.Success)
        {
            Users = response.Data;
        }
        else
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
        userEditDto = new UserEditDto
        {
            Id = args.RowData.Id,
            UserName = args.RowData.UserName,
            Email = args.RowData.Email,
            FirstName = args.RowData.FirstName,
            LastName = args.RowData.LastName,
            EmailConfirmed = args.RowData.EmailConfirmed,
            Admin = args.RowData.Admin
        };
        IsUserModalVisible = true;
    }

    public async Task AddUserOnSubmit()
    {
        var response = await UserMoviesHttpRepo.UpdateUser(userEditDto);
        if (response.Success)
        {
            IsUserModalVisible = false;
            userEditDto = new();
            await ReloadGrid();
            toastContent = "User Updated Successfully";
            await ToastObj.ShowAsync();
        }
        else
        {
            toastContent = "Error Updating User";
            toastSuccess = "e-toast-danger";
            await ToastObj.ShowAsync();
        }
    }

    public void Reset()
    {
        userEditDto = new();
        IsUserModalVisible = false;
    }
}
