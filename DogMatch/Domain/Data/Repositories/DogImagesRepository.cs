using DogMatch.Domain.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogMatch.Domain.Data.Repositories
{
    public class DogImagesRepository : IDogImagesRepository
    {
        #region DI
        private readonly DogMatchDbContext _context;
        private readonly DbSet<DogImages> _dbSet;

        public DogImagesRepository(DogMatchDbContext context)
        {
            _context = context;
            _dbSet = context.Set<DogImages>();
        }
        #endregion DI

        #region Public Repository Methods
        /// <summary>
        /// Saves new single <see cref="DogImages"/> entity to database
        /// </summary>
        /// <param name="image"><see cref="DogImages"/> entity to write to database</param>
        /// <returns><see cref="DogImages"/> entity instance w/SQL generated Id</returns>
        public async Task<DogImages> SaveDogImage(DogImages image)
        {
            _dbSet.Add(image);
            await _context.SaveChangesAsync();

            return image;
        }

        /// <summary>
        /// Save new <see cref="DogImages"/> entities by adding range from passed <see cref="IEnumerable{DogImages}"/>
        /// </summary>
        /// <param name="images"><see cref="IEnumerable{DogImages}"/> images to save</param>
        public async Task SaveDogAlbumImages(IEnumerable<DogImages> images)
        {
            _dbSet.AddRange(images);
            await _context.SaveChangesAsync();
        }
        
        /// <summary>
        /// Soft Deleted Dog Images by Range using List of Dog Image Id's
        /// </summary>
        /// <param name="imageIdList"><see cref="IEnumerable{int}"/> Enumerated Dog Image Id's to soft delete</param>
        public async Task SoftDeleteImageRange(IEnumerable<int> imageIdList)
        {
            // find dog image entities using id list
            IEnumerable<DogImages> images = await _dbSet
                .Where(i => imageIdList.Contains(i.Id))
                .ToListAsync();

            // set entities as deleted (soft delete)
            foreach (var image in images)
                image.IsDeleted = true;

            // save
            await _context.SaveChangesAsync();
        }
        #endregion Public Repository Methods
    }
}
