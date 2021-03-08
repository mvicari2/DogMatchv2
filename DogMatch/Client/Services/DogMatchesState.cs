using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DogMatch.Shared.Globals;
using DogMatch.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace DogMatch.Client.Services
{
    /// <summary>
    /// Manages client state for Dog Profile components and pages.
    /// </summary> 
    [Authorize]
    public class DogMatchesState
    {
        #region Properties / Variables
        public DogMatches DogMatch { get; set; }
        public bool LoadingMatches { get; set; }
        public event Action OnChange;
        public event Action CloseModal;
        #endregion Properties / Variables

        #region DI
        private readonly HttpClient _http;
        private readonly NavigationService _navigate;
        private readonly NotificationMsgService _notification;

        public DogMatchesState(HttpClient httpInstance, NavigationService navigate, NotificationMsgService notification)
        {
            _http = httpInstance;
            _navigate = navigate;
            _notification = notification;
        }
        #endregion DI

        #region Methods / WebApi Calls
        /// <summary>
        /// Invokes OnChange Action to notify subscribers state has changed.
        /// </summary>
        public void NotifyStateChanged() => OnChange?.Invoke();

        /// <summary>
        /// Invokes CloseModal Action to notify subscribers modal must close.
        /// </summary>
        public void CloseMatchesModal() => CloseModal?.Invoke();

        /// <summary>
        /// Calls WebApi to get current Dog Matches for a single dog and 
        /// sets matches into state. WebApi returns some current dog demographic 
        /// data and top matches (up to 10) for current dog
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        public async Task GetMatches(int id)
        {
            NewDogMatches();

            // call WebApi to get dog's matches (top 10)
            HttpResponseMessage response = await _http.GetAsync($"api/Matches/{id}");

            if (response.IsSuccessStatusCode)
            {
                // set dog profile instance into state
                DogMatch = await response.Content.ReadFromJsonAsync<DogMatches>();
                LoadingMatches = false;
                NotifyStateChanged();
                return;
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // unauthorized to get matches, display message and close modal
                _notification.DisplayMessage(NotificationType.MatchesUnauthorized);
                CloseMatchesModal();
                NotifyStateChanged();
                return;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // dog not found / does not exist
                _notification.DisplayMessage(NotificationType.DogNotFound, "Dog Profile");
                CloseMatchesModal();
                return;
            }
            else
            {
                // general error message
                _notification.DisplayMessage(
                    NotificationType.GeneralError,
                    "There was an error getting dog's Matches."
                );
                CloseMatchesModal();
                return;
            }
        }

        /// <summary>
        /// Close matches modal and navigate to a new dog profile
        /// </summary>
        /// <param name="id">Dog id <see cref="int"/> for new dog profile to navigate to</param>
        public void NavigateAndCloseModal(int id)
        {
            CloseMatchesModal();
            _navigate.ToProfile(id);
        }
        #endregion Methods / WebApi Calls

        #region Initialize Class
        /// <summary>
        /// Initializes new <see cref="DogMatches"/> instance in state.
        /// </summary>
        public void NewDogMatches() => DogMatch = new DogMatches();
        #endregion Initialize Class
    }
}
