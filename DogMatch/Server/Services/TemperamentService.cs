using AutoMapper;
using DogMatch.Server.Data.Models;
using DogMatch.Server.Data.Repositories;
using DogMatch.Shared.Models;
using System;
using System.Threading.Tasks;

namespace DogMatch.Server.Services
{
    public class TemperamentService : ITemperamentService
    {
        private readonly ITemperamentRepository _repository;
        private readonly IMapper _mapper;

        public TemperamentService(ITemperamentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        #region Service Methods
        /// <summary>
        /// Gets <see cref="DogTemperament"/> instance by dogId
        /// </summary>        
        /// <param name="dogId">Dog Id integer</param>
        /// <returns>The found (mapped) <see cref="DogTemperament"/> instance if it exists</returns>
        public async Task<DogTemperament> GetDogTemperament(int dogId)
        {
            var temperamentEntity = await _repository.FindTemperament(dogId);
            return _mapper.Map<DogTemperament>(temperamentEntity);            
        }

        /// <summary>
        /// Creates new single <see cref="Temperament"/>
        /// </summary>        
        /// <param name="dogId">Dog Id integer</param>
        /// <param name="userId">User Id string</param>
        /// <returns>Created (mapped) <see cref="DogTemperament"/> instance with SQL generated Id</returns>
        public async Task<DogTemperament> CreateTemperament(int dogId, string userId)
        {
            var now = DateTime.Now;
            var temperamentEntity = new Temperament
            {
                DogId = dogId,
                Created = now,
                LastModified = now,
                CreatedBy = userId,
                LastModifiedBy = userId
            };            

            var newEntity = await _repository.CreateNewTemperament(temperamentEntity);
            return _mapper.Map<DogTemperament>(newEntity);
        }

        /// <summary>
        /// Updates single <see cref="Temperament"/> entity
        /// </summary>
        /// <param name="temperament">
        /// <see cref="DogTemperament"/> instance with which to update existing record
        /// </param>
        /// <param name="userId">User Id string</param>
        /// <returns>Updated (mapped) <see cref="DogTemperament"/> instance</returns>
        public async Task<bool> UpdateTemperament(DogTemperament temperament, string userId)
        {
            var temperamentEntity = await _repository.FindTemperament(temperament.DogId);
            _mapper.Map(temperament, temperamentEntity);
            
            temperamentEntity.LastModified = DateTime.Now;           
            temperamentEntity.LastModifiedBy = userId;

            var updated = await _repository.SaveTemperament(temperamentEntity);

            if (updated)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion Service Methods
    }
}
