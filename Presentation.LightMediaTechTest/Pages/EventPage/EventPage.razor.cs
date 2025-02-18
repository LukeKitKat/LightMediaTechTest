using Microsoft.AspNetCore.Components;
using Server.LightMediaTechTest.EventManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.LightMediaTechTest.DatabaseManager.Models;
using Presentation.LightMediaTechTest.Pages.Login.Models;
using Presentation.LightMediaTechTest.Pages.Login.Services;
using Server.LightMediaTechTest.UserManager;

namespace Presentation.LightMediaTechTest.Pages.EventPage
{
    public partial class EventPage
    {
        // Used for routing
        [Parameter]
        public int EventId { get; set; }

        [Inject]
        private EventManager EventManager { get; set; } = default!;
        [Inject]
        private UserManager UserManager { get; set; } = default!;
        [Inject]
        private CookiesManager CookiesManager { get; set; } = default!;

        private User CurrentUser { get; set; } = new User();
        private Event Event { get; set; } = new Event();
        private bool AlreadyApplied { get; set; }
        private bool RecentlyApplied { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var resp = await EventManager.FetchEventByIdAsync(EventId);
            if (!resp.Success)
                return;

            Event = resp.Result!;

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (int.TryParse(await CookiesManager.GetCookieAsync(LoginConstants.UserCookieId), out int result))
            {
                var resp = await UserManager.FetchUserByIdAsync(result);
                if (resp.Success)
                    CurrentUser = resp.Result!;
            }

            await CheckForExistingApplicationAsync();
            StateHasChanged();
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task AttendEventClicked(bool addToEvent)
        {
            var resp = await EventManager.UpdateUserEventStatusAsync(EventId, CurrentUser.Id, addToEvent);
            if (!resp.Success)
                return;

            var userResp = await UserManager.UpdateUserDetails(CurrentUser);
            if (!userResp.Success)
                return;

            RecentlyApplied = true;

            await CheckForExistingApplicationAsync();
            StateHasChanged();
        }

        private async Task CheckForExistingApplicationAsync()
        {
            var resp = await EventManager.CheckForExistingApplicationAsync(EventId, CurrentUser.Id);
            if (resp.Success)
                AlreadyApplied = resp.Result;
        }
    }
}
