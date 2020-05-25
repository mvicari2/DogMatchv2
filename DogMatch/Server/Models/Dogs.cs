using System;
using System.Collections.Generic;

namespace DogMatch.Server.Models
{
    public class Dogs
    {
        public Dogs()
        {
            Colors = new HashSet<Color>();
            AlbumImages = new HashSet<Album>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public DateTime? Birthday { get; set; }
        public char Gender { get; set; }
        public int Weight { get; set; }
        public byte[] ProfileImage { get; set; }
        public int? TemperamentId { get; set; }
        public int? BiographyId { get; set; }
        public string OwnerId { get; set; }
        public int? AddressId { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string LastModifiedBy { get; set; }
        public Guid DogGUID { get; set; }
        public virtual Biography Biography { get; set; }
        public virtual Temperament Temperament { get; set; }
        public virtual DogMatchUser Owner { get; set; }
        public virtual DogMatchUser CreatedByUser { get; set; }
        public virtual DogMatchUser ModifiedByUser { get; set; }
        public virtual Addresses Address { get; set; }        
        public virtual ICollection<Color> Colors { get; set; }
        public virtual ICollection<Album> AlbumImages { get; set; }
    }
}