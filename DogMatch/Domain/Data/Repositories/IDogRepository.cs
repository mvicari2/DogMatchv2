using DogMatch.Domain.Data.Models;
using DogMatch.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DogMatch.Domain.Data.Repositories
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
        /// Find single <see cref="Dogs"/> entity with navigation properties for Owner, Profile Image, Colors, Biography, Temperament and Album Images included
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns>Single <see cref="Dogs"/> entity for full profile</returns>
        Task<Dogs> FindFullDogProfileById(int id);

        /// <summary>
        /// Find dogs, filter and search using <see cref="DogsFilter"/> object property values
        /// </summary>
        /// <param name="filter"><see cref="DogsFilter"/> object containing filter and search values</param>
        /// <param name="ownerId">request user <see cref="string"/>, for finding dogs owned by user</param>
        /// <returns><see cref="IEnumerable{Dogs}"/> found dogs list</returns>
        Task<IEnumerable<Dogs>> FindDogsAndFilter(DogsFilter filter, string ownerId);

        /// <summary>
        /// Finds single <see cref="Dogs"/> entity and includes all active, non-deleted Dog Album Images (<see cref="DogImages"/>)
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns>Single <see cref="Dogs"/> entity object instance with album images</returns>
        Task<Dogs> FindDogWithAlbumImagesById(int id);

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
