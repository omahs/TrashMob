﻿namespace TrashMob.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public class EventAttendeeRepository : IEventAttendeeRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventAttendeeRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<EventAttendee>> GetAllEventAttendees(Guid eventId)
        {
            return await mobDbContext.EventAttendees.Where(ea => ea.EventId == eventId).ToListAsync().ConfigureAwait(false);
        }

        public Task<int> AddEventAttendee(Guid eventId, Guid attendeeId)
        {
            var eventAttendee = new EventAttendee
            {
                EventId = eventId,
                UserId = attendeeId,
                SignUpDate = DateTimeOffset.UtcNow,
            };

            mobDbContext.EventAttendees.Add(eventAttendee);
            return mobDbContext.SaveChangesAsync();
        }

        public Task<int> UpdateEventAttendee(EventAttendee eventAttendee)
        {
            mobDbContext.Entry(eventAttendee).State = EntityState.Modified;
            return mobDbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteEventAttendee(Guid eventId, Guid userId)
        {
            var eventAttendee = await mobDbContext.EventAttendees.FindAsync(eventId, userId).ConfigureAwait(false);
            mobDbContext.EventAttendees.Remove(eventAttendee);
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid attendeeId)
        {
            // TODO: There are better ways to do this.
            var eventAttendees = await mobDbContext.EventAttendees.Where(ea => ea.UserId == attendeeId).ToListAsync().ConfigureAwait(false);

            var events = await mobDbContext.Events.Where(e => eventAttendees.Select(ea => ea.EventId).Contains(e.Id)).ToListAsync().ConfigureAwait(false);
            return events;
        }
    }
}
