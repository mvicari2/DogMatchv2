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
        public Dog Doggo { get; set; }
        public IEnumerable<Dog> Doggos { get; set; }
        public IEnumerable<Dog> OwnersDogs { get; set; }
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
            Doggos = await _http.GetFromJsonAsync<IEnumerable<Dog>>("/api/Doggo");

        /// <summary>
        /// Calls WebApi that returns all doggos owned by current user and sets them into state.
        /// </summary>        
        public async Task GetOwnersDoggos() =>
            OwnersDogs = await _http.GetFromJsonAsync<IEnumerable<Dog>>("/api/Owners");

        /// <summary>
        /// Calls WebApi to get single dog and set dog into state.
        /// </summary>        
        /// <param name="id">Dog Id integer</param>
        public async Task GetDoggo(int id)
        {
            NewDoggo();
            Doggo = await _http.GetFromJsonAsync<Dog>($"api/Doggo/{id}");
        }

        /// <summary>
        /// Calls WebApi to POST new dog.
        /// </summary>        
        public async Task CreateDoggo()
        {
            HttpResponseMessage response = await _http.PostAsJsonAsync("api/Doggo/", Doggo);
            Doggo = await response.Content.ReadFromJsonAsync<Dog>();

            _notification.DisplayMessage(NotificationType.DogCreated, Doggo.Name);
            _navigate.ToUpdateDoggo(Doggo.Id);
        }

        /// <summary>
        /// Calls WebApi to update single dog.
        /// </summary>        
        public async Task UpdateDoggo()
        {
            HttpResponseMessage response = await _http.PutAsJsonAsync($"api/Doggo/{Doggo.Id}", Doggo);

            if (response.IsSuccessStatusCode)
            {
                _notification.DisplayMessage(NotificationType.DogUpdated, Doggo.Name);
            }
            else
            {
                _notification.DisplayMessage(NotificationType.DogUpdateError, Doggo.Name);
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
            Dog doggo = await _http.GetFromJsonAsync<Dog>($"api/Doggo/{dogId}");

            // soft delete dog if request user is owner
            if (doggo.Owner == currentUser)
            {
                HttpResponseMessage response = await _http.DeleteAsync($"api/Doggo/{dogId}");
                DeleteDogResponse delResponse = await response.Content.ReadFromJsonAsync<DeleteDogResponse>();

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

        #region Data Methods
        /// <summary>
        /// Initializes new <see cref="Dog"/> instance dog in state.
        /// </summary>
        public void NewDoggo() => Doggo = new Dog();

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
            Doggo.Birthday = date;

            DateTime today = DateTime.Today;
            int a = (today.Year * 100 + today.Month) * 100 + today.Day;
            int b = (date.Value.Year * 100 + date.Value.Month) * 100 + date.Value.Day;

            int age = (a - b) / 10000;

            // if less than 1 year old then determine age in months
            if (age < 1)
            {
                int m1 = (today.Month - date.Value.Month);
                int m2 = (today.Year - date.Value.Year) * 12;
                int months = m1 + m2;

                Doggo.Age = "Age: " + months + " months old";
            }
            else if (age == 1)
            {
                Doggo.Age = "Age: " + age + " year old";
            }
            else
            {
                Doggo.Age = "Age: " + age + " years old";
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
