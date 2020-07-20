using DogMatch.Server.Data;
using DogMatch.Shared.Models;
using DogMatch.Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace DogMatch.Server.Services
{
    public class DogService : IDogService
    {
        private readonly DogMatchDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageService _imgService;

        public DogService(DogMatchDbContext context, IMapper mapper, IImageService imgService)
        {
            _context = context;
            _mapper = mapper;
            _imgService = imgService;
        }

        #region Service Methods
        /// <summary>
        /// Gets single <see cref="Dog"/> by Id
        /// </summary>        
        /// <param name="id">Dog Id integer</param>
        /// <returns>The found (mapped) <see cref="Dog"/> instance if it exists</returns>
        public async Task<Dog> GetDog(int id)
        {            
            Dogs dogEntity = await _context.Dogs
                .Include(d => d.Owner)
                .Include(d => d.ProfileImage)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            return _mapper.Map<Dog>(dogEntity);
        }

        /// <summary>
        /// Gets all active dogs
        /// </summary>        
        /// <returns>Mapped, enumerated <see cref="Dog"/></returns>
        public async Task<IEnumerable<Dog>> GetAllDogs()
        {
            var dogsEntity = await _context.Dogs
                .AsNoTracking()
                .Include(d => d.Owner)
                .Include(d => d.ProfileImage)
                .Where(d => d.IsDeleted != true)
                .ToListAsync();

            var dogs = _mapper.Map<IEnumerable<Dog>>(dogsEntity);

            return dogs;
        }

        /// <summary>
        /// Create and save new <see cref="Dogs"/> entity
        /// </summary>
        /// <param name="dog"></param>
        /// <param name="userId"></param>
        /// <returns><see cref="Dog"/> instance mapped from new entity with SQL generated Id</returns>
        public async Task<Dog> CreateDog(Dog dog, string userId)
        {
            var dogEntity = _mapper.Map<Dogs>(dog);

            var now = DateTime.Now;
            dogEntity.Created = now;
            dogEntity.LastModified = now;
            dogEntity.OwnerId = userId;
            dogEntity.CreatedBy = userId;
            dogEntity.LastModifiedBy = userId;

            _context.Dogs.Add(dogEntity);
            await _context.SaveChangesAsync();

            dog.Id = dogEntity.Id;

            return dog;
        }

        /// <summary>
        /// Updates single <see cref="Dogs"/> entity, mapped from <see cref="Dog"/> object
        /// </summary>
        /// <param name="id">dog Id int</param>
        /// <param name="dog"><see cref="Dog"/> object with updated values</param>
        /// <param name="userId">user Id string</param>
        /// <returns><see cref="bool"/>, true if entity successfully updated and saved</returns>
        public async Task<bool> UpdateDog(int id, Dog dog, string userId)
        {
            Dogs dogEntity = await _context.Dogs.FindAsync(id);
            _mapper.Map(dog, dogEntity);

            if (dog.ProfileImage != null && dog.Extension != null)
            {
                dogEntity.ProfileImageId = await _imgService.SaveProfileImage(dog.ProfileImage, dog.Extension, dog.Id, userId);
            }

            dogEntity.LastModified = DateTime.Now;
            dogEntity.LastModifiedBy = userId;            

            _context.Entry(dogEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DogExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }
        #endregion Service Methods

        #region Interal Methods
        /// <summary>
        /// Checks if dog exists by dog Id
        /// </summary>
        /// <param name="id">Dog Id int</param>
        /// <returns><see cref="bool"/>, true is dog exists</returns>
        private bool DogExists(int id) =>
            _context.Dogs.Any(d => d.Id == id);

        #endregion Internal Methods
    }
}
