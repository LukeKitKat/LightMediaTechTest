using Microsoft.AspNetCore.Components;
using Presentation.LightMediaTechTest.Components.MainLayout;
using Presentation.LightMediaTechTest.Components.PresentationPageBase;
using Presentation.LightMediaTechTest.Pages.Login.Models;
using Server.LightMediaTechTest.DatabaseContext.Models;
using Server.LightMediaTechTest.Services.EventManager;
using Server.LightMediaTechTest.Services.UserManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.LightMediaTechTest.Pages.Home
{
    public partial class Home : PresentationPageBase
    {
        [Inject]
        private UserManager UserManager { get; set; } = default!;
        [Inject]
        private EventManager EventManager { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private List<User> Users { get; set; } = [];
        private List<Event> EventsSource { get; set; } = [];
        private List<Event> Events { get; set; } = [];

        private List<string> Catagories { get; set; } = [];
        private string NameFilter { get; set; } = string.Empty;
        private string CatagoryFilter { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await FetchUsersAsync();
            await FetchEventsAsync();
            await FetchCatagoriesAsync();

            if (Users.Count == 0)
            {
                var resp = await UserManager.GenerateExampleUserAsync();
                if (!resp.Success)
                    return;

                await FetchUsersAsync();
            }

            if (EventsSource.Count == 0)
            {
                var resp = await EventManager.GenerateTemplateEventsAsync();
                if (!resp.Success)
                    return;
                await FetchEventsAsync();
            }

            Filter();
        }

        private async Task FetchUsersAsync()
        {
            var resp = await UserManager.FetchAllUsersAsync();
            if (!resp.Success)
                return;

            Users = resp.Result!;
        }

        private async Task FetchEventsAsync()
        {
            var resp = await EventManager.FetchAllEventsAsync();
            if (!resp.Success)
                return;

            EventsSource = resp.Result!;
        }

        private async Task FetchCatagoriesAsync()
        {
            var resp = await EventManager.FetchAllEventCatagoriesAsync();
            if (!resp.Success)
                return;

            Catagories.Add("All");
            foreach (var catagory in resp.Result!)
            {
                if (catagory?.CatagoryName is null)
                    continue;

                Catagories.Add(catagory.CatagoryName);
            }
        }

        private void Filter()
        {
            Events = EventsSource;

            if (!string.IsNullOrEmpty(NameFilter))
            {
                Events = EventsSource.Where(x => x.EventName!.Contains(NameFilter, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            if (CatagoryFilter != "All")
            {
                Events = Events.Where(x => x.EventCatagory!.CatagoryName!.Contains(CatagoryFilter, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
        }
    }
}
