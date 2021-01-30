using EventbriteApiV3.Attendees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventbrite.Ruffle.Models
{
    public class AttendeeWinner
    {
        [LiteDB.BsonId]
        public double Id { get; set; }
        public int Incremental { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }
        public Attendee Attendee { get; set; }
        public string RuffleName { get; set; }
        public double EventId { get; set; }

        public AttendeeWinner SetAttendee(Attendee a)
        {
            Id = a.Id;
            Name = a.Profile.Name;
            Email = a.Profile.Email;
            Firstname = a.Profile.Firstname;
            Lastname = a.Profile.Lastname;
            Gender = a.Profile.Gender;
            return this;
        }
    }
}
