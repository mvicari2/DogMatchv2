using System;
using System.Collections.Generic;

namespace DogMatch.Server.Models
{
    public class Color
    {
        public int Id { get; set; }
        public int DogId { get; set; }
        public string ColorString { get; set; }
        public Guid ColorGUID { get; set; }
        public virtual Dogs Dog { get; set; }
    }
}