using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Presentation.LightMediaTechTest.Pages.Login.Models;
using Server.LightMediaTechTest.DatabaseManager.Models;
using Server.LightMediaTechTest.EventManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.LightMediaTechTest.Pages.EventManagementPage
{
    public partial class EventManagement
    {
        [Parameter]
        public int EventId { get; set; }

        [Inject]
        private EventManager EventManager { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private List<EventCatagory> EventCatagories { get; set; } = new();
        private Event Event { get; set; } = new Event();
        private EditContext? EditContext { get; set; }

        private string? _eventCatagoryOption;
        private string? EventCatagoryOption
        {
            get => _eventCatagoryOption;
            set
            {
                if (_eventCatagoryOption != value)
                    _eventCatagoryOption = value;

                Event.EventCatagoryId = EventCatagories.First(x => x.CatagoryName == _eventCatagoryOption).Id;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            EditContext = new(new Event());

            if (Event.Id != 0)
            {
                var resp = await EventManager.FetchEventByIdAsync(EventId);
                if (!resp.Success)
                    return;

                Event = resp.Result!;
            }
            else
            {
                Event.EventCatagoryId = EventCatagories.First().Id;
            }

            EditContext = new(Event);

            var catagoriesResp = await EventManager.FetchAllEventCatagoriesAsync();
            if (!catagoriesResp.Success)
                return;

            EventCatagories = catagoriesResp.Result!;

            await base.OnInitializedAsync();
        }

        private async Task OnEventDetailsSubmit()
        {
            bool updating = true;
            if (Event.Id == 0)
            {
                updating = false;
            }

            var resp = await EventManager.AddOrUpdateEventAsync(Event, updating);
            if (resp.Success)
                NavigationManager.NavigateTo("/", false, true);
        }

        private async Task DeleteEventClicked()
        {
            var resp = await EventManager.DeleteExistingEventAsync(EventId);
            if (resp.Success)
                NavigationManager.NavigateTo("/", false, true);
        }
    }
}
