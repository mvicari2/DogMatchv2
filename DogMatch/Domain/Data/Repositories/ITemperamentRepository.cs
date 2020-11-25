using DogMatch.Domain.Data.Models;
using System.Threading.Tasks;

namespace DogMatch.Domain.Data.Repositories
{
    public interface ITemperamentRepository
    {
        /// <summary>
        /// Find single temperament entity by dogId.
        /// </summary>        
        /// <param name="dogId">Dog Id integer</param>
        /// <returns><see cref="Temperament"/> entity</returns>
        Task<Temperament> FindTemperament(int dogId);

        /// <summary>
        /// Writes new temperament entity to database.
        /// </summary>        
        /// <param name="temperament"><see cref="Temperament"/> entity object</param>
        /// <returns>New <see cref="Temperament"/> entity with sql generated id.</returns>
        Task<Temperament> CreateNewTemperament(Temperament temperament);

        /// <summary>
        /// Save/Update Temperament entity
        /// </summary>
        /// <param name="temperament">Temperament entity object.</param>
        /// <returns>Bool to confirm changes saved.</returns>
        Task<bool> SaveTemperament(Temperament temperament);

        /// <summary>
        /// Evaluates all rating values in single dog <see cref="Temperament"/> object to confirm if all <see cref="int"/> properties are populated (not null or zero)
        /// </summary>
        /// <param name="t"><see cref="Temperament"/> entity object with values to evaluate</param>
        /// <returns><see cref="bool"/>, true if all temperament ratings values are not null or zero</returns>
        public bool HasCompletedTemperament(Temperament t);
    }
}
