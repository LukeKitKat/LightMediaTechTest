using Microsoft.AspNetCore.Components;
using Presentation.LightMediaTechTest.Pages.Login.Models;
using Presentation.LightMediaTechTest.Pages.Login.Services;
using Server.LightMediaTechTest.DatabaseManager.Models;
using Server.LightMediaTechTest.UserManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.LightMediaTechTest.Components.EventComponent
{
    public partial class EventComponent
    {
        [Parameter]
        public Event Event { get; set; } = new Event();
        [Parameter]
        public bool CanBookNow { get; set; } = true;
        [Parameter]
        public bool DisplayCreatedDate { get; set; } = true;

        private bool CanManage { get; set; }
        private bool HideBooking { get; set; }

        [Inject]
        private UserManager UserManager { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private CookiesManager CookiesManager { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (int.TryParse(await CookiesManager.GetCookieAsync(LoginConstants.UserCookieId), out int result))
            {
                var resp = await UserManager.FetchUserRoleByUserIdAsync(result);
                if (!resp.Success)
                    return;

                CanManage = resp.Result!.CanAmendEvents;
                HideBooking = false;
            }
            else
            {
                HideBooking = true;
            }

            StateHasChanged();

            await base.OnAfterRenderAsync(firstRender);
        }

        private void BookingClicked(int eventId) =>
            NavigationManager.NavigateTo($"/event/{eventId}");
    }
}
