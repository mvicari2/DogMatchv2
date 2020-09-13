using DogMatch.Domain.Data.Repositories;
using System.Threading.Tasks;

namespace DogMatch.Domain.Services
{
    public class UserService : IUserService
    {
        #region DI
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) => 
            _userRepository = userRepository;
        #endregion DI

        #region Public Service Methods
        /// <summary>
        /// Get's the User Id <see cref="string"/> for a Dog's owner by Dog Id
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns>Owner/User Id <see cref="string"/></returns>
        public async Task<string> GetOwnerIdByDogId(int id) =>
            await _userRepository.FindDogOwnerUserIdByDogId(id);
        #endregion Public Service Methods

    }
}
