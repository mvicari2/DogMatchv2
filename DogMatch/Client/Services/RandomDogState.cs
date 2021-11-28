using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DogMatch.Shared.Globals;
using DogMatch.Shared.Models;

namespace DogMatch.Client.Services
{
    /// <summary>
    /// Manages client state for Random Dog components
    /// </summary>
    public class RandomDogState
    {
        #region Properties / Variables
        public event Action OnChange;
        public RandomDog RandomDog { get; set; }
        public bool Loading { get; set; }
        #endregion Properties / Variables

        #region DI
        private readonly HttpClient _http;
        private readonly NotificationMsgService _notification;

        public RandomDogState(HttpClient httpInstance, NotificationMsgService notification)
        {
            _http = httpInstance;
            _notification = notification;
        }
        #endregion DI

        #region Methods / WebApi Calls
        /// <summary>
        /// Calls WebApi to get a single random dog
        /// </summary>
        public async Task GetRandomDog()
        {
            // set loading true (for loading animation, etc.)
            Loading = true;
            NotifyStateChanged();

            // get random dog
            RandomDog = await _http.GetFromJsonAsync<RandomDog>("api/RandomDog");

            if (RandomDog == null)
            {
                // error notification: random dog could not be fetched
                _notification.DisplayMessage(
                    NotificationType.GeneralError,
                    "There was an error fetching a random dog."
                );
            }
            else
            {
                Loading = false;
                NotifyStateChanged();
            }
        }
        #endregion Methods / WebApi Calls

        #region Internal Methods
        /// <summary>
        /// Invokes OnChange Action to notify subscribers state has changed.
        /// </summary>
        private void NotifyStateChanged() => OnChange?.Invoke();
        #endregion Internal Methods
    }
}
