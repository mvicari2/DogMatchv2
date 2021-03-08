using DogMatch.Domain.Data.Models;
using DogMatch.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DogMatch.Domain.Services
{
    public interface IDogProfileService
    {
        /// <summary>
        /// Get's <see cref="DogProfile"/> object for single dog, including basic details, 
        /// biography, album images, and generated temperament scores
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns>Single <see cref="DogProfile"/> object</returns>
        Task<DogProfile> GetDogProfile(int id);

        /// <summary>
        /// Generates dog temperament scores for 13 categories, each score is out of 100, 
        /// mostly derived by averaging similar or related properties in each category
        /// </summary>
        /// <param name="t"><see cref="Temperament"/> entity object</param>
        /// <returns><see cref="IEnumerable{TemperamentScore}"/> list generated temperament scores, score labels, and score types</returns>
        IEnumerable<TemperamentScore> GetTemperamentScores(Temperament t);
    }
}
