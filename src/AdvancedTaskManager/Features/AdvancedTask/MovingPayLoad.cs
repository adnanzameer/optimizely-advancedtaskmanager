using EPiServer.Core;

namespace AdvancedTaskManager.Features.AdvancedTask
{
    public class MovingPayLoad
    {
        public ContentReference Source { get; set; }

        public ContentReference Destination { get; set; }
    }
}
