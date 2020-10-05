using DogMatch.Domain.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace DogMatch.Domain.Data.Repositories
{
    public class TemperamentRepository : ITemperamentRepository
    {
        #region DI
        private readonly DogMatchDbContext _context;
        private readonly DbSet<Temperament> _dbSet;
        private readonly ILogger<TemperamentRepository> _logger;

        public TemperamentRepository(DogMatchDbContext context, ILogger<TemperamentRepository> logger)
        {
            _context = context;
            _dbSet = context.Set<Temperament>();
            _logger = logger;
        }
        #endregion DI

        #region Repository Methods
        /// <summary>
        /// Find single temperament entity by dogId.
        /// </summary>        
        /// <param name="dogId">Dog Id integer</param>
        /// <returns><see cref="Temperament"/> entity</returns>
        public async Task<Temperament> FindTemperament(int dogId)
        {
            Temperament temperament = await _dbSet
                .Where(t => t.DogId == dogId)              
                .SingleOrDefaultAsync();

            if (temperament != null)
            {
                temperament.Dog = await _context.Dogs
                .Where(d => d.Id == dogId)
                .Include(d => d.Owner)
                .SingleOrDefaultAsync();
            }            

            return temperament;
        }

        /// <summary>
        /// Writes new temperament entity to database.
        /// </summary>        
        /// <param name="temperament"><see cref="Temperament"/> entity object</param>
        /// <returns>New <see cref="Temperament"/> entity with sql generated id.</returns>
        public async Task<Temperament> CreateNewTemperament(Temperament temperament)
        {
            _dbSet.Add(temperament);
            await _context.SaveChangesAsync();

            // include Dog and Owner relationship
            temperament.Dog = await _context.Dogs
                .Where(d => d.Id == temperament.DogId)
                .Include(d => d.Owner)
                .SingleOrDefaultAsync();

            return temperament;
        }

        /// <summary>
        /// Save/Update Temperament entity
        /// </summary>
        /// <param name="temperament">Temperament entity object.</param>
        /// <returns>Bool to confirm changes saved.</returns>
        public async Task<bool> SaveTemperament(Temperament temperament)
        {
            _context.Entry(temperament).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TemperamentExists(temperament.Id))
                    _logger.LogError(ex, $"Failed saving updated Dog Temperament entity, record doesn't exist (attempted Temperament id: {temperament.Id}).");
                else
                    _logger.LogError(ex, $"Db Update Concurrency Exception while saving updated Dog Temperament for Temperament id {temperament.Id}.");

                return false;
            }            
        }
        #endregion Repository Methods

        #region Internal
        /// <summary>
        /// Checks if Temperament record exists
        /// </summary>
        /// <param name="id">Temperament Id</param>
        /// <returns>bool, true if temperament exists.</returns>
        private bool TemperamentExists(int id) =>
            _dbSet.Any(t => t.Id == id);

        #endregion Internal

    }
}
