using DogMatch.Domain.Data.Models;
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
        /// Finds all active (non-deleted) Dogs
        /// </summary>        
        /// <returns><see cref="IEnumerable{Dogs}" /><see cref="Dogs"/> All active Dogs</returns>
        Task<IEnumerable<Dogs>> FindAllDogs();

        /// <summary>
        /// Find Dogs by Owner (finds all dogs where current user is owner)
        /// </summary>        
        /// <param name="userId">User Id <see cref="string"/> for owner</param>
        /// <returns><see cref="IEnumerable{Dogs}" /> All dogs owned by user</returns>
        Task<IEnumerable<Dogs>> FindDogsByOwner(string userId);

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
