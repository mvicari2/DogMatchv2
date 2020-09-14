using DogMatch.Domain.Data.Models;
using System.Threading.Tasks;

namespace DogMatch.Domain.Data.Repositories
{
    public interface IDogImagesRepository
    {
        /// <summary>
        /// Saves new single <see cref="DogImages"/> entity to database
        /// </summary>
        /// <param name="image"><see cref="DogImages"/> entity to write to database</param>
        /// <returns><see cref="DogImages"/> entity instance w/SQL generated Id</returns>
        Task<DogImages> SaveDogImage(DogImages image);
    }
}
