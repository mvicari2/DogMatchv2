using DogMatch.Server.Data.Models;
using DogMatch.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogMatch.Server.Data.Repositories
{
    public class DogRepository : IDogRepository
    {
        #region DI
        private readonly DogMatchDbContext _context;
        private readonly DbSet<Dogs> _dbSet;

        public DogRepository(DogMatchDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Dogs>();
        }
        #endregion DI

        #region Repository Methods
        /// <summary>
        /// Find single <see cref="Dogs"/> entity by dog Id <see cref="int"/>.
        /// </summary>        
        /// <param name="id">Dog Id integer</param>
        /// <returns>single <see cref="Dogs"/> entity</returns>
        public async Task<Dogs> FindDogById(int id) =>
            await _dbSet
            .AsNoTracking()
            .Include(d => d.Owner)
            .Include(d => d.ProfileImage)
            .SingleOrDefaultAsync(d => d.Id == id && d.IsDeleted != true);

        /// <summary>
        /// Finds all active (non-deleted) Dogs
        /// </summary>        
        /// <returns><see cref="IEnumerable{Dogs}" /><see cref="Dogs"/> All active Dogs</returns>
        public async Task<IEnumerable<Dogs>> FindAllDogs() =>
            await _dbSet
            .AsNoTracking()
            .Where(d => d.IsDeleted != true)
            .Include(d => d.Owner)
            .Include(d => d.ProfileImage)
            .ToListAsync();

        /// <summary>
        /// Find Dogs by Owner (finds all dogs where current user is owner)
        /// </summary>        
        /// <param name="userId">User Id <see cref="string"/> for owner</param>
        /// <returns><see cref="IEnumerable{Dogs}" /> All dogs owned by user</returns>
        public async Task<IEnumerable<Dogs>> FindDogsByOwner(string userId) =>
            await _dbSet
            .AsNoTracking()
            .Where(d => d.IsDeleted != true && d.OwnerId == userId)
            .Include(d => d.Owner)
            .Include(d => d.ProfileImage)
            .ToListAsync();

        /// <summary>
        /// Writes new, single <see cref="Dogs"/> entity to database.
        /// </summary>        
        /// <param name="dog"><see cref="Dogs"/> entity object</param>
        /// <returns>The new <see cref="Dogs"/> entity with sql generated id <see cref="int"/>.</returns>
        public async Task<Dogs> SaveNewDog(Dogs dog)
        {
            _dbSet.Add(dog);
            await _context.SaveChangesAsync();
            return dog;
        }

        /// <summary>
        /// Save/Update single <see cref="Dogs"/> entity
        /// </summary>
        /// <param name="dog"><see cref="Dogs"/> entity object.</param>
        /// <returns><see cref="bool"/> to confirm changes saved.</returns>
        public async Task<bool> SaveDog(Dogs dog)
        {
            _context.Entry(dog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DogExists(dog.Id))
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
        #endregion Repository Methods

        #region Internal
        /// <summary>
        /// Checks if <see cref="Dogs"/> record exists
        /// </summary>
        /// <param name="id">Dog Id</param>
        /// <returns><see cref="bool"/>, true if <see cref="Dogs"/> record exists.</returns>
        private bool DogExists(int id) =>
            _dbSet.Any(d => d.Id == id && d.IsDeleted != false);

        #endregion Internal
    }
}
