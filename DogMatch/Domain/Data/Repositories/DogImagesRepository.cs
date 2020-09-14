using DogMatch.Domain.Data.Models;
using Microsoft.EntityFrameworkCore;
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
        #endregion Public Repository Methods
    }
}
