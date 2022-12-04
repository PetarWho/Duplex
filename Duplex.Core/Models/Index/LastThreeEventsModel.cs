using Duplex.Core.Models.Event;

namespace Duplex.Core.Models.Index
{
    public class LastThreeEventsModel
    {
        public IEnumerable<EventModel> Events { get; set; } = new List<EventModel>();
    }
}
