using Eventbrite.Ruffle.Configs;
using Eventbrite.Ruffle.Models;
using LiteDB;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eventbrite.Ruffle.Services
{
    public class StorageWinnerAttendee : IStorageWinner
    {
        private readonly StorageConfig options;
        private Lazy<LiteDatabase> Database { get; set; }

        public StorageWinnerAttendee(IOptions<StorageConfig> options)
        {
            this.options = options.Value;
            Database = new Lazy<LiteDatabase>(() => new LiteDatabase(this.options.FilePath));
        }

        public void InsertWinner(AttendeeWinner attendeeWinner)
        {
            var db = Database.Value;
            var col = db.GetCollection<AttendeeWinner>(options.CollectionName);
            attendeeWinner.Incremental = col.Count();
            col.Insert(attendeeWinner);
            col.EnsureIndex(x => x.Id);
            db.Checkpoint();
        }

        public IEnumerable<AttendeeWinner> GetWinners(double eventId)
        {
            var db = Database.Value;
            var col = db.GetCollection<AttendeeWinner>(options.CollectionName);
            return col.Find(x => x.EventId == eventId);
        }

        public void DeleteWinner(double personId)
        {
            var db = Database.Value;
            var col = db.GetCollection<AttendeeWinner>(options.CollectionName);
            col.Delete(personId);
            db.Checkpoint();
        }

        public void ClearAllByEventId(double eventId)
        {
            var db = Database.Value;
            var col = db.GetCollection<AttendeeWinner>(options.CollectionName);
            col.DeleteMany(t => t.EventId == eventId);
            db.Checkpoint();
        }
    }
}
