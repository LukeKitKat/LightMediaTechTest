using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Presentation.LightMediaTechTest.Components.PresentationPageBase;
using Presentation.LightMediaTechTest.Pages.Login.Models;
using Server.LightMediaTechTest.DatabaseContext.Models;
using Server.LightMediaTechTest.Services.UserManager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.LightMediaTechTest.Pages.Login
{
    public partial class Login : PresentationPageBase
    {
        [Inject]
        private UserManager UserManager { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private EditContext? EditContext { get; set; }
        private LoginModel LoginModel = new();

        private bool Registering;
        private User RegisteringUser = new();
        private List<UserRole> RegisteringRoles = [];
        private string RegisteringPassword = string.Empty;
        private string RegisteringConfirmPassword = string.Empty;
        private string? _registerUserOption;
        private string? RegisterUserOption
        {
            get => _registerUserOption;
            set
            {
                if (_registerUserOption != value)
                    _registerUserOption = value;

                RegisteringUser.UserRoleId = RegisteringRoles.First(x => x.RoleName == _registerUserOption).Id;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            EditContext = new(LoginModel);

            var resp = await UserManager.FetchAllUserRolesAsync();
            if (!resp.Success)
                return;

            RegisteringRoles = resp.Result!;
            RegisteringUser.UserRoleId = RegisteringRoles.First().Id;

            await base.OnInitializedAsync();
        }

        private async Task RegisterNewUserAsync()
        {
            if (RegisteringPassword == RegisteringConfirmPassword)
            {
                var resp = await UserManager.RegisterNewUserAsync(RegisteringUser, RegisteringPassword);
                if (!resp.Success)
                    return;

                ToggleRegisterState();
            }
        }

        private async Task LoginToExistingAccount()
        {
            var resp = await UserManager.ValidateUserLoginAsync(LoginModel.LoginUserName, LoginModel.LoginPassword);

            if (!resp.Success)
                return;

            if (resp.Result)
            {
                var userFetchResp = await UserManager.FetchUserByUsernameAsync(LoginModel.LoginUserName);
                if (!userFetchResp.Success)
                    return;

                await StoreCookieAsync(LoginConstants.UserCookieId, $"{userFetchResp.Result!.Id}");
                NavigationManager.NavigateTo("/", false, true);
            }
        }

        private void ToggleRegisterState()
        {
            Registering = !Registering;

            if (Registering)
                EditContext = new(RegisteringUser);
            else
                EditContext = new(LoginModel);

            StateHasChanged();
        }
    }
}
