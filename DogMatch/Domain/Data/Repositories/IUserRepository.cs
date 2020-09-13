using System.Threading.Tasks;

namespace DogMatch.Domain.Data.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Find Dog's Owner's User Id by Dog Id
        /// </summary>        
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns>User Id <see cref="string" /> for Dog's Owner</returns>
        Task<string> FindDogOwnerUserIdByDogId(int dogId);
    }
}
