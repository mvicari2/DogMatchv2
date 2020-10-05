using AutoMapper;
using DogMatch.Domain.Data.Models;
using DogMatch.Domain.Data.Repositories;
using DogMatch.Shared.Models;
using System;
using System.Threading.Tasks;

namespace DogMatch.Domain.Services
{
    public class BiographyService : IBiographyService
    {
        #region DI
        private readonly IBiographyRepository _repository;
        private readonly IMapper _mapper;

        public BiographyService(IBiographyRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        #endregion DI

        #region Service Methods
        /// <summary>
        /// Gets <see cref="DogBiography"/> instance by dogId
        /// </summary>        
        /// <param name="dogId">Dog Id <see cref="int"/></param>
        /// <returns>The found (mapped) <see cref="DogBiography"/> instance if it exists</returns>
        public async Task<DogBiography> GetDogBiography(int dogId) => 
            _mapper.Map<DogBiography>(
                await _repository.FindBiography(dogId)
            );

        /// <summary>
        /// Creates new single <see cref="Biography"/>
        /// </summary>        
        /// <param name="dogId">Dog Id <see cref="int"/></param>
        /// <param name="userId">User Id <see cref="string"/></param>
        /// <returns>Created (mapped) <see cref="DogBiography"/> instance with SQL generated Id</returns>
        public async Task<DogBiography> CreateBiography(int dogId, string userId)
        {
            DateTime now = DateTime.Now;
            Biography bio = new Biography
            {
                DogId = dogId,
                Created = now,
                LastModified = now,
                CreatedBy = userId,
                LastModifiedBy = userId
            };

            return _mapper.Map<DogBiography>(
                await _repository.CreateNewBiography(bio)
            );
        }

        /// <summary>
        /// Updates single <see cref="Biography"/> entity
        /// </summary>
        /// <param name="bio">
        /// <see cref="DogBiography"/> instance with which to update existing record
        /// </param>
        /// <param name="userId">User Id <see cref="string"/></param>
        /// <returns>Updated (mapped) <see cref="DogBiography"/> instance</returns>
        public async Task<bool> UpdateBiography(DogBiography bio, string userId)
        {
            Biography bioEntity = await _repository.FindBiography(bio.DogId);
            _mapper.Map(bio, bioEntity);

            bioEntity.LastModified = DateTime.Now;
            bioEntity.LastModifiedBy = userId;

            bool updated = await _repository.SaveBiography(bioEntity);

            if (updated)
                return true;
            else
                return false;
        }
        #endregion Service Methods
    }   
}
