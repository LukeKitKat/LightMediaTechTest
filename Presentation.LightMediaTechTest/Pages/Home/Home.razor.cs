using Microsoft.AspNetCore.Components;
using Presentation.LightMediaTechTest.Components.MainLayout;
using Presentation.LightMediaTechTest.Pages.Login.Models;
using Presentation.LightMediaTechTest.Pages.Login.Services;
using Server.LightMediaTechTest.DatabaseManager.Models;
using Server.LightMediaTechTest.EventManager;
using Server.LightMediaTechTest.UserManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.LightMediaTechTest.Pages.Home
{
    public partial class Home
    {
        [Inject]
        private UserManager UserManager { get; set; } = default!;
        [Inject]
        private EventManager EventManager { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private CookiesManager CookiesManager { get; set; } = default!;

        private MainLayout MainLayout { get; set; } = default!;

        private List<User> Users { get; set; } = new List<User>();
        private List<Event> EventsSource { get; set; } = new List<Event>();
        private List<Event> Events { get; set; } = new List<Event>();

        private User? CurrentUser { get; set; }
        private List<string> Catagories { get; set; } = new();
        private string NameFilter { get; set; } = string.Empty;
        private string CatagoryFilter { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await FetchUsersAsync();
            await FetchEventsAsync();
            await FetchCatagoriesAsync();

            if (!Users.Any())
            {
                var resp = await UserManager.GenerateExampleUserAsync();
                if (!resp.Success)
                    return;

                await FetchUsersAsync();
            }

            if (!EventsSource.Any())
            {
                var resp = await EventManager.GenerateTemplateEventsAsync();
                if (!resp.Success)
                    return;
                await FetchEventsAsync();
            }

            Filter();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            CurrentUser = null;

            if (int.TryParse(await CookiesManager.GetCookieAsync(LoginConstants.UserCookieId), out int result))
            {
                var resp = await UserManager.FetchUserByIdAsync(result);
                if (!resp.Success)
                    return;

                CurrentUser = resp.Result;
                StateHasChanged();
            }

            await base.OnAfterRenderAsync(firstRender);
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
            foreach(var catagory in resp.Result!)
                Catagories.Add(catagory.CatagoryName);
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
                Events = Events.Where(x => x.EventCatagory.CatagoryName.Contains(CatagoryFilter, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
        }
    }
}
