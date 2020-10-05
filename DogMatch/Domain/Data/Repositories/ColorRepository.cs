using DogMatch.Domain.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DogMatch.Domain.Data.Repositories
{
    public class ColorRepository : IColorRepository
    {
        #region DI
        private readonly DogMatchDbContext _context;
        private readonly DbSet<Color> _dbSet;

        public ColorRepository(DogMatchDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Color>();
        }
        #endregion DI

        #region Repository Methods
        /// <summary>
        /// Save dog color entities by adding range
        /// </summary>
        /// <param name="colors"><see cref="IEnumerable{Color}"/> colors to save</param>
        public async Task SaveColors(IEnumerable<Color> colors)
        {
            _dbSet.AddRange(colors);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes dog color entities by removing range
        /// </summary>
        /// <param name="colors"><see cref="IEnumerable{Color}"/> colors to remove</param>
        public async Task RemoveColors(IEnumerable<Color> colors)
        {
            _dbSet.RemoveRange(colors);
            await _context.SaveChangesAsync();
        }
        #endregion Repository Methods
    }
}
