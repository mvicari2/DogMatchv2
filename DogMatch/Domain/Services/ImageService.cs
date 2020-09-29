using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using DogMatch.Domain.Data.Repositories;
using DogMatch.Domain.Data.Models;
using DogMatch.Shared.Models;
using System.Collections.Generic;
using AutoMapper;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace DogMatch.Domain.Services
{
    public class ImageService : IImageService
    {
        #region DI
        private readonly IDogImagesRepository _repository;
        private readonly IDogRepository _dogRepository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ILogger<ImageService> _logger;

        public ImageService(IDogImagesRepository repository, IDogRepository dogRepository,IConfiguration config, IMapper mapper, ILogger<ImageService> logger)
        {
            _repository = repository;
            _dogRepository = dogRepository;
            _config = config;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion DI

        #region Service Methods
        /// <summary>
        /// Get single dog with all active/non-deleted dog album images
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns><see cref="DogAlbumImages"/> object for single dog</returns>
        public async Task<DogAlbumImages> GetDogAlbumImages(int id)
        {
            // get dog entity with active/non-deleted album images
            Dogs dog = await _dogRepository.FindDogWithAlbumImagesById(id);

            return new DogAlbumImages()
            {
                DogId = dog.Id,
                DogName = dog.Name,
                Images = _mapper.Map<List<AlbumImage>>(dog.AlbumImages)
            };
        }

        /// <summary>
        /// Update Dog Album images (adds any new images, removes any deleted images that are already saved)
        /// </summary>
        /// <param name="dogAlbum"><see cref="DogAlbumImages"/> object with updated images</param>
        /// <param name="userId">User Id <see cref="string"/> for current request user</param>
        /// <returns>success <see cref="bool"/></returns>
        public async Task<bool> UpdateDogAlbumImages(DogAlbumImages dogAlbum, string userId)
        {
            if (dogAlbum.Images.Count() > 0)
            {
                try
                {
                    // separate image groups, ignore previously saved unchanged images
                    // new image group for saving new images 
                    IEnumerable<AlbumImage> newImages = dogAlbum.Images
                        .Where(i => i.Id == 0 && !(i.Delete ?? false));

                    // and list of image Id's to soft delete
                    List<int> imageIdsToDelete = dogAlbum.Images
                         .Where(i => (i.Delete ?? false) && i.Id != 0)
                         .Select(i => i.Id)
                         .ToList();

                    // save new images
                    if (newImages.Count() > 0)
                        await SaveNewAlbumImages(newImages, dogAlbum.DogId, userId);

                    // soft delete any images flagged for deletion
                    if (imageIdsToDelete.Count > 0)
                        await _repository.SoftDeleteImageRange(imageIdsToDelete);

                    _logger.LogInformation("Image Service Updated Dog Album Images Successfully");

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Image Service error while updating Dog Album Images.");
                    return false;
                }                      
            }
            
            // no dog album images, return true
            return true;
        }

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
            string filename = SaveImageToDisk(imgStr, extension, true);

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
        /// <param name="isProfileImage"></param>
        /// <returns>Generated filename <see cref="string"/> for image</returns>
        private string SaveImageToDisk(string imgStr, string extension, bool isProfileImage)
        { 
            // clean up image string and convert to byte array
            imgStr = imgStr.Substring(22, imgStr.Length - 22);
            byte[] imgBytes = Convert.FromBase64String(imgStr);

            // randomize file name excessively using new Guid string and datetime.now ticks
            string filename = Guid.NewGuid().ToString();
            filename = filename + DateTime.Now.Ticks.ToString() + extension;

            try
            {
                // write byte array to disk
                File.WriteAllBytes(_config.GetValue<string>(isProfileImage ? "FilePaths:ProfileImageDir" : "FilePaths:AlbumImageDir") + filename, imgBytes);
                return filename;
            }
            catch
            {
                return string.Empty;
            }            
        }

        /// <summary>
        /// Creates new <see cref="DogImages"/> entities for new dog album images, calls repository to save new entities, and calls save to disk method to save image files to disk
        /// </summary>
        /// <param name="newImages"><see cref="IEnumerable{AlbumImage}"/> new images to save</param>
        /// <param name="dogId">Dog Id <see cref="int"/></param>
        /// <param name="userId">User Id <see cref="string"/></param>
        private async Task SaveNewAlbumImages(IEnumerable<AlbumImage> newImages, int dogId, string userId)
        {
            List<DogImages> ImageEntities = new List<DogImages>();
            DateTime now = DateTime.Now;

            // save each image to disk and create new DogImage entities for all new images
            foreach (var image in newImages)
            {
                // save image to disk, generates and returns internal filename string
                string filename = SaveImageToDisk(image.ImageString, image.Extension, false);

                // Add new DogImages entity to list  
                ImageEntities.Add(new DogImages()
                {
                    DogId = dogId,
                    Filename = filename,
                    IsProfileImage = false,
                    Created = now,
                    CreatedBy = userId
                });
            }

            // save new album images from list
            await _repository.SaveDogAlbumImages(ImageEntities);
        }
        #endregion Interal Methods
    }
}
