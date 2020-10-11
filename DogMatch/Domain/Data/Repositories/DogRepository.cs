using DogMatch.Domain.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Extensions.Logging;

namespace DogMatch.Domain.Data.Repositories
{
    public class DogRepository : IDogRepository
    {
        #region DI
        private readonly DogMatchDbContext _context;
        private readonly DbSet<Dogs> _dbSet;
        private readonly ILogger<DogRepository> _logger;

        public DogRepository(DogMatchDbContext context, ILogger<DogRepository> logger)
        {
            _context = context;
            _dbSet = context.Set<Dogs>();
            _logger = logger;
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
            .Include(d => d.DogProfileImage)
            .Include(d => d.Colors)
            .SingleOrDefaultAsync(d => d.Id == id && d.IsDeleted != true);

        /// <summary>
        /// Find single <see cref="Dogs"/> entity with navigation properties for Owner, Profile Image, Colors, Biography, Temperament and Album Images included
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns>Single <see cref="Dogs"/> entity for full profile</returns>
        public async Task<Dogs> FindFullDogProfileById(int id)
        {
            var dog = await _dbSet
                .AsNoTracking()
                .Include(d => d.Owner)
                .Include(d => d.DogProfileImage)
                .Include(d => d.Colors)
                .Include(d => d.Biography)
                .Include(d => d.Temperament)
                .SingleOrDefaultAsync(d => d.Id == id && d.IsDeleted != true);

            // for now (until efcore 5 / lambdas within include), get album images separately for active/non-deleted album images
            if (dog != null)
                dog.AlbumImages = _context.DogImages
                    .AsNoTracking()
                    .Where(i => i.DogId == id && 
                        !(i.IsProfileImage ?? false) && 
                        !(i.IsDeleted ?? false))
                    .ToList();

            return dog;
        }

        /// <summary>
        /// Finds all active (non-deleted) Dogs
        /// </summary>        
        /// <returns><see cref="IEnumerable{Dogs}" /><see cref="Dogs"/> All active Dogs</returns>
        public async Task<IEnumerable<Dogs>> FindAllDogs() =>
            await _dbSet
            .AsNoTracking()
            .Where(d => d.IsDeleted != true)
            .Include(d => d.Owner)
            .Include(d => d.DogProfileImage)
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
            .Include(d => d.DogProfileImage)
            .ToListAsync();

        /// <summary>
        /// Finds single <see cref="Dogs"/> entity and includes all active, non-deleted Dog Album Images (<see cref="DogImages"/>)
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns>Single <see cref="Dogs"/> entity object instance with album images</returns>
        public async Task<Dogs> FindDogWithAlbumImagesById(int id)
        {
            Dogs dog = await _dbSet
                .AsNoTracking()
                // lambda within include not available in efcore 3.x, but will be in efcore 5.0 (saving for future upgrade)
                //.Include(d => d.AlbumImages.Where(a => !(a.IsDeleted ?? false)
                //    && !(a.IsProfileImage ?? false)))
                .SingleOrDefaultAsync(d => d.Id == id && d.IsDeleted != true);

            // for now (until efcore 5), get album images separately for active/non-deleted album images
            if (dog != null)
                dog.AlbumImages = _context.DogImages
                    .AsNoTracking()
                    .Where(i => i.DogId == id && 
                        !(i.IsProfileImage ?? false) && 
                        !(i.IsDeleted ?? false))
                    .ToList();

            return dog;
        }

        /// <summary>
        /// Writes new, single <see cref="Dogs"/> entity to database.
        /// </summary>        
        /// <param name="dog"><see cref="Dogs"/> entity object</param>
        /// <returns>The new <see cref="Dogs"/> entity with sql generated id <see cref="int"/>.</returns>
        public async Task<Dogs> SaveNewDog(Dogs dog)
        {
            _dbSet.Add(dog);
            await _context.SaveChangesAsync();
            _logger.LogTrace($"New Dog {dog.Id} created by {dog.CreatedBy}");

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
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!DogExists(dog.Id))
                    _logger.LogError(ex, $"Failed saving updated Dog entity, Dog doesn't exist (attempted dog id {dog.Id}).");
                else
                    _logger.LogError(ex, $"Db Update Concurrency Exception while saving updated Dog entity for dog id {dog.Id}.");

                return false;
            }
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
