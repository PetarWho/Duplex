using Duplex.Infrastructure.Data.Models;

namespace Duplex.Core.Models.Event
{
    public class EventModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int EntryCost { get; set; }
        public string ImageUrl { get; set; } = null!;
        public DateTime CreatedOnUTC { get; set; }
        public bool IsDone { get; set; } = false;
        public IEnumerable<EventUser> Participants { get; set; } = new HashSet<EventUser>();
    }
}
