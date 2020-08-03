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
    /// Manages client state for Dog components and pages.
    /// </summary> 
    [Authorize]
    public class DogState
    {
        #region Properties / Variables / DI
        public Dog dog { get; set; }
        public IEnumerable<Dog> doggos { get; set; }
        public DateTime today = DateTime.Now;
        public event Action OnChange;

        // DI
        private readonly HttpClient _http;
        private readonly NavigationService _navigate;
        private readonly NotificationMsgService _notification;

        public DogState(HttpClient httpInstance, NavigationService navigate, NotificationMsgService notification)
        {
            _http = httpInstance;
            _navigate = navigate;
            _notification = notification;
        }
        #endregion Properties / Variables / DI

        #region Methods / WebApi Calls
        /// <summary>
        /// Invokes OnChange Action to notify subscribers state has changed.
        /// </summary>
        private void NotifyStateChanged() => OnChange?.Invoke();

        /// <summary>
        /// Calls WebApi that returns all doggos and sets them into state.
        /// </summary>        
        public async Task GetAllDoggos() =>
            doggos = await _http.GetFromJsonAsync<IEnumerable<Dog>>("/api/Doggo");

        /// <summary>
        /// Calls WebApi to get single dog and set dog into state.
        /// </summary>        
        /// <param name="id">Dog Id integer</param>
        public async Task GetDoggo(int id)
        {
            NewDoggo();
            dog = await _http.GetFromJsonAsync<Dog>($"api/Doggo/{id}");
        }

        /// <summary>
        /// Calls WebApi to POST new dog.
        /// </summary>        
        public async Task CreateDoggo()
        {
            var response = await _http.PostAsJsonAsync("api/Doggo/", dog);
            dog = await response.Content.ReadFromJsonAsync<Dog>();

            _notification.DisplayMessage(NotificationType.DogCreated, dog.Name);
            _navigate.ToUpdateDoggo(dog.Id);
        }

        /// <summary>
        /// Calls WebApi to update single dog.
        /// </summary>        
        public async Task UpdateDoggo()
        {
            var response = await _http.PutAsJsonAsync($"api/Doggo/{dog.Id}", dog);

            if (response.IsSuccessStatusCode)
            {
                _notification.DisplayMessage(NotificationType.DogUpdated, dog.Name);
            }
            else
            {
                _notification.DisplayMessage(NotificationType.DogUpdateError, dog.Name);
            }
        }

        /// <summary>
        /// Calls WebApi to delete single dog by dog Id, refreshes doggos
        /// </summary>
        /// <param name="dogId">Dog Id <see cref="int"/></param>
        /// <param name="currentUser">Current Username <see cref="string"/></param>
        public async Task DeleteDog(int dogId, string currentUser)
        {
            // get Dog
            var doggo = await _http.GetFromJsonAsync<Dog>($"api/Doggo/{dogId}");

            // soft delete dog if request user is owner
            if (doggo.Owner == currentUser)
            {
                var response = await _http.DeleteAsync($"api/Doggo/{dogId}");
                var delResponse = await response.Content.ReadFromJsonAsync<DeleteDogResponse>();

                if (delResponse == DeleteDogResponse.Success)
                {
                    _notification.DisplayMessage(NotificationType.DogDeleted, doggo.Name);

                    // refresh all doggogs and notify subscriber state has changed
                    await GetAllDoggos();
                    NotifyStateChanged();
                }
                else if (delResponse == DeleteDogResponse.Unauthorized)
                {
                    // service delcares user unathorized to make delete request (non-owner)
                    _notification.DisplayMessage(NotificationType.DogDeleteUnauthorized, dog.Name);
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
                _notification.DisplayMessage(NotificationType.DogDeleteUnauthorized, dog.Name);
            }
        }
        #endregion Methods / WebApi Calls        

        #region Data Methods
        /// <summary>
        /// Initializes new Dog object dog in state.
        /// </summary>
        public void NewDoggo() => dog = new Dog();

        /// <summary>
        /// Converts birthday <see cref="DateTime?"/> to short date string.
        /// </summary>        
        /// <param name="bday">nullable birthday DateTime</param>
        /// <returns>short DateTime string</returns>
        public string GetBirthday(DateTime? bday) =>
            bday.Value.ToShortDateString();

        /// <summary>
        /// Sets dog.Birthday and gets current age (dog.Age) string based on birthday <see cref="DateTime?"/>.
        /// </summary>        
        /// <param name="date">birthday <see cref="DateTime?"/></param>
        /// <returns><see cref="string"/> containing current age</returns>
        public Task GetAge(DateTime? date)
        {
            dog.Birthday = date;

            var today = DateTime.Today;
            var age = 0;

            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (date.Value.Year * 100 + date.Value.Month) * 100 + date.Value.Day;

            age = (a - b) / 10000;

            // if less than 1 year old then determine age in months
            if (age < 1)
            {
                int m1 = (today.Month - date.Value.Month);
                int m2 = (today.Year - date.Value.Year) * 12;
                int months = m1 + m2;

                dog.Age = "Age: " + months + " months old";
            }
            else if (age == 1)
            {
                dog.Age = "Age: " + age + " year old";
            }
            else
            {
                dog.Age = "Age: " + age + " years old";
            }

            return Task.CompletedTask;
        }
        #endregion Data Methods

        #region Styles
        public string ColStyle = "display: block; margin-left: auto; margin-right: auto;";
        public string CardStyle = "display: block; margin-left: auto; margin-right: auto; width: 60%; margin-bottom: 20px; height:200px;";
        #endregion Styles
    }
}
