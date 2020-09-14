using System.Threading.Tasks;
using DogMatch.Domain.Data.Models;

namespace DogMatch.Domain.Services
{
    public interface IImageService
    {
        /// <summary>
        /// Creates (and saves) new <see cref="DogImages"/> entity and calls method to write dog profile image file to disk
        /// </summary>        
        /// <param name="imgStr">Base64 encoded string of user uploaded image</param>
        /// <param name="extension">File extension of uploaded image</param>
        /// <param name="dogId">Dog Id int for dog that owns image</param>
        /// <param name="userId">User Id string for user that uploaded image</param>
        /// <returns>SQL generated image Id <see cref="int"/> for newly saved image</returns>
        Task<int?> SaveProfileImage(string imgStr, string extension, int dogId, string userId);
    }
}
