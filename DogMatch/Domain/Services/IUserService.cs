using System.Threading.Tasks;

namespace DogMatch.Domain.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Get's the User Id <see cref="string"/> for a Dog's owner by Dog Id
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns>Owner/User Id <see cref="string"/></returns>
        Task<string> GetOwnerIdByDogId(int id);
    }
}
