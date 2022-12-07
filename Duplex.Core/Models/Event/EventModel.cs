using Duplex.Infrastructure.Data.Models;

namespace Duplex.Core.Models.Event
{
    public class EventModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int EntryCost { get; set; }
        public int TeamSize { get; set; }
        public string ImageUrl { get; set; } = null!;
        public DateTime CreatedOnUTC { get; set; }
        public IEnumerable<EventUser> Participants { get; set; } = new HashSet<EventUser>();
    }
}
