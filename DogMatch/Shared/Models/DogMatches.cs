using System.Collections.Generic;

namespace DogMatch.Shared.Models
{
    public class DogMatches
    {
        public int DogId { get; set; }
        public string DogName { get; set; }
        public string OwnerName { get; set; }
        public bool CompletedProfile { get; set; }
        public List<Match> Matches { get; set; }
    }

    public class Match
    {
        public int DogId { get; set; }
        public string DogName { get; set; }
        public string Breed { get; set; }
        public string Gender { get; set; }
        public string Weight { get; set; }
        public string Age { get; set; }
        public string ProfileImage { get; set; }
        public string OwnerName { get; set; }
        public string OwnerId { get; set; }
    }
}
