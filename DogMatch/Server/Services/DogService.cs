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

        public async Task<Dog> GetDog(int id)
        {            
            Dogs dogEntity = await _context.Dogs
                .Include(d => d.Owner)
                .Include(d => d.ProfileImage)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            var dog = _mapper.Map<Dog>(dogEntity);

            return dog;
        }

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

        public async Task<bool> UpdateDog(int id, Dog dog, string userId)
        {
            Dogs dogEntity = await _context.Dogs.FindAsync(id);
            _mapper.Map<Dog, Dogs>(dog, dogEntity);

            if (dog.ProfileImage != null && dog.Extension != null)
            {
                dogEntity.ProfileImageId = await _imgService.HandleProfileImage(dog.ProfileImage, dog.Extension, dog.Id, userId);
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

        private bool DogExists(int id)
        {
            return _context.Dogs.Any(e => e.Id == id);
        }
    }
}
