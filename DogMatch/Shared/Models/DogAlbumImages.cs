using System.Collections.Generic;

namespace DogMatch.Shared.Models
{
    public class DogAlbumImages
    {
        public int DogId { get; set; }
        public string DogName { get; set; }
        public List<AlbumImage> Images { get; set; }
    }

    public class AlbumImage
    {
        public int Id { get; set; }
        public string ImageString { get; set; }
        public string Extension { get; set; }
        public bool? Delete { get; set; }
    }
}
