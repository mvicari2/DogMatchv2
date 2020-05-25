using System;

namespace DogMatch.Server.Models
{
    public class Album
    {
        public int Id { get; set; }
        public int DogId { get; set; }
        public string Filename { get; set; }
        public byte[] Image { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public Guid AlbumGUID { get; set; }
        public virtual Dogs Dog { get; set; }
        public virtual DogMatchUser CreatedByUser { get; set; }
    }
}