using System;

namespace DogMatch.Server.Models
{
    public class Biography
    {
        public int Id { get; set; }
        public int DogId { get; set; }
        public string AboutDoggo { get; set; }
        public string FavoriteMemory { get; set; }
        public string FavoriteFoods { get; set; }
        public string FavoriteToy { get; set; }
        public string FavoriteSleepLocation { get; set; }
        public string FavoriteWalkLocation { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }
        public Guid BiographyGUID { get; set; }
        public virtual Dogs Dog { get; set; }
        public virtual DogMatchUser CreatedByUser { get; set; }
        public virtual DogMatchUser ModifiedByUser { get; set; }
    }
}