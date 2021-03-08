using AutoMapper;
using DogMatch.Domain.Data.Models;
using DogMatch.Domain.Data.Repositories;
using DogMatch.Shared.Models;
using DogMatch.Shared.Globals;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogMatch.Domain.Services
{
    public class MatchesService : IMatchesService
    {
        #region DI
        private readonly IDogRepository _dogRepository;
        private readonly IDogProfileService _dogProfileService;
        private readonly ITemperamentRepository _temperamentRepository;
        private readonly IMapper _mapper;

        public MatchesService(IDogRepository dogRepository, IDogProfileService dogProfileService, IMapper mapper, ITemperamentRepository temperamentRepository)
        {            
            _dogRepository = dogRepository;
            _dogProfileService = dogProfileService;
            _temperamentRepository = temperamentRepository;
            _mapper = mapper;
        }
        #endregion DI

        #region Public Methods
        /// <summary>
        /// Public service method for getting the top matches (up to 10) for a specific single dog,
        /// matched largely based on the scores from the temperament profile ratings completed for 
        /// each dog. Only completed profiles are assessed (where all temperament ratings, basic 
        /// details, about dog are completed). If the requested primary dog's profile is not 
        /// completed no matches will be returned.
        /// 
        /// Logic/Process for getting matches for a requested primary dog:
        /// (1) Get requested primary dog by dog Id and check for completed profile.
        /// (2) Call repo to get all active dogs (entities) that have completed profiles.
        /// (3) Initialize objects to store match data, and get initial scores and match values 
        ///     (per category) for primary dog with which to compare against all other 
        ///     completed profiles.
        /// (4) Get temperament ratings scores based on categories for each possible dog match, and
        ///     use them (and other dog details) to determine a match value for each possible
        ///     matching criteria category. 
        /// (5) Compare the primary dog's match values in each category against each potential dog's
        ///     match values, and determine a match count int for each possible match by tabulating
        ///     the occurances of a category match value being equal to the primary dog. The match
        ///     values are determined by placing a category's match score within one of several
        ///     possible score ranges, and assigning that range a number (that is the match value).
        /// (6) The higher the determined match count int is for each dog the higher the quality of
        ///     match that dog is to the primary dog. The top matches (up to 10), ordered by
        ///     the highest match counts, are then selected to be returned as the top matches
        ///     for the primary dog.
        /// </summary>
        /// <param name="id">
        /// Primary dog id <see cref="int"/> for which to determine top matches
        /// </param>
        /// <param name="ownerId">Owner id <see cref="string"/> for the primary dog owner</param>
        /// <returns><see cref="DogMatches"/> object which contains a list of the top dog matches 
        /// to the primary dog (up to 10 possible matches)</returns>
        public async Task<DogMatches> GetDogMatchesByDogId(int id, string ownerId)
        {
            // get primary dog
            Dogs primaryDog = await _dogRepository.FindFullDogProfileById(id);

            // determine if primary dog has completed profile, 
            // if not return DogMatches object that declares primary dog's profile is incomplete,
            // and do not attempt to find matches for primary dog
            if (!DogHasCompletedProfile(primaryDog))
                return IncompleteProfile(primaryDog);

            // get dogs w/completed profiles
            IEnumerable<Dogs> allDogs = await _dogRepository.FindAllActiveMatchableDogs(id, ownerId);

            // set primary dog matches scores, the base scores from which to
            // compare all other completed dog profiles and potential matches
            MatchScores primaryDogScores = GetAllMatchValuesForDog(primaryDog);

            // prepare dog match count tracker
            List<DogMatchTracker> dogMatchCounters = InitializeDogMatchTrackers(allDogs);

            // tabulate match counts
            dogMatchCounters = TabulateMatchCounts(primaryDogScores, dogMatchCounters);

            // order dog match counters by highest match, then take top 10,
            // then filter allDogs by these 10 DogId's
            IEnumerable<int> OrderedDogIds = dogMatchCounters
                .OrderByDescending(d => d.MatchCount)
                .Select(d => d.DogId)
                .Take(10);

            return new DogMatches()
            {
                DogId = primaryDog.Id,
                DogName = primaryDog.Name,
                OwnerName = primaryDog.Owner.UserName,
                CompletedProfile = true,
                Matches = PrepareMatches(OrderedDogIds, allDogs) 
            };
        }
        #endregion Public Methods

        #region Intneral Methods
        /// <summary>
        /// Gets the <see cref="Dogs"/> object (dto) for each individually matching dog and adds them
        /// to a list using a list of dog id's (top matches) extracted during the matching process
        /// </summary>
        /// <param name="dogIds">enumerated dog Ids <see cref="IEnumerable{int}"/> for the top matches</param>
        /// <param name="allDogs">All possible (complete) dog profiles from which matches are found</param>
        /// <returns><see cref="List{Match}"/> list of top dog matches</returns>
        private List<Match> PrepareMatches(IEnumerable<int> dogIds, IEnumerable<Dogs> allDogs)
        {
            List<Match> finalMatches = new List<Match>();

            foreach (var dogId in dogIds)
            {
                // map Dogs object to Match object and add to match list
                finalMatches.Add(
                    _mapper.Map<Match>(
                        allDogs.Where(d => d.Id == dogId).SingleOrDefault()
                ));
            }

            return finalMatches;
        }

        /// <summary>
        /// Tabulates the count of match score categories in which a possible match 
        /// (a dog w/completed profile) has the same match value in a category as the primary dog.
        /// The higher the match count for a dog, the higher the match quality. Top matches are later
        /// determined by the dogs with the highest match counts derived from this method.
        /// </summary>
        /// <param name="primaryDogScores">Match values of the primary dog</param>
        /// <param name="dogTrackers"><see cref="List{DogMatchTracker}"/> list of all dogs and their match values</param>
        /// <returns><see cref="List{DogMatchTracker}"/> list of dogs that includes the total match count for each possible dog match</returns>
        private List<DogMatchTracker> TabulateMatchCounts(MatchScores primaryDogScores, List<DogMatchTracker> dogTrackers)
        {
            foreach (var dog in dogTrackers)
            {
                // determine if Weight scores match, add 3 match points for weight 
                // (gives size/weight of dog more weight than other categories)
                if (dog.Scores.Weight == primaryDogScores.Weight)
                    dog.MatchCount = dog.MatchCount + 3;

                // determine if Playfullness scores match
                if (dog.Scores.Playfullness == primaryDogScores.Playfullness)
                    dog.MatchCount = dog.MatchCount + 1;

                // determine if Friendliness scores match
                if (dog.Scores.Friendliness == primaryDogScores.Friendliness)
                    dog.MatchCount = dog.MatchCount + 1;

                // determine if Athletic scores match
                if (dog.Scores.Athletic == primaryDogScores.Athletic)
                    dog.MatchCount = dog.MatchCount + 1;

                // determine if Training scores match
                if (dog.Scores.Training == primaryDogScores.Training)
                    dog.MatchCount = dog.MatchCount + 1;

                // determine if Empathy scores match
                if (dog.Scores.Empathy == primaryDogScores.Empathy)
                    dog.MatchCount = dog.MatchCount + 1;

                // determine if Intelligence scores match
                if (dog.Scores.Intelligence == primaryDogScores.Intelligence)
                    dog.MatchCount = dog.MatchCount + 1;

                // determine if Aggression scores match
                if (dog.Scores.Aggression == primaryDogScores.Aggression)
                    dog.MatchCount = dog.MatchCount + 1;

                // determine if Anxiety scores match
                if (dog.Scores.Anxiety == primaryDogScores.Anxiety)
                    dog.MatchCount = dog.MatchCount + 1;

                // determine if Instinct scores match
                if (dog.Scores.Instinct == primaryDogScores.Instinct)
                    dog.MatchCount = dog.MatchCount + 1;

                // determine if Confidence scores match
                if (dog.Scores.Confidence == primaryDogScores.Confidence)
                    dog.MatchCount = dog.MatchCount + 1;

                // determine if Stubbornness scores match
                if (dog.Scores.Stubbornness == primaryDogScores.Stubbornness)
                    dog.MatchCount = dog.MatchCount + 1;

                // determine if Shedding scores match
                if (dog.Scores.Shedding == primaryDogScores.Shedding)
                    dog.MatchCount = dog.MatchCount + 1;

                // determine if Smelliness scores match
                if (dog.Scores.Smelliness == primaryDogScores.Smelliness)
                    dog.MatchCount = dog.MatchCount + 1;

                // determine if dogs are same breed, if so give 5 points (more points than others)
                if (dog.Scores.Breed == primaryDogScores.Breed)
                    dog.MatchCount = dog.MatchCount + 5;
            }

            return dogTrackers;
        }

        /// <summary>
        /// Initialzes new <see cref="List{DogMatchTracker}"/> (<see cref="DogMatchTracker"/> list),
        /// which tracks match scores and dog demographics for a list of individual dogs (the 
        /// possible matches, only dogs with completed profiles)
        /// </summary>
        /// <param name="dogs"><see cref="IEnumerable{Dogs}"/> enumerated Dogs to get match scores for</param>
        /// <returns><see cref="List{DogMatchTracker}"/> (<see cref="DogMatchTracker"/> list)</returns>
        private List<DogMatchTracker> InitializeDogMatchTrackers(IEnumerable<Dogs> dogs)
        {
            List<DogMatchTracker> trackers = new List<DogMatchTracker>();

            foreach (var dog in dogs)
            {
                trackers.Add(new DogMatchTracker()
                {
                    DogId = dog.Id,
                    MatchCount = 0, // count determined later in TabulateMatchCounts method
                    Scores = GetAllMatchValuesForDog(dog)
                });
            }

            return trackers;
        }

        /// <summary>
        /// Get's the match score values for each possible matching catgegory for an individual dog
        /// and returns the values packaged into a <see cref="MatchScores"/> object
        /// </summary>
        /// <param name="dog"><see cref="Dogs"/> dog to get match values for</param>
        /// <returns>populated <see cref="MatchScores"/> object for an individual dog</returns>
        private MatchScores GetAllMatchValuesForDog(Dogs dog)
        {
            IEnumerable<TemperamentScore> temperamentScores = _dogProfileService.GetTemperamentScores(dog.Temperament);

            return new MatchScores()
            {
                Weight = DetermineWeightMatchValue(dog.Weight ?? 0),
                Breed = dog.Breed, // passes actual breed string for matching
                Playfullness = DetermineTemperamentMatchValue(
                    temperamentScores
                        .Where(t => t.ScoreType == TemperamentScoreTypes.Playfullness)
                        .Select(t => t.ScoreValue)
                        .SingleOrDefault()
                    ),
                Friendliness = DetermineTemperamentMatchValue(
                    temperamentScores
                        .Where(t => t.ScoreType == TemperamentScoreTypes.Friendliness)
                        .Select(t => t.ScoreValue)
                        .SingleOrDefault()
                    ),
                Athletic = DetermineTemperamentMatchValue(
                    temperamentScores
                        .Where(t => t.ScoreType == TemperamentScoreTypes.Athletic)
                        .Select(t => t.ScoreValue)
                        .SingleOrDefault()
                    ),
                Training = DetermineTemperamentMatchValue(
                    temperamentScores
                        .Where(t => t.ScoreType == TemperamentScoreTypes.Training)
                        .Select(t => t.ScoreValue)
                        .SingleOrDefault()
                    ),
                Empathy = DetermineTemperamentMatchValue(
                    temperamentScores
                        .Where(t => t.ScoreType == TemperamentScoreTypes.Empathy)
                        .Select(t => t.ScoreValue)
                        .SingleOrDefault()
                    ),
                Intelligence = DetermineTemperamentMatchValue(
                    temperamentScores
                        .Where(t => t.ScoreType == TemperamentScoreTypes.Intelligence)
                        .Select(t => t.ScoreValue)
                        .SingleOrDefault()
                    ),
                Aggression = DetermineTemperamentMatchValue(
                    temperamentScores
                        .Where(t => t.ScoreType == TemperamentScoreTypes.Aggression)
                        .Select(t => t.ScoreValue)
                        .SingleOrDefault()
                    ),
                Anxiety = DetermineTemperamentMatchValue(
                    temperamentScores
                        .Where(t => t.ScoreType == TemperamentScoreTypes.Anxiety)
                        .Select(t => t.ScoreValue)
                        .SingleOrDefault()
                    ),
                Instinct = DetermineTemperamentMatchValue(
                    temperamentScores
                        .Where(t => t.ScoreType == TemperamentScoreTypes.Instinct)
                        .Select(t => t.ScoreValue)
                        .SingleOrDefault()
                    ),
                Confidence = DetermineTemperamentMatchValue(
                    temperamentScores
                        .Where(t => t.ScoreType == TemperamentScoreTypes.Confidence)
                        .Select(t => t.ScoreValue)
                        .SingleOrDefault()
                    ),
                Stubbornness = DetermineTemperamentMatchValue(
                    temperamentScores
                        .Where(t => t.ScoreType == TemperamentScoreTypes.Stubbornness)
                        .Select(t => t.ScoreValue)
                        .SingleOrDefault()
                    ),
                Shedding = DetermineTemperamentMatchValue(
                    temperamentScores
                        .Where(t => t.ScoreType == TemperamentScoreTypes.Shedding)
                        .Select(t => t.ScoreValue)
                        .SingleOrDefault()
                    ),
                Smelliness = DetermineTemperamentMatchValue(
                    temperamentScores
                        .Where(t => t.ScoreType == TemperamentScoreTypes.Smelliness)
                        .Select(t => t.ScoreValue)
                        .SingleOrDefault()
                    )
            };
        }

        /// <summary>
        /// Finds a match value, 1-4, based on an individual temperament score for a single score
        /// category. The 1-4 match value is determined by which of 4 possible score ranges the
        /// provided score (0-100) falls into:
        /// (1) -> 0-25, (2) -> 26-50, (3) -> 51-75, (4) -> 76-100
        /// </summary>
        /// <param name="temperamentScore">the temperament score <see cref="int"/> to evaluate</param>
        /// <returns>temperament match value <see cref="int"/> (1-4)</returns>
        private int DetermineTemperamentMatchValue(int temperamentScore)
        {
            if (temperamentScore > 0 && temperamentScore <= 25)
                return 1;
            else if (temperamentScore > 25 && temperamentScore <= 50)
                return 2;
            else if (temperamentScore > 50 && temperamentScore <= 75)
                return 3;
            else if (temperamentScore > 75 && temperamentScore <= 100)
                return 4;
            else            
                return 0;
        }

        /// <summary>
        /// Finds a match value, 1-6, based on a the weight (in lbs) of a dog. The 1-4 match value 
        /// is determined by which of 6 possible weight ranges the provided weight falls into:
        /// (1) -> 0-15 lbs (2) -> 16-30 lbs, (3) -> 31-50 lbs, 
        /// (4) -> 51-75 lbs, (5) ->  76-100lbs, (6) -> 101+ lbs
        /// </summary>
        /// <param name="weight">the weight <see cref="int"/> to evaluate</param>
        /// <returns>match value <see cref="int"/> (1-6)</returns>
        private int DetermineWeightMatchValue(int weight)
        {
            if (weight >= 0 && weight < 15)
                return 1;
            else if (weight > 15 && weight <= 30)
                return 2;
            else if (weight > 30 && weight <= 50)
                return 3;
            else if (weight > 50 && weight <= 75)
                return 4;
            else if (weight > 75 && weight < 100)
                return 5;
            else if (weight > 100)
                return 6;
            else
                return 0;
        }

        /// <summary>
        /// Determines if an individual dog (entity) has a completed profile by checking if all
        /// temperament ratings and basic details are populated or have value and if the 'about dog'
        /// biography is populated (just 'about dog', not all bio fields)
        /// </summary>
        /// <param name="dog"><see cref="Dogs"/> entity to check for completed profile</param>
        /// <returns><see cref="bool"/>, true if dog has completed profile</returns>
        private bool DogHasCompletedProfile(Dogs dog)
        {
            // check if temperament profile is completed
            if (!_temperamentRepository.HasCompletedTemperament(dog.Temperament))
                return false;

            // check if biography:about dog is completed
            if (!dog.BiographyId.HasValue)
                return false;
            else
                if (string.IsNullOrWhiteSpace(dog.Biography.AboutDoggo))
                    return false;

            // check if all basic details are populated or have value
            if (string.IsNullOrWhiteSpace(dog.Breed) || 
                string.IsNullOrWhiteSpace(dog.Biography?.AboutDoggo) ||
                !dog.Birthday.HasValue ||
                !dog.ProfileImageId.HasValue ||
                !dog.Weight.HasValue ||
                !dog.Gender.HasValue ||
                dog.Colors.Count < 1) 
                return false;

            // if dog passes all checks return true for completed profile
            return true;
        }

        /// <summary>
        /// Initializes new <see cref="DogMatches"/> object for a single dog
        /// with CompletedProfile bool property set to false. Used for returning a
        /// <see cref="DogMatches"/> object without any actual <see cref="List{Matches}}"/> matches.
        /// </summary>
        /// <param name="dog"><see cref="Dogs"/> object for single dog</param>
        /// <returns>New <see cref="DogMatches"/> instance for incomplete profile designation</returns>
        private static DogMatches IncompleteProfile(Dogs dog) =>
            new DogMatches()
            {
                DogId = dog.Id,
                DogName = dog.Name,
                OwnerName = dog.Owner.UserName,
                CompletedProfile = false
            };

        /// <summary>
        /// Internal type object used to organize individual dog's match scores and match count
        /// </summary>
        private class DogMatchTracker
        {
            public int DogId { get; set; }
            public int MatchCount { get; set; }
            public MatchScores Scores { get; set; }
        }

        /// <summary>
        /// Internal type object used to track match scores
        /// in specific categories for an individual dog
        /// </summary>
        private class MatchScores
        {
            public int Weight { get; set; }
            public string Breed { get; set; }
            public int Playfullness { get; set; }
            public int Friendliness { get; set; }
            public int Athletic { get; set; }
            public int Training { get; set; }
            public int Empathy { get; set; }
            public int Intelligence { get; set; }
            public int Aggression { get; set; }
            public int Anxiety { get; set; }
            public int Instinct { get; set; }
            public int Confidence { get; set; }
            public int Stubbornness { get; set; }
            public int Shedding { get; set; }
            public int Smelliness { get; set; }
        }
        #endregion Internal Methods        
    }   
}