using System.Threading.Tasks;
using DogMatch.Domain.Data.Models;
using DogMatch.Shared.Models;

namespace DogMatch.Domain.Services
{
    public interface IImageService
    {
        /// <summary>
        /// Get single dog with all active/non-deleted dog album images
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns><see cref="DogAlbumImages"/> object for single dog</returns>
        Task<DogAlbumImages> GetDogAlbumImages(int id);

        /// <summary>
        /// Update Dog Album images (adds any new images, removes any deleted images that are already saved)
        /// </summary>
        /// <param name="dogAlbum"><see cref="DogAlbumImages"/> object with updated images</param>
        /// <param name="userId">User Id <see cref="string"/> for current request user</param>
        /// <returns>success <see cref="bool"/></returns>
        Task<bool> UpdateDogAlbumImages(DogAlbumImages dogAlbum, string userId);

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
