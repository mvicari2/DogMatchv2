using DogMatch.Domain.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DogMatch.Domain.Data.Repositories
{
    public class BiographyRepository : IBiographyRepository
    {
        #region DI
        private readonly DogMatchDbContext _context;
        private readonly DbSet<Biography> _dbSet;

        public BiographyRepository(DogMatchDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Biography>();
        }
        #endregion DI

        #region Repository Methods
        /// <summary>
        /// Find single Biography entity by dogId.
        /// </summary>        
        /// <param name="dogId">Dog Id <see cref="int"/></param>
        /// <returns><see cref="Biography"/> entity</returns>
        public async Task<Biography> FindBiography(int dogId)
        {
            Biography bio = await _dbSet
                .Where(b => b.DogId == dogId)
                .SingleOrDefaultAsync();

            if (bio != null)
            {
                bio.Dog = await _context.Dogs
                .Where(d => d.Id == dogId)
                .Include(d => d.Owner)
                .SingleOrDefaultAsync();
            }

            return bio;
        }

        /// <summary>
        /// Writes new Biography entity to database.
        /// </summary>        
        /// <param name="bio"><see cref="Biography"/> entity object</param>
        /// <returns>New <see cref="Biography"/> entity with sql generated id.</returns>
        public async Task<Biography> CreateNewBiography(Biography bio)
        {
            _dbSet.Add(bio);
            await _context.SaveChangesAsync();

            // include Dog and Owner relationship
            bio.Dog = await _context.Dogs
                .Where(d => d.Id == bio.DogId)
                .Include(d => d.Owner)
                .SingleOrDefaultAsync();

            return bio;
        }

        /// <summary>
        /// Save/Update Biography entity
        /// </summary>
        /// <param name="bio"><see cref="Biography"/> entity object.</param>
        /// <returns>Bool to confirm changes saved.</returns>
        public async Task<bool> SaveBiography(Biography bio)
        {
            _context.Entry(bio).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BiographyExists(bio.Id))
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
        /// Checks if Biography record exists
        /// </summary>
        /// <param name="id">Biography Id</param>
        /// <returns>bool, true if Biography exists.</returns>
        private bool BiographyExists(int id) =>
            _dbSet.Any(t => t.Id == id);

        #endregion Internal
    }
}

