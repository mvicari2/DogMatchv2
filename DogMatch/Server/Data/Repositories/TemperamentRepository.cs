using DogMatch.Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DogMatch.Server.Data.Repositories
{
    public class TemperamentRepository : ITemperamentRepository
    {        
        private readonly DogMatchDbContext _context;
        private readonly DbSet<Temperament> _dbSet;

        public TemperamentRepository(DogMatchDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Temperament>();
        }

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
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TemperamentExists(temperament.Id))
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
        /// Checks if Temperament record exists
        /// </summary>
        /// <param name="id">Temperament Id</param>
        /// <returns>bool, true if temperament exists.</returns>
        private bool TemperamentExists(int id) =>
            _dbSet.Any(t => t.Id == id);

        #endregion Internal

    }
}
