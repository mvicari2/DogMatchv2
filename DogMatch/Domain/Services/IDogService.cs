﻿using DogMatch.Shared.Globals;
using DogMatch.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using DogMatch.Domain.Data.Models;

namespace DogMatch.Domain.Services
{
    public interface IDogService
    {
        /// <summary>
        /// Gets single <see cref="Dog"/> by Id
        /// </summary>        
        /// <param name="id">Dog Id integer</param>
        /// <returns>The found (mapped) <see cref="Dog"/> instance if it exists</returns>
        Task<Dog> GetDog(int id);

        /// <summary>
        /// Gets, searches, and filters all active dogs
        /// </summary>
        /// <param name="filter"><see cref="DogsFilter"/> object containing filter properties</param>
        /// <param name="userId">current request user Id <see cref="string"/></param>
        /// <returns><see cref="IEnumerable{Dogs}"/> active filtered/searched dog results </returns>
        Task<IEnumerable<Dog>> GetDogsAndFilter(DogsFilter filter, string userId);

        /// <summary>
        /// Create and save new <see cref="Dogs"/> entity
        /// </summary>
        /// <param name="dog"></param>
        /// <param name="userId"></param>
        /// <returns><see cref="Dog"/> instance mapped from new entity with SQL generated Id <see cref="int"/></returns>
        Task<Dog> CreateDog(Dog dog, string userId);

        /// <summary>
        /// Updates single <see cref="Dogs"/> entity, mapped from <see cref="Dog"/> object
        /// </summary>
        /// <param name="id">dog Id <see cref="int"/></param>
        /// <param name="dog"><see cref="Dog"/> object with updated values</param>
        /// <param name="userId">user Id <see cref="string"/></param>
        /// <returns><see cref="bool"/>, true if entity successfully updated and saved</returns>
        Task<bool> UpdateDog(int id, Dog dog, string userId);

        /// <summary>
        /// Soft deletes single <see cref="Dogs"/> instance (makes IsDeleted = true)
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <param name="userId">User Id <see cref="string"/> of current user</param>
        /// <returns><see cref="DeleteDogResponse"/> to confirm if dog was soft deleted successfully, failed, or unauthorized (non-owner)</returns>
        Task<DeleteDogResponse> DeleteDog(int id, string userId);
    }
}
