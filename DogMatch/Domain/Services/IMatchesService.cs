using DogMatch.Shared.Models;
using System.Threading.Tasks;

namespace DogMatch.Domain.Services
{
    public interface IMatchesService
    {
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
        Task<DogMatches> GetDogMatchesByDogId(int id, string ownerId);
    }
}
