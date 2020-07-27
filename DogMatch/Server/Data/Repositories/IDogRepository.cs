using DogMatch.Server.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DogMatch.Server.Data.Repositories
{
    public interface IDogRepository
    {
        /// <summary>
        /// Find single <see cref="Dogs"/> entity by dog Id <see cref="int"/>.
        /// </summary>        
        /// <param name="id">Dog Id integer</param>
        /// <returns>single <see cref="Dogs"/> entity</returns>
        Task<Dogs> FindDogById(int id);

        /// <summary>
        /// Finds all active (non-deleted) Dogs
        /// </summary>        
        /// <returns><see cref="IEnumerable{Dogs}" /><see cref="Dogs"/> All active Dogs</returns>
        Task<IEnumerable<Dogs>> FindAllDogs();

        /// <summary>
        /// Writes new, single <see cref="Dogs"/> entity to database.
        /// </summary>        
        /// <param name="dog"><see cref="Dogs"/> entity object</param>
        /// <returns>The new <see cref="Dogs"/> entity with sql generated id <see cref="int"/>.</returns>
        Task<Dogs> SaveNewDog(Dogs dog);

        /// <summary>
        /// Save/Update single <see cref="Dogs"/> entity
        /// </summary>
        /// <param name="dog"><see cref="Dogs"/> entity object.</param>
        /// <returns><see cref="bool"/> to confirm changes saved.</returns>
        Task<bool> SaveDog(Dogs dog);        
    }
}
