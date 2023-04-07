﻿#nullable disable

namespace TrashMob.Models
{
    using System;

    public partial class EventAttendee : BaseModel
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public DateTimeOffset SignUpDate { get; set; }

        public DateTimeOffset? CanceledDate { get; set; }

        public virtual Event Event { get; set; }

        public virtual User User { get; set; }
    }
}
