using DogMatch.Server.Data;
using DogMatch.Server.Data.Models;
using System;
using System.Threading.Tasks;
using AutoMapper;
using System.IO;

namespace DogMatch.Server.Services
{
    public class ImageService : IImageService
    {
        private readonly DogMatchDbContext _context;
        private readonly IMapper _mapper;       

        public ImageService(DogMatchDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int?> HandleProfileImage(string imgStr, string extension, int dogId, string userId)
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
    }
}
