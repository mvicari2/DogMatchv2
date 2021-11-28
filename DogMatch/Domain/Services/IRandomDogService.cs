using DogMatch.Shared.Models;
using System.Threading.Tasks;

namespace DogMatch.Domain.Services
{
    public interface IRandomDogService
    {
        /// <summary>
        /// Gets a random dog by fetching various public APIs to get a random dog image with breed
        /// (using open source public dog.ceo api which utilizes the stanford dogs dataset), a random
        /// name (using a public name generator api), and also generates a random age <see cref="int"/>
        /// </summary>
        /// <returns>Task containing <see cref="RandomDog"/> type object</returns>
        Task<RandomDog> FetchRandomDog();
    }
}
