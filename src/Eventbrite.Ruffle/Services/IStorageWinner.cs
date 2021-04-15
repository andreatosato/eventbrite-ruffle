using Eventbrite.Ruffle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventbrite.Ruffle.Services
{
    public interface IStorageWinner : IDisposable
    {
        void InsertWinner(AttendeeWinner attendeeWinner);
        IEnumerable<AttendeeWinner> GetWinners(double eventId);
        void DeleteWinner(double personId);
        void ClearAllByEventId(double eventId);
    }
}
