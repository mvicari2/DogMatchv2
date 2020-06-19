using System.Threading.Tasks;

namespace DogMatch.Server.Services
{
    public interface IImageService
    {
        Task<int?> HandleProfileImage(string imgStr, string filename, int dogId, string userId);
    }
}
