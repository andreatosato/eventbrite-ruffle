using Eventbrite.Ruffle.Configs;
using Eventbrite.Ruffle.Models;
using EventbriteApiV3;
using EventbriteApiV3.Attendees;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventbrite.Ruffle.Services
{
    public class EventbriteService
    {
        private readonly EventbriteConfig eventbriteConfig;
        private List<Attendee> cacheAttendee = new List<Attendee>();
        private readonly Random random = new Random();
        private readonly IStorageWinner storageWinner;

        public EventbriteService(IOptions<EventbriteConfig> options, IStorageWinner storageWinner)
        {
            eventbriteConfig = options.Value;
            this.storageWinner = storageWinner;
        }

        public List<Attendee> GetAttendees(double eventId)
        {
            if (cacheAttendee.Count == 0)
            {
                var eventbriteContext = new EventbriteContext(eventbriteConfig.AppKey);
                var criterias = new AttendeeSearchCriterias()
                                .Status(AttendeeSearchCriterias.AttendeeStatus.Attending);

                var result = eventbriteContext.GetAttendees(eventId, criterias);
                cacheAttendee.AddRange(result.Attendees);
                for (int i = 2; i <= result.Pagination.PageCount; i++)
                {
                    criterias = new AttendeeSearchCriterias()
                                .Status(AttendeeSearchCriterias.AttendeeStatus.Attending)
                                .Page(i);
                    result = eventbriteContext.GetAttendees(eventId, criterias);
                    cacheAttendee.AddRange(result.Attendees);
                }
            }
            return cacheAttendee;
        }

        public AttendeeWinner RandomizeAttendeeAndSave(double eventId, string ruffleName)
        {
            var attendee = GetAttendees(eventId);
            bool find = false;
            Attendee winner;
            var winners = storageWinner.GetWinners(eventId).ToList();
            do
            {
                int position = random.Next(0, attendee.Count);
                winner = attendee.ElementAt(position);
                if (!winners.Any(t => t.Id == winner.Id))
                    find = true;
            } while (!find);

            var attendeeWinner = new AttendeeWinner
            {
                Attendee = winner,
                EventId = eventbriteConfig.EventId,
                RuffleName = ruffleName
            }
            .SetAttendee(winner);
            storageWinner.InsertWinner(attendeeWinner);

            return attendeeWinner;
        }

        public void DeleteWinner(double personId)
        {
            storageWinner.DeleteWinner(personId);
        }

        public void ClearEvent(double personId)
        {
            storageWinner.ClearAllByEventId(personId);
        }

        public IList<AttendeeWinner> GetWinners(double eventId)
        {
            return storageWinner.GetWinners(eventId).OrderBy(t => t.Incremental).ToList();
        }
    }
}
