using DogMatch.Domain.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DogMatch.Domain.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        #region DI
        private readonly DogMatchDbContext _context;
        private readonly DbSet<DogMatchUser> _dbSet;

        public UserRepository(DogMatchDbContext context)
        {
            _context = context;
            _dbSet = context.Set<DogMatchUser>();
        }
        #endregion DI

        #region Repository Methods
        /// <summary>
        /// Find Dog's Owner's User Id by Dog Id
        /// </summary>        
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns>User Id <see cref="string" /> for Dog's Owner</returns>
        public async Task<string> FindDogOwnerUserIdByDogId(int dogId) =>
            await _context.Dogs
                .AsNoTracking()
                .Where(d => d.Id == dogId)
                .Include(d => d.Owner)
                .Select(d => d.Owner.Id)
                .SingleOrDefaultAsync();
        #endregion Repository Methods
    }
}
