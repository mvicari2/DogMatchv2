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
    public class DogProfileState
    {
        #region Properties / Variables
        public DogProfile Profile { get; set; }
        public event Action OnChange;
        #endregion Properties / Variables

        #region DI
        private readonly HttpClient _http;
        private readonly NavigationService _navigate;
        private readonly NotificationMsgService _notification;

        public DogProfileState(HttpClient httpInstance, NavigationService navigate, NotificationMsgService notification)
        {
            _http = httpInstance;
            _navigate = navigate;
            _notification = notification;
        }
        #endregion DI

        #region Methods / WebApi Calls
        /// <summary>
        /// Calls WebApi to get single dog and set dog into state.
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        public async Task GetDoggo(int id)
        {
            NewProfile();
            Profile = await _http.GetFromJsonAsync<DogProfile>($"api/DogProfile/{id}");
            NotifyStateChanged();
        }       

        /// <summary>
        /// Calls WebApi to delete single dog by dog Id, navigates to All Doggos if successful
        /// </summary>
        /// <param name="dogId">Dog Id <see cref="int"/> for dog to be deleted</param>
        /// <param name="dogName">Dog name string for dog to be deleted<see cref="string"/></param>
        public async Task DeleteDog(int dogId, string dogName)
        {
            HttpResponseMessage response = await _http.DeleteAsync($"api/Doggo/{dogId}");

            if (response.IsSuccessStatusCode)
            {
                _notification.DisplayMessage(NotificationType.DogDeleted, dogName);
                _navigate.ToAllDoggos();
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // user unauthorized to make delete request (likely non-owner)
                _notification.DisplayMessage(NotificationType.DogDeleteUnauthorized, dogName);
                _navigate.ToAllDoggos();
            }
            else
            {
                // failure response
                _notification.DisplayMessage(NotificationType.DogDeleteError, dogName);
            }
        }        
        #endregion Methods / WebApi Calls

        #region Internal Methods
        /// <summary>
        /// Invokes OnChange Action to notify subscribers state has changed.
        /// </summary>
        private void NotifyStateChanged() => OnChange?.Invoke();        
        #endregion Internal Methods

        #region Initialize Classes
        /// <summary>
        /// Initializes new <see cref="Dog"/> instance dog in state.
        /// </summary>
        public void NewProfile() => Profile = new DogProfile();
        #endregion Initialize Classes
    }
}
