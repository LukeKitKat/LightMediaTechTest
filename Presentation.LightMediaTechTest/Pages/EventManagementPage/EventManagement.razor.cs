using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Presentation.LightMediaTechTest.Components.PresentationPageBase;
using Presentation.LightMediaTechTest.Pages.Login.Models;
using Server.LightMediaTechTest.DatabaseContext.Models;
using Server.LightMediaTechTest.Services.EventManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.LightMediaTechTest.Pages.EventManagementPage
{
    public partial class EventManagement : PresentationPageBase
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

            var catagoriesResp = await EventManager.FetchAllEventCatagoriesAsync();
            if (!catagoriesResp.Success)
                return;

            EventCatagories = catagoriesResp.Result!;

            var resp = await EventManager.FetchEventByIdAsync(EventId);
            if (resp.Success)
                Event = resp.Result ?? new();

            if (Event.Id == 0)
            {
                Event.EventCatagoryId = EventCatagories.First().Id;
            }

            EditContext = new(Event);

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

        private async Task ExportAttendeesToCSVAsync()
        {
            var fileName = $"EventAttendees-{DateTime.Now.ToShortDateString().Replace("/", "")}.csv";
            var fileContent = new StringBuilder("Attendee,Email\n");

            foreach (var attendee in Event.EventUsers)
            {
                fileContent.AppendLine($"{attendee.User?.DisplayName},{attendee.User?.Email}\n");
            }

            await CreateAndDownloadFileAsync(fileName, fileContent.ToString());
        }
    }
}
