using System.Formats.Tar;
using System.Text.Json.Serialization;

namespace Domain.Models.Deck
{
    public class CourseDeck
    {
        public List<CardEntry> MainDeck { get; set; }
        public List<CardEntry> Sideboard { get; set; }
        public List<CardEntry> ReducedSideboard { get; set; }
        public List<CardEntry> CommandZone { get; set; }
        public List<CardEntry> Companions { get; set; }
        public List<CardSkin> CardSkins { get; set; }
    }
}