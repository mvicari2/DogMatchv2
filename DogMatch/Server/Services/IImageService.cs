using System.Threading.Tasks;
using DogMatch.Server.Data.Models;

namespace DogMatch.Server.Services
{
    public interface IImageService
    {
        /// <summary>
        /// Saves new <see cref="Images"/> entity instance and calls method to write file to disk
        /// </summary>        
        /// <param name="imgStr">Base64 encoded string of user uploaded image</param>
        /// <param name="extension">File extension of uploaded image</param>
        /// <param name="dogId">Dog Id int for dog that owns image</param>
        /// <param name="userId">User Id string for user that uploaded image</param>
        /// <returns>SQL generated image Id for newly saved image</returns>
        Task<int?> SaveProfileImage(string imgStr, string extension, int dogId, string userId);
    }
}
