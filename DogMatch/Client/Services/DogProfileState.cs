using System;
using System.Collections.Generic;
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
        public Dog Doggo { get; set; }
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
        /// <param name="id">Dog Id integer</param>
        public async Task GetDoggo(int id)
        {
            NewDoggo();
            Doggo = await _http.GetFromJsonAsync<Dog>($"api/Doggo/{id}");
            NotifyStateChanged();
        }       

        /// <summary>
        /// Calls WebApi to delete single dog by dog Id, refreshes doggos
        /// </summary>
        /// <param name="dogId">Dog Id <see cref="int"/></param>
        /// <param name="currentUser">Current Username <see cref="string"/></param>
        public async Task DeleteDog(int dogId, string currentUser)
        {
            // get Dog
            Dog doggo = await _http.GetFromJsonAsync<Dog>($"api/Doggo/{dogId}");

            // soft delete dog if request user is owner
            if (doggo.Owner == currentUser)
            {
                HttpResponseMessage response = await _http.DeleteAsync($"api/Doggo/{dogId}");
                DeleteDogResponse delResponse = await response.Content.ReadFromJsonAsync<DeleteDogResponse>();

                if (delResponse == DeleteDogResponse.Success)
                {
                    _notification.DisplayMessage(NotificationType.DogDeleted, doggo.Name);
                    _navigate.ToAllDoggos();
                }
                else if (delResponse == DeleteDogResponse.Unauthorized)
                {
                    // service delcares user unathorized to make delete request (non-owner)
                    _notification.DisplayMessage(NotificationType.DogDeleteUnauthorized, Doggo.Name);
                }
                else
                {
                    // failed response
                    _notification.DisplayMessage(NotificationType.DogDeleteError, doggo.Name);
                }
            }
            else
            {
                // user not authorization to delete dog (non-owner)
                _notification.DisplayMessage(NotificationType.DogDeleteUnauthorized, Doggo.Name);
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
        public void NewDoggo() => Doggo = new Dog();
        #endregion Initialize Classes
    }
}
