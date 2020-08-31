using DogMatch.Domain.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DogMatch.Domain.Data.Repositories
{
    public interface IColorRepository
    {
        /// <summary>
        /// Save dog color entities by adding range
        /// </summary>
        /// <param name="colors"><see cref="IEnumerable{Color}"/> colors to save</param>
        Task SaveColors(IEnumerable<Color> colors);

        /// <summary>
        /// Removes dog color entities by removing range
        /// </summary>
        /// <param name="colors"><see cref="IEnumerable{Color}"/> colors to remove</param>
        Task RemoveColors(IEnumerable<Color> colors);
    }
}
