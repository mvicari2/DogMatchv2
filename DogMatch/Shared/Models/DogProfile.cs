﻿using DogMatch.Shared.Globals;
using System.Collections.Generic;

namespace DogMatch.Shared.Models
{
    public class DogProfile
    {
        public Dog Dog { get; set; }        
        public DogBiography Bio { get; set; }
        public List<AlbumImage> AlbumImages { get; set; }
        public IEnumerable<TemperamentScore> TemperamentScores { get; set; }
        public bool HasBio { get; set; }
        public bool HasTemperament { get; set; }
    }

    public class TemperamentScore
    {
        public string ScoreLabel { get; set; }
        public int ScoreValue { get; set; }
        public TemperamentScoreTypes ScoreType { get; set; }
    }
}
