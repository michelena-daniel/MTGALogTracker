namespace Domain.Models.Deck
{
    public class EventSetDeckV2Dto
    {
        public string CourseId { get; set; }
        public string InternalEventName { get; set; }
        public string CurrentModule { get; set; }
        public string ModulePayload { get; set; }
        public CourseDeckSummary CourseDeckSummary { get; set; }
        public CourseDeck CourseDeck { get; set; }
    }
}
