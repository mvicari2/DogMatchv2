using System;

namespace DogMatch.Shared.Models
{
    public class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public DateTime? Birthday { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public int Weight { get; set; }
        public string ProfileImage { get; set; }
        public string Extension { get; set; }
        public string Owner { get; set; }

        
    }
}
