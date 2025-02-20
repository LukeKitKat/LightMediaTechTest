using Microsoft.AspNetCore.Components;
using Presentation.LightMediaTechTest.Pages.Login.Models;
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
        public User? CurrentUser { get; set; }
        [Parameter]
        public Event Event { get; set; } = new Event();
        [Parameter]
        public bool DisplayBookRedirect { get; set; } = true;
        [Parameter]
        public bool DisplayCreatedDate { get; set; } = true;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        /// <summary>
        /// Navigates to a specific event.
        /// </summary>
        /// <param name="eventId">The ID of the event.</param>
        private void BookingClicked(int eventId) =>
            NavigationManager.NavigateTo($"/event/{eventId}");
    }
}
