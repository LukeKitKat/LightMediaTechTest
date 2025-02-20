using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.LightMediaTechTest.DatabaseManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.LightMediaTechTest.EventManager
{
    public class EventManager : ServiceBase
    {
        /// <summary>
        /// A basic task just to generate three example events for the sake of this Test.
        /// </summary>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse> GenerateTemplateEventsAsync()
        {
            return await ExecAsync(async (db, resp) =>
            {
                var funRunId = (await db.EventCatagories.AsNoTracking().FirstAsync(x => x.CatagoryName == "Fun-Run")).Id;
                var bakeOffId = (await db.EventCatagories.AsNoTracking().FirstAsync(x => x.CatagoryName == "Bake-Off")).Id;

                await db.Events.AddRangeAsync(new List<Event>()
                {
                    new Event()
                    {
                        EventName = "5k Fun-Run",
                        EventShortDescription = "Lorum Ipsum",
                        EventFullDescription = "Lorem ipsum dolor sit amet. Ut repellat reiciendis qui culpa deserunt ad debitis sequi. Id similique exercitationem ut similique recusandae et odit mollitia et necessitatibus deleniti quo asperiores consectetur.",
                        EventCatagoryId = funRunId,
                        EventLocation="10 Coder Avenue, England",
                        EventDateTime = DateTime.Now.AddDays(3),
                        PublishedDateTime = DateTime.UtcNow,
                        EventPicture = "Placeholder.png",
                        AcceptingBookings = true,
                    },

                    new Event()
                    {
                        EventName = "10k Fun-Run",
                        EventShortDescription = "Lorum Ipsum",
                        EventFullDescription = "Lorem ipsum dolor sit amet. Ut repellat reiciendis qui culpa deserunt ad debitis sequi. Id similique exercitationem ut similique recusandae et odit mollitia et necessitatibus deleniti quo asperiores consectetur.",
                        EventCatagoryId = funRunId,
                        EventLocation="23 Silicon Way, Wales",
                        EventDateTime = DateTime.Now.AddDays(5),
                        PublishedDateTime = DateTime.UtcNow,
                        EventPicture = "Placeholder.png",
                        AcceptingBookings = true,
                    },

                    new Event()
                    {
                        EventName = "Bake-Off",
                        EventShortDescription = "Lorum Ipsum",
                        EventFullDescription = "Lorem ipsum dolor sit amet. Ut repellat reiciendis qui culpa deserunt ad debitis sequi. Id similique exercitationem ut similique recusandae et odit mollitia et necessitatibus deleniti quo asperiores consectetur.",
                        EventCatagoryId = bakeOffId,
                        EventLocation="2 Ascii Close, England",
                        EventDateTime = DateTime.Now.AddDays(7),
                        PublishedDateTime = DateTime.UtcNow,
                        EventPicture = "Placeholder.png",
                        AcceptingBookings = true,
                    },
                });

                await db.SaveChangesAsync();
            });
        }

        /// <summary>
        /// Fetches every event in the database and returns them within the service response.
        /// </summary>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse<List<Event>>> FetchAllEventsAsync()
        {
            return await ExecAsync<List<Event>>(async (db, resp) =>
            {
                return await db.Events.AsNoTracking()
                                      .Include(x => x.EventCatagory)
                                      .OrderBy(x => x.EventDateTime)
                                      .ToListAsync();
            });
        }

        /// <summary>
        /// Fetches every event catagory and returns them within the service response.
        /// </summary>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse<List<EventCatagory>>> FetchAllEventCatagoriesAsync()
        {
            return await ExecAsync<List<EventCatagory>>(async (db, resp) =>
            {
                return await db.EventCatagories.AsNoTracking()
                                               .ToListAsync();
            });
        }

        /// <summary>
        /// Fetches a specific event by it's ID and returns it within the service response.
        /// </summary>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse<Event?>> FetchEventByIdAsync(int eventId)
        {
            return await ExecAsync<Event?>(async (db, resp) =>
            {
                return await db.Events.AsNoTracking()
                                      .Include(x => x.EventCatagory)
                                      .Include(x => x.EventUsers)
                                        .ThenInclude(x => x.User)
                                      .OrderBy(x => x.EventDateTime)
                                      .FirstOrDefaultAsync(x => x.Id == eventId);
            });
        }

        /// <summary>
        /// Adds or updates an event using the data in a given model.
        /// </summary>
        /// <param name="event">The event model containing the new or updated data.</param>
        /// <param name="updating">a condition stating whether the data is to be added as new or updated.</param>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse> AddOrUpdateEventAsync(Event @event, bool updating)
        {
            return await ExecAsync(async (db, resp) =>
            {
                if (updating)
                {
                    db.Events.Update(@event);
                }
                else
                {
                    @event.PublishedDateTime = DateTime.UtcNow;
                    await db.Events.AddAsync(@event);
                }
                
                await db.SaveChangesAsync();
            });
        }

        /// <summary>
        /// Deletes an existing event from the database using it's ID.
        /// </summary>
        /// <param name="eventId">The ID of the event to be deleted.</param>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse> DeleteExistingEventAsync(int eventId)
        {
            return await ExecAsync(async (db, resp) =>
            {
                db.Events.Remove(db.Events.AsNoTracking().First(x => x.Id == eventId));
                await db.SaveChangesAsync();
            });
        }

        /// <summary>
        /// Updates the relational table that determines which users have signed up to which events.
        /// </summary>
        /// <param name="eventId">The ID of the event being attended.</param>
        /// <param name="userId">The ID of the user being marked as an attendee.</param>
        /// <param name="addToEvent">A condition stating whether the user is being marked as an attendee or removed from attendance.</param>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse> UpdateUserEventStatusAsync(int eventId, int userId, bool addToEvent)
        {
            return await ExecAsync(async (db, resp) =>
            {
                if (await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId) is null)
                {
                    resp.Errors.Add("User not found");
                    return;
                }

                if (await db.Events.AsNoTracking().Include(x => x.EventUsers).FirstOrDefaultAsync(x => x.Id == eventId) is null)
                {
                    resp.Errors.Add("Event not found");
                    return;
                }

                if (addToEvent)
                    await db.EventAttendees.AddAsync(new() { UserId = userId, EventId = eventId });
                else
                    db.EventAttendees.Remove(db.EventAttendees.First(x => x.UserId == userId && x.EventId == eventId));

                await db.SaveChangesAsync();
            });
        }

        /// <summary>
        /// Checks to see if a specific user is already attending an event.
        /// </summary>
        /// <param name="eventId">The ID of the event being checked.</param>
        /// <param name="userId">The ID of the user being checked.</param>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse<bool>> CheckForExistingApplicationAsync(int eventId, int userId)
        {
            return await ExecAsync<bool>(async (db, resp) =>
            {
                return await db.Events.AsNoTracking().Include(x => x.EventUsers).Where(x => x.Id == eventId).AnyAsync(x => x.EventUsers.Any(x => x.UserId == userId));
            });
        }
    }
}
