using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using DogMatch.Domain.Data.Repositories;
using DogMatch.Domain.Data.Models;

namespace DogMatch.Domain.Services
{
    public class ImageService : IImageService
    {        
        private readonly IDogImagesRepository _repository;
        private readonly IConfiguration _config;

        public ImageService(IDogImagesRepository repository, IConfiguration config)
        {
            _repository = repository;
            _config = config;
        }

        #region Service Methods
        /// <summary>
        /// Creates (and saves) new <see cref="DogImages"/> entity and calls method to write dog profile image file to disk
        /// </summary>        
        /// <param name="imgStr">Base64 encoded string of user uploaded image</param>
        /// <param name="extension">File extension of uploaded image</param>
        /// <param name="dogId">Dog Id int for dog that owns image</param>
        /// <param name="userId">User Id string for user that uploaded image</param>
        /// <returns>SQL generated image Id <see cref="int"/> for newly saved image</returns>
        public async Task<int?> SaveProfileImage(string imgStr, string extension, int dogId, string userId)
        {
            // save image to disk, generates and returns internal filename string
            string filename = SaveImageToDisk(imgStr, extension);

            // create new DogImages entity using uploaded profile image details
            DogImages img = new DogImages()
            {
                DogId = dogId,
                Filename = filename,
                IsProfileImage = true,
                Created = DateTime.Now,
                CreatedBy = userId
            };

            // save new DogImages entity 
            DogImages image = await _repository.SaveDogImage(img);

            return image.Id;
        }
        #endregion Service Methods

        #region Interal Methods
        /// <summary>
        /// Generates internal filename and writes image file to disk
        /// </summary>        
        /// <param name="imgStr">Base64 encoded <see cref="string"/> of user uploaded image</param>
        /// <param name="extension">File extension <see cref="string"/> of uploaded image</param>
        /// <returns>Generated filename <see cref="string"/> for image</returns>
        private string SaveImageToDisk(string imgStr, string extension)
        { 
            // clean up image string and convert to byte array
            imgStr = imgStr.Substring(22, imgStr.Length - 22);
            byte[] imgArr = Convert.FromBase64String(imgStr);

            // randomize file name excessively using new Guid string and datetime.now ticks
            string filename = Guid.NewGuid().ToString();
            filename = filename + DateTime.Now.Ticks.ToString() + extension;

            try
            {
                // write byte array to disk
                File.WriteAllBytes(_config.GetValue<string>("FilePaths:ProfileImageDir") + filename, imgArr);
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
