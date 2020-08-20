using DogMatch.Domain.Data.Models;
using System.Threading.Tasks;

namespace DogMatch.Domain.Data.Repositories
{
    public interface IBiographyRepository
    {
        /// <summary>
        /// Find single Biography entity by dogId.
        /// </summary>        
        /// <param name="dogId">Dog Id <see cref="int"/></param>
        /// <returns><see cref="Biography"/> entity</returns>
        Task<Biography> FindBiography(int dogId);

        /// <summary>
        /// Writes new Biography entity to database.
        /// </summary>        
        /// <param name="bio"><see cref="Biography"/> entity object</param>
        /// <returns>New <see cref="Biography"/> entity with sql generated id.</returns>
        Task<Biography> CreateNewBiography(Biography bio);

        /// <summary>
        /// Save/Update Biography entity
        /// </summary>
        /// <param name="bio"><see cref="Biography"/> entity object.</param>
        /// <returns>Bool to confirm changes saved.</returns>
        Task<bool> SaveBiography(Biography bio);
    }
}
