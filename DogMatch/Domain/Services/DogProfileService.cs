using AutoMapper;
using DogMatch.Domain.Data.Models;
using DogMatch.Domain.Data.Repositories;
using DogMatch.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DogMatch.Domain.Services
{
    public class DogProfileService : IDogProfileService
    {
        #region DI
        private readonly IDogRepository _dogRepository;
        private readonly IMapper _mapper;

        public DogProfileService(IDogRepository dogRepository, IMapper mapper)
        {
            _dogRepository = dogRepository;
            _mapper = mapper;
        }
        #endregion DI

        #region Public Service Methods
        /// <summary>
        /// Get's <see cref="DogProfile"/> object for single dog, including basic details, 
        /// biography, album images, and generated temperament scores
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns>Single <see cref="DogProfile"/> object</returns>
        public async Task<DogProfile> GetDogProfile(int id)
        {
            Dogs dog = await _dogRepository.FindFullDogProfileById(id);

            // if dog doesn't exist, return null
            if (dog == null)
                return null;

            // initialize dog profile and map dog's basic details and album images
            DogProfile dogProfile = new DogProfile()
            {
                Dog = _mapper.Map<Dog>(dog),
                AlbumImages = _mapper.Map<List<AlbumImage>>(dog.AlbumImages)
            };

            // if all temperament values are populated (not null or zero), generate and set temperament scores
            if (HasCompletedTemperament(dog.Temperament))
            {
                dogProfile.HasTemperament = true;
                dogProfile.TemperamentScores = GetTemperamentScores(dog.Temperament);
            }

            // if any biography properties are populated, map and set values
            if (HasBiography(dog.Biography))
            {
                dogProfile.HasBio = true;
                dogProfile.Bio = _mapper.Map<DogBiography>(dog.Biography);
            }

            return dogProfile;
        }
        #endregion Public Service Methods

        #region Internal Methods
        /// <summary>
        /// Evaluates all rating values in single dog <see cref="Temperament"/> object to confirm if all <see cref="int"/> properties are populated (not null or zero)
        /// </summary>
        /// <param name="t"><see cref="Temperament"/> object with values to evaluate</param>
        /// <returns><see cref="bool"/>, true if all temperament ratings values are not null or zero</returns>
        private bool HasCompletedTemperament(Temperament t)
        {
            if (t == null)
                return false;
            else if (IntHasNonZeroValue(t.Anxiety) && 
                    IntHasNonZeroValue(t.Fearful) && 
                    IntHasNonZeroValue(t.IsAfraidFireworks) && 
                    IntHasNonZeroValue(t.FriendlinessOverall) &&
                    IntHasNonZeroValue(t.GoodWithPeople) && 
                    IntHasNonZeroValue(t.GoodWithOtherDogs) &&
                    IntHasNonZeroValue(t.GoodWithCats) &&
                    IntHasNonZeroValue(t.GoodWithOtherAnimals) &&
                    IntHasNonZeroValue(t.GoodWithChildren) &&
                    IntHasNonZeroValue(t.Playfulness) &&
                    IntHasNonZeroValue(t.LikesPlayingHumans) &&
                    IntHasNonZeroValue(t.LikesPlayingDogs) &&
                    IntHasNonZeroValue(t.PlaysFetch) &&
                    IntHasNonZeroValue(t.LikesToys) &&
                    IntHasNonZeroValue(t.LikesTreats) &&
                    IntHasNonZeroValue(t.AthleticLevel) &&
                    IntHasNonZeroValue(t.LikesExercise) &&
                    IntHasNonZeroValue(t.TrainingLevel) &&
                    IntHasNonZeroValue(t.Trainability) &&
                    IntHasNonZeroValue(t.Stubbornness) &&
                    IntHasNonZeroValue(t.Intelligence) &&
                    IntHasNonZeroValue(t.SenseOfSmell) &&
                    IntHasNonZeroValue(t.PreyDrive) &&
                    IntHasNonZeroValue(t.AggressionLevel) &&
                    IntHasNonZeroValue(t.Protectiveness) &&
                    IntHasNonZeroValue(t.DistinguishThreatening) &&
                    IntHasNonZeroValue(t.BalanceStability) &&
                    IntHasNonZeroValue(t.Confidence) &&
                    IntHasNonZeroValue(t.IsPickyEater) &&
                    IntHasNonZeroValue(t.Shedding) &&
                    IntHasNonZeroValue(t.Barking) &&
                    IntHasNonZeroValue(t.SmellRating))
                return true;
            else
                return false;            
        }

        /// <summary>
        /// Checks if any biography property string in <see cref="Biography"/> object have value
        /// </summary>
        /// <param name="b"><see cref="Biography"/> object to check</param>
        /// <returns><see cref="bool"/>, true is any biography strings are populated</returns>
        private bool HasBiography(Biography b)
        {
            if (b == null)
                return false;
            else if (!string.IsNullOrWhiteSpace(b.AboutDoggo) ||
                !string.IsNullOrWhiteSpace(b.FavoriteMemory) ||
                !string.IsNullOrWhiteSpace(b.FavoriteFoods) ||
                !string.IsNullOrWhiteSpace(b.FavoriteFoods) ||
                !string.IsNullOrWhiteSpace(b.AboutDoggo) ||
                !string.IsNullOrWhiteSpace(b.AboutDoggo))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Generates dog temperament scores for 13 categories, each score is out of 100, 
        /// mostly derived by averaging similar or related properties in each category
        /// </summary>
        /// <param name="t"><see cref="Temperament"/> entity object</param>
        /// <returns><see cref="TemperamentScore[]"/> array of generated temperament scores and labels</returns>
        private IEnumerable<TemperamentScore> GetTemperamentScores(Temperament t)
        {
            return new TemperamentScore[]
            {
                new TemperamentScore
                {
                    ScoreLabel = "Playfullness",
                    ScoreValue = Convert.ToInt32((decimal)(
                        t.Playfulness +
                        t.LikesPlayingHumans +
                        t.LikesPlayingDogs +
                        t.PlaysFetch +
                        t.LikesToys +
                        t.LikesTreats) 
                        / 60 * 100)
                },
                new TemperamentScore
                {
                    ScoreLabel = "Friendliness",
                    ScoreValue = Convert.ToInt32((decimal)(
                        t.FriendlinessOverall +
                        t.GoodWithPeople +
                        t.GoodWithOtherDogs +
                        t.GoodWithCats +
                        t.GoodWithOtherAnimals +
                        t.GoodWithChildren) 
                        / 60 * 100)
                },
                new TemperamentScore
                {
                    ScoreLabel = "Athletic",
                    ScoreValue = Convert.ToInt32((decimal)(
                        t.AthleticLevel +
                        t.LikesExercise +
                        t.BalanceStability)
                        / 30 * 100)
                },
                new TemperamentScore
                {
                    ScoreLabel = "Training",
                    ScoreValue =  Convert.ToInt32((decimal)(
                        t.TrainingLevel +
                        t.Trainability +
                        t.Intelligence)
                        / 30 * 100)
                },
                new TemperamentScore
                {
                    ScoreLabel = "Empathy",
                    ScoreValue = Convert.ToInt32((decimal)(
                        t.Empathetic +
                        t.Intelligence)
                        / 20 * 100)
                },
                new TemperamentScore
                {
                    ScoreLabel = "Intelligence",
                    ScoreValue = Convert.ToInt32((decimal)t.Intelligence / 10 * 100)
                },
                new TemperamentScore
                {
                    ScoreLabel = "Aggression",
                    ScoreValue = Convert.ToInt32((decimal)(
                        t.AggressionLevel +
                        t.Barking +
                        t.PreyDrive)
                        / 30 * 100)
                },
                new TemperamentScore
                {
                    ScoreLabel = "Anxiety",
                    ScoreValue = Convert.ToInt32((decimal)(
                        t.Anxiety +
                        t.Fearful +
                        t.IsAfraidFireworks)
                        / 30 * 100)
                },
                new TemperamentScore
                {
                    ScoreLabel = "Instinct",
                    ScoreValue = Convert.ToInt32((decimal)(
                        t.PreyDrive +
                        t.DistinguishThreatening +
                        t.Protectiveness +
                        t.SenseOfSmell)
                        / 40 * 100)
                },
                new TemperamentScore
                {                     
                    ScoreLabel = "Confidence",
                    // subtract score by 20% of t.anxiety if t.confidence is greater,
                    // if 20% of t.anxiety is greater thescn subtract 20% of t.confidence from score
                    ScoreValue = Convert.ToInt32(                        
                        t.Confidence > (t.Anxiety * .2) ?
                        (t.Confidence -  ((decimal)(t.Anxiety * .2))) * 10 :                     
                        (t.Confidence - ((decimal)(t.Confidence * .2))) * 10)
                },
                new TemperamentScore
                {
                    ScoreLabel = "Stubbornness",
                    ScoreValue = Convert.ToInt32((decimal)(
                        t.Stubbornness +
                        t.IsPickyEater)
                        / 20 * 100)
                },
                new TemperamentScore
                {
                    ScoreLabel = "Shedding",
                    ScoreValue = Convert.ToInt32((decimal)t.Shedding / 10 * 100)
                },
                new TemperamentScore
                {
                    ScoreLabel = "Smelliness",
                    ScoreValue = Convert.ToInt32((decimal)t.SmellRating / 10 * 100)
                }
            };
        }

        /// <summary>
        /// Checks if nullable <see cref="int"/> is not null or 0
        /// </summary>
        /// <param name="i">nullable <see cref="int"/> to evaluate</param>
        /// <returns><see cref="bool"/>, true is nullable int is not null or zero</returns>
        private bool IntHasNonZeroValue(int? i) => i.GetValueOrDefault() != 0;
    #endregion Intneral Methods
    }
}
