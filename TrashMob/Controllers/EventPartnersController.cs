﻿
namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Poco;
    using TrashMob.Shared;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/eventpartners")]
    public class EventPartnersController : ControllerBase
    {
        private readonly IEventPartnerRepository eventPartnerRepository;
        private readonly IUserRepository userRepository;

        public EventPartnersController(IEventPartnerRepository eventPartnerRepository, IUserRepository userRepository)
        {
            this.eventPartnerRepository = eventPartnerRepository;
            this.userRepository = userRepository;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventPartners(Guid eventId)
        {
            var displayEventPartners = new List<DisplayEventPartner>();
            var currentPartners = await eventPartnerRepository.GetEventPartners(eventId).ConfigureAwait(false);
            var possiblePartners = await eventPartnerRepository.GetPotentialEventPartners(eventId).ConfigureAwait(false);

            // Convert the current list of partners for the event to a display partner (reduces round trips)
            foreach (var cp in currentPartners)
            {
                var displayEventPartner = new DisplayEventPartner
                {
                    EventId = eventId,
                    PartnerId = cp.PartnerId,
                    PartnerLocationId = cp.PartnerLocationId,
                    EventPartnerStatusId = cp.EventPartnerStatusId,
                    PartnerLocationName = cp.PartnerLocation.Name,
                    PartnerLocationNotes = cp.PartnerLocation.Notes,
                    PartnerName = cp.Partner.Name,
                };

                displayEventPartners.Add(displayEventPartner);
            }

            // Convert the current list of possible partners for the event to a display partner unless the partner location is already included (reduces round trips)
            foreach (var pp in possiblePartners)
            {
                if (!displayEventPartners.Any(ep => ep.PartnerLocationId == pp.Id))
                {
                    var displayEventPartner = new DisplayEventPartner
                    {
                        EventId = eventId,
                        PartnerId = pp.PartnerId,
                        PartnerLocationId = pp.Id,
                        EventPartnerStatusId = (int)EventPartnerStatusEnum.None,
                        PartnerLocationName = pp.Name,
                        PartnerLocationNotes = pp.Notes,
                        PartnerName = pp.Partner.Name,
                    };

                    displayEventPartners.Add(displayEventPartner);
                }
            }

            return Ok(displayEventPartners);
        }

        [HttpPut("{id}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PutEventPartner(EventPartner eventPartner)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);
            if (currentUser == null || !ValidateUser(currentUser.NameIdentifier))
            {
                return Forbid();
            }

            var updatedEventPartner = await eventPartnerRepository.UpdateEventPartner(eventPartner).ConfigureAwait(false);
            return Ok(updatedEventPartner);
        }

        [HttpPost]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PostEventPartner(EventPartner eventPartner)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);
            if (currentUser == null || !ValidateUser(currentUser.NameIdentifier))
            {
                return Forbid();
            }

            eventPartner.CreatedByUserId = currentUser.Id;
            eventPartner.LastUpdatedByUserId = currentUser.Id;
            eventPartner.CreatedDate = DateTimeOffset.UtcNow;
            eventPartner.LastUpdatedDate = DateTimeOffset.UtcNow;
            await eventPartnerRepository.AddEventPartner(eventPartner).ConfigureAwait(false);

            return Ok();
        }

        // Ensure the user calling in is the owner of the record
        private bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}
