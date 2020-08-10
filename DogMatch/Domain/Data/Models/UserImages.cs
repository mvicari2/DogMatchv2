using System;

namespace DogMatch.Domain.Data.Models
{
    public class UserImages
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool? IsProfileImage { get; set; }
        public string Filename { get; set; }        
        public bool? IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public Guid UserImageGUID { get; set; }
        public virtual DogMatchUser ProfileImageUser { get; set; }
        public virtual DogMatchUser User { get; set; }
        public virtual DogMatchUser CreatedByUser { get; set; }
    }
}