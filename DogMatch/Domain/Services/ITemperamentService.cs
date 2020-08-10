using DogMatch.Shared.Models;
using System.Threading.Tasks;

namespace DogMatch.Domain.Services
{
    public interface ITemperamentService
    {
        /// <summary>
        /// Gets <see cref="DogTemperament"/> instance by dogId
        /// </summary>        
        /// <param name="dogId">Dog Id integer</param>
        /// <returns>The found (mapped) <see cref="DogTemperament"/> instance if it exists</returns>
        Task<DogTemperament> GetDogTemperament(int dogId);

        /// <summary>
        /// Creates new single <see cref="Temperament"/>
        /// </summary>        
        /// <param name="dogId">Dog Id integer</param>
        /// <param name="userId">User Id string</param>
        /// <returns>Created (mapped) <see cref="DogTemperament"/> instance with SQL generated Id</returns>
        Task<DogTemperament> CreateTemperament(int dogId, string userId);

        /// <summary>
        /// Updates single <see cref="Temperament"/> entity
        /// </summary>
        /// <param name="temperament">
        /// <see cref="DogTemperament"/> instance with which to update existing record
        /// </param>
        /// <param name="userId">User Id string</param>
        /// <returns>Updated (mapped) <see cref="DogTemperament"/> instance</returns>
        Task<bool> UpdateTemperament(DogTemperament temperament, string userId);
    }
}
