using System;

namespace DogMatch.Domain.Data.Models
{
    public class Temperament
    {
        public int Id { get; set; }
        public int? DogId { get; set; }
        public int? Empathetic { get; set; }
        public int? Anxiety { get; set; }
        public int? Fearful { get; set; }
        public int? IsAfraidFireworks { get; set; }
        public int? FriendlinessOverall { get; set; }
        public int? GoodWithPeople { get; set; }
        public int? GoodWithOtherDogs { get; set; }
        public int? GoodWithCats { get; set; }
        public int? GoodWithOtherAnimals { get; set; }
        public int? GoodWithChildren { get; set; }
        public int? Playfulness { get; set; }
        public int? LikesPlayingHumans { get; set; }
        public int? LikesPlayingDogs { get; set; }
        public int? PlaysFetch { get; set; }
        public int? LikesToys { get; set; }
        public int? LikesTreats { get; set; }
        public int? AthleticLevel { get; set; }
        public int? LikesExcersize { get; set; }
        public int? TrainingLevel { get; set; }
        public int? Trainability { get; set; }
        public int? Stubbornness { get; set; }
        public int? Intelligence { get; set; }
        public int? SenseOfSmell { get; set; }
        public int? PreyDrive { get; set; }
        public int? AggressionLevel { get; set; }
        public int? Protectiveness { get; set; }
        public int? DistinguishThreatening { get; set; }
        public int? BalanceStability { get; set; }
        public int? Confidence { get; set; }
        public int? IsPickyEater { get; set; }
        public int? Shedding { get; set; }
        public int? Barking { get; set; }
        public int? SmellRating { get; set; }
        public bool? HairOrFur { get; set; }
        public bool? Housebroken { get; set; }
        public bool? OutsideOrInside { get; set; }
        public bool? IsFixed { get; set; }
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string LastModifiedBy { get; set; }
        public Guid TemperamentGUID { get; set; }
        public virtual Dogs Dog { get; set; }
        public virtual DogMatchUser CreatedByUser { get; set; }
        public virtual DogMatchUser ModifiedByUser { get; set; }
    }
}