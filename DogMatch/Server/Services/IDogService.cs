using DogMatch.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogMatch.Server.Services
{
    public interface IDogService
    {
        Task<Dog> GetDog(int id);
        Task<IEnumerable<Dog>> GetAllDogs();
        Task<Dog> CreateDog(Dog dog, string userId);
        Task<bool> UpdateDog(int id, Dog dog, string userId);
    }
}
