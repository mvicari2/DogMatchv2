namespace DogMatch.Shared.Models
{
    public class DogBiography
    {
        public int Id { get; set; }
        public int DogId { get; set; }
        public string DogName { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string AboutDoggo { get; set; }
        public string FavoriteMemory { get; set; }
        public string FavoriteFoods { get; set; }
        public string FavoriteToy { get; set; }
        public string FavoriteSleepLocation { get; set; }
        public string FavoriteWalkLocation { get; set; }
    }
}
