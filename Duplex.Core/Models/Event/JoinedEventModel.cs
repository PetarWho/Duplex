using Duplex.Infrastructure.Data.Models;

namespace Duplex.Core.Models.Event
{
    public class JoinedEventModel
    {
        public Guid EventId { get; set; }
        public string EventName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int EntryCost { get; set; }
        public string EventImageUrl { get; set; } = null!;
        public DateTime CreatedOnUTC { get; set; }
        public string UserName { get; set; } = null!;
        public ICollection<EventUser> Participants { get; set; } = new HashSet<EventUser>();
    }
}
