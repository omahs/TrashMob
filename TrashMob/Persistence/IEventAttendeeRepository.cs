﻿namespace TrashMob.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventAttendeeRepository
    {
        Task<IEnumerable<EventAttendee>> GetAllEventAttendees(Guid eventId);

        Task<int> AddEventAttendee(Guid eventId, Guid attendeeId);

        Task<int> UpdateEventAttendee(EventAttendee eventAttendee);

        Task<int> DeleteEventAttendee(Guid eventId, Guid attendeeId);

        Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid attendeeId);
    }
}
