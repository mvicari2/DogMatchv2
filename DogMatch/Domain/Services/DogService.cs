using DogMatch.Shared.Models;
using DogMatch.Domain.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DogMatch.Domain.Data.Repositories;
using DogMatch.Shared.Globals;
using System.Linq;

namespace DogMatch.Domain.Services
{
    public class DogService : IDogService
    {
        #region DI
        private readonly IDogRepository _repository;
        private readonly IColorRepository _colorRepository;
        private readonly IMapper _mapper;
        private readonly IImageService _imgService;

        public DogService(IDogRepository repository, IColorRepository colorRepository, IMapper mapper, IImageService imgService)
        {
            _repository = repository;
            _colorRepository = colorRepository;
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
        /// Gets all active dogs owned by single user
        /// </summary>
        /// <param name="userId">Owner/User Id <see cref="string"/></param>
        /// <returns><see cref="IEnumerable{Dog}"/> Owner's/User's dogs, mapped to <see cref="Dog"/> from entity</returns>
        public async Task<IEnumerable<Dog>> GetDogsByOwner(string userId) =>
            _mapper.Map<IEnumerable<Dog>>(await _repository.FindDogsByOwner(userId));

        /// <summary>
        /// Create and save new <see cref="Dogs"/> entity
        /// </summary>
        /// <param name="dog"></param>
        /// <param name="userId"></param>
        /// <returns><see cref="Dog"/> instance mapped from new entity with SQL generated Id <see cref="int"/></returns>
        public async Task<Dog> CreateDog(Dog dog, string userId)
        {
            // map to new dog entity
            Dogs dogEntity = _mapper.Map<Dogs>(dog);

            DateTime now = DateTime.Now;
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
            // find existing entity
            Dogs dogEntity = await _repository.FindDogById(id);

            // add/remove dog colors 
            await HandleColors(dog.Id, dog.Colors, dogEntity.Colors);

            // map new dog values to existing dog entity
            _mapper.Map(dog, dogEntity);            

            // save updated profile image if changed and get new image id
            if (!string.IsNullOrWhiteSpace(dog.ProfileImage) && !string.IsNullOrWhiteSpace(dog.Extension))
            {                
                dogEntity.ProfileImageId = await _imgService.SaveProfileImage(dog.ProfileImage, dog.Extension, dog.Id, userId);
            }

            // update last modified values
            dogEntity.LastModified = DateTime.Now;
            dogEntity.LastModifiedBy = userId;

            // save dog entity
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
                bool success = await _repository.SaveDog(dogEntity);

                return success ? DeleteDogResponse.Success : DeleteDogResponse.Failed;
            }
            else
            {
                // return not authorized to delete
                return DeleteDogResponse.Unauthorized;
            }            
        }
        #endregion Service Methods

        #region Internal Methods
        /// <summary>
        /// Handles saving of new dog colors and deleteing of removed Dog colors based on new color list
        /// </summary>
        /// <param name="dogId">Dog Id <see cref="int"/> for dog that owns colors</param>
        /// <param name="colors">New colors <see cref="IEnumerable{Color}"/> for dog</param>
        /// <param name="existingColors">Existing <see cref="Color"/> entities <see cref="IEnumerable{Color}"/> for dog</param>
        private async Task HandleColors(int dogId, IEnumerable<string> colors, IEnumerable<Color> existingColors)
        {
            if (existingColors?.Count() > 0 && colors?.Count() > 0)
            {
                // delete any existing colors not in new color list
                IEnumerable<Color> deleteColors = existingColors
                    .Where(c => !colors.Any(clr => clr.Equals(c.ColorString)));

                await _colorRepository.RemoveColors(deleteColors);

                // remove any existing colors from new color list (don't need to save them again)
                colors = colors.Where(c => !existingColors.Any(clr => clr.ColorString.Equals(c)));
            }
            else if (existingColors?.Count() > 0 && colors?.Count() < 1)
            {
                // delete all existing colors if new color list is empty
                await _colorRepository.RemoveColors(existingColors);
            }            

            // map new colors to Color entities
            if (colors.Count() > 0)
            {
                List<Color> colorEntities = new List<Color>();

                colorEntities = _mapper.Map<IEnumerable<string>, List<Color>>(colors, opt =>
                    opt.AfterMap((src, dest) => {
                        foreach (var color in dest)
                        {
                            color.DogId = dogId;
                        }
                    }));

                // save new color entities
                await _colorRepository.SaveColors(colorEntities);
            }
        }
        #endregion Internal Methods
    }
}
