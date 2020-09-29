using DogMatch.Domain.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DogMatch.Domain.Data.Repositories
{
    public interface IDogImagesRepository
    {
        /// <summary>
        /// Saves new single <see cref="DogImages"/> entity to database
        /// </summary>
        /// <param name="image"><see cref="DogImages"/> entity to write to database</param>
        /// <returns><see cref="DogImages"/> entity instance w/SQL generated Id</returns>
        Task<DogImages> SaveDogImage(DogImages image);

        /// <summary>
        /// Save new <see cref="DogImages"/> entities by adding range from passed <see cref="IEnumerable{DogImages}"/>
        /// </summary>
        /// <param name="images"><see cref="IEnumerable{DogImages}"/> images to save</param>
        Task SaveDogAlbumImages(IEnumerable<DogImages> images);

        /// <summary>
        /// Soft Deleted Dog Images by Range using List of Dog Image Id's
        /// </summary>
        /// <param name="imageIdList"><see cref="IEnumerable{int}"/> Enumerated Dog Image Id's to soft delete</param>
        Task SoftDeleteImageRange(IEnumerable<int> imageIdList);
    }
}
