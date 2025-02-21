using Microsoft.AspNetCore.Components;
using Presentation.LightMediaTechTest.Pages.Login.Models;
using Presentation.LightMediaTechTest.Components.PresentationPageBase;
using Server.LightMediaTechTest.Services.UserManager;
using Server.LightMediaTechTest.Services.EventManager;
using Server.LightMediaTechTest.DatabaseContext.Models;

namespace Presentation.LightMediaTechTest.Pages.EventPage
{
    public partial class EventPage : PresentationPageBase
    {
        // Used for routing
        [Parameter]
        public int EventId { get; set; }

        [Inject]
        private EventManager EventManager { get; set; } = default!;
        [Inject]
        private UserManager UserManager { get; set; } = default!;

        private Event Event { get; set; } = new();
        private bool AlreadyApplied { get; set; }
        private bool RecentlyApplied { get; set; }

        public EventPage() =>
            OnAfterRenderAsyncOverloadEvent += (sender, args) => { return CheckForExistingApplicationAsync(); };

        protected override async Task OnInitializedAsync()
        {
            var resp = await EventManager.FetchEventByIdAsync(EventId);
            if (!resp.Success)
                return;

            Event = resp.Result!;

            await base.OnInitializedAsync();
        }

        private async Task AttendEventClicked(bool addToEvent)
        {
            if (CurrentUser is null)
                return;

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
            if (CurrentUser is null)
                return;

            var resp = await EventManager.CheckForExistingApplicationAsync(EventId, CurrentUser.Id);
            if (resp.Success)
                AlreadyApplied = resp.Result;
        }
    }
}
