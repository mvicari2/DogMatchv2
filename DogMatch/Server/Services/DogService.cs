using DogMatch.Shared.Models;
using DogMatch.Server.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DogMatch.Server.Data.Repositories;
using DogMatch.Shared.Globals;

namespace DogMatch.Server.Services
{
    public class DogService : IDogService
    {
        #region DI
        private readonly IDogRepository _repository;
        private readonly IMapper _mapper;
        private readonly IImageService _imgService;

        public DogService(IDogRepository repository, IMapper mapper, IImageService imgService)
        {
            _repository = repository;
            _mapper = mapper;
            _imgService = imgService;
        }
        #endregion DI

        #region Service Methods
        /// <summary>
        /// Gets single <see cref="Dog"/> by Id
        /// </summary>        
        /// <param name="id">Dog Id integer</param>
        /// <returns>The found (mapped) <see cref="Dog"/> instance if it exists</returns>
        public async Task<Dog> GetDog(int id) =>
            _mapper.Map<Dog>(await _repository.FindDogById(id));

        /// <summary>
        /// Gets all active dogs
        /// </summary>        
        /// <returns>Mapped, <see cref="IEnumerable{Dog}"/><see cref="Dog"/></returns>
        public async Task<IEnumerable<Dog>> GetAllDogs() =>
            _mapper.Map<IEnumerable<Dog>>(await _repository.FindAllDogs());

        /// <summary>
        /// Create and save new <see cref="Dogs"/> entity
        /// </summary>
        /// <param name="dog"></param>
        /// <param name="userId"></param>
        /// <returns><see cref="Dog"/> instance mapped from new entity with SQL generated Id <see cref="int"/></returns>
        public async Task<Dog> CreateDog(Dog dog, string userId)
        {
            // map to new dog entity
            var dogEntity = _mapper.Map<Dogs>(dog);

            var now = DateTime.Now;
            dogEntity.Created = now;
            dogEntity.LastModified = now;
            dogEntity.OwnerId = userId;
            dogEntity.CreatedBy = userId;
            dogEntity.LastModifiedBy = userId;

            return _mapper.Map<Dog>(await _repository.SaveNewDog(dogEntity));
        }

        /// <summary>
        /// Updates single <see cref="Dogs"/> entity, mapped from <see cref="Dog"/> object
        /// </summary>
        /// <param name="id">dog Id <see cref="int"/></param>
        /// <param name="dog"><see cref="Dog"/> object with updated values</param>
        /// <param name="userId">user Id <see cref="string"/></param>
        /// <returns><see cref="bool"/>, true if entity successfully updated and saved</returns>
        public async Task<bool> UpdateDog(int id, Dog dog, string userId)
        {
            // find existing entity and map new values to it
            Dogs dogEntity = await _repository.FindDogById(id);
            _mapper.Map(dog, dogEntity);

            // save updated profile image if changed and get new image id
            if (dog.ProfileImage != null && dog.Extension != null)
            {                
                dogEntity.ProfileImageId = await _imgService.SaveProfileImage(dog.ProfileImage, dog.Extension, dog.Id, userId);
            }

            // update last modified values
            dogEntity.LastModified = DateTime.Now;
            dogEntity.LastModifiedBy = userId;

            // save entity
            return await _repository.SaveDog(dogEntity);
        }

        /// <summary>
        /// Soft deletes single <see cref="Dogs"/> instance (makes IsDeleted = true)
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <param name="userId">User Id <see cref="string"/> of current user</param>
        /// <returns><see cref="DeleteDogResponse"/> to confirm if dog was soft deleted successfully, failed, or unauthorized (non-owner)</returns>
        public async Task<DeleteDogResponse> DeleteDog(int id, string userId)
        {
            // find existing entity
            Dogs dogEntity = await _repository.FindDogById(id);

            // ensure owner is same user making request to delete
            if (dogEntity.OwnerId == userId)
            {
                dogEntity.IsDeleted = true;
                dogEntity.LastModified = DateTime.Now;
                dogEntity.LastModifiedBy = userId;

                // save entity
                var success = await _repository.SaveDog(dogEntity);

                return success ? DeleteDogResponse.Success : DeleteDogResponse.Failed;
            }
            else
            {
                // return not authorized to delete
                return DeleteDogResponse.Unauthorized;
            }            
        }
        #endregion Service Methods
    }
}
