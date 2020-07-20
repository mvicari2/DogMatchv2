using DogMatch.Server.Data;
using DogMatch.Server.Data.Models;
using System;
using System.Threading.Tasks;
using System.IO;

namespace DogMatch.Server.Services
{
    public class ImageService : IImageService
    {
        private readonly DogMatchDbContext _context;
        public ImageService(DogMatchDbContext context) => _context = context;

        #region Service Methods
        /// <summary>
        /// Saves new <see cref="Images"/> entity instance and calls method to write file to disk
        /// </summary>        
        /// <param name="imgStr">Base64 encoded string of user uploaded image</param>
        /// <param name="extension">File extension of uploaded image</param>
        /// <param name="dogId">Dog Id int for dog that owns image</param>
        /// <param name="userId">User Id string for user that uploaded image</param>
        /// <returns>SQL generated image Id for newly saved image</returns>
        public async Task<int?> SaveProfileImage(string imgStr, string extension, int dogId, string userId)
        {
            var filename = SaveImageToDisk(imgStr, extension);

            Images img = new Images()
            {
                DogId = dogId,
                Filename = filename,
                IsProfileImage = true,
                Created = DateTime.Now,
                CreatedBy = userId
            };

            _context.Images.Add(img);
            await _context.SaveChangesAsync();

            return img.Id;
        }
        #endregion Service Methods

        #region Interal Methods
        /// <summary>
        /// Generates internal filename and writes image file to disk
        /// </summary>        
        /// <param name="imgStr">Base64 encoded string of user uploaded image</param>
        /// <param name="extension">File extension of uploaded image</param>
        /// <returns>Generated filename string for image</returns>
        private string SaveImageToDisk(string imgStr, string extension)
        { 
            var rootDir = "Images/ProfileImages/";

            // clean up image string and convert to byte array
            imgStr = imgStr.Substring(22, imgStr.Length - 22);
            byte[] imgArr = Convert.FromBase64String(imgStr);

            // randomize file name excessively
            var filename = Guid.NewGuid().ToString();
            filename = filename + DateTime.Now.Ticks.ToString() + extension;
            try
            {
                File.WriteAllBytes(rootDir + filename, imgArr);
                return filename;
            }
            catch
            {
                return string.Empty;
            }            
        }
        #endregion Interal Methods
    }
}
