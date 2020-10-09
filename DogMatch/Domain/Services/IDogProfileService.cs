using DogMatch.Shared.Models;
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
    }
}
