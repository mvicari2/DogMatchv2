using DogMatch.Shared.Models;
using System.Threading.Tasks;

namespace DogMatch.Domain.Services
{
    public interface IBiographyService
    {
        /// <summary>
        /// Gets <see cref="DogBiography"/> instance by dogId
        /// </summary>        
        /// <param name="dogId">Dog Id <see cref="int"/></param>
        /// <returns>The found (mapped) <see cref="DogBiography"/> instance if it exists</returns>
        Task<DogBiography> GetDogBiography(int dogId);

        /// <summary>
        /// Creates new single <see cref="Biography"/>
        /// </summary>        
        /// <param name="dogId">Dog Id <see cref="int"/></param>
        /// <param name="userId">User Id <see cref="string"/></param>
        /// <returns>Created (mapped) <see cref="DogBiography"/> instance with SQL generated Id</returns>
        Task<DogBiography> CreateBiography(int dogId, string userId);

        /// <summary>
        /// Updates single <see cref="Biography"/> entity
        /// </summary>
        /// <param name="bio">
        /// <see cref="DogBiography"/> instance with which to update existing record
        /// </param>
        /// <param name="userId">User Id <see cref="string"/></param>
        /// <returns>Updated (mapped) <see cref="DogBiography"/> instance</returns>
        Task<bool> UpdateBiography(DogBiography bio, string userId);
    }
}
