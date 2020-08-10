using System;

namespace DogMatch.Domain.Data.Models
{
    public class DogImages
    {
        public int Id { get; set; }
        public int? DogId { get; set; }
        public string Filename { get; set; }
        public bool? IsProfileImage { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public Guid DogImageGUID { get; set; }
        public virtual Dogs Dog { get; set; }
        public virtual Dogs ProfileImageDog { get; set; }
        public virtual DogMatchUser CreatedByUser { get; set; }
    }
}