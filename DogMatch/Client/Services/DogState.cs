using System;
using System.Collections.Generic;
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
    /// Manages client state for Dog components and pages.
    /// </summary> 
    [Authorize]
    public class DogState
    {
        #region Properties / Variables
        public Dog Doggo { get; set; }
        public IEnumerable<Dog> Doggos { get; set; }
        public IEnumerable<Dog> OwnersDogs { get; set; }
        public DogsFilter Filter { get; set; }
        public DogsFilter OwnersDogFilter { get; set; }
        public DateTime today = DateTime.Now;
        public int? initialWeight = null;
        public bool loading = false;
        public bool dogsListLoading = false;
        public bool ownersDogsLoading = false;
        public event Action OnChange;
        #endregion Properties / Variables

        #region DI
        private readonly HttpClient _http;
        private readonly NavigationService _navigate;
        private readonly NotificationMsgService _notification;

        public DogState(HttpClient httpInstance, NavigationService navigate, NotificationMsgService notification)
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
        /// Calls WebApi to get (and search/filter) all dogs or owner's dogs
        /// </summary>
        /// <param name="filter"><see cref="DogsFilter"/> object to post to WebApi</param>
        public async Task GetDogsAndFilter(DogsFilter filter)
        {
            // show loading animation
            switch (filter.DogListType)
            {
                case DogListType.AllDogs:
                    dogsListLoading = true;
                    break;
                case DogListType.Owners:
                    ownersDogsLoading = true;
                    break;
                default:
                    Filter = filter;
                    break;
            }       

            NotifyStateChanged();
             
            // post filter object, returns dogs
            HttpResponseMessage response = await _http.PostAsJsonAsync($"/api/GetDogs", filter);

            if (response.IsSuccessStatusCode)
            {
                var dogs = await response.Content.ReadFromJsonAsync<IEnumerable<Dog>>();

                // set returned dogs into state based on which dog list is active
                switch (filter.DogListType)
                {
                    case DogListType.AllDogs:
                        Doggos = dogs;
                        Filter = filter;
                        dogsListLoading = false;
                        break;
                    case DogListType.Owners:
                        OwnersDogs = dogs;
                        OwnersDogFilter = filter;
                        ownersDogsLoading = false;
                        break;
                    default:
                        Filter = filter;
                        break;
                }
                
                NotifyStateChanged();
            }
            else
            {
                // api call failed, display error message notification
                _notification.DisplayMessage(
                    NotificationType.GeneralError, 
                    "Could not return the requested dogs, please reload."
                );
            }
        }

        /// <summary>
        /// Calls WebApi to get single dog and set dog into state.
        /// </summary>        
        /// <param name="id">Dog Id integer</param>
        /// <param name="requestUser">User Id <see cref="string"/> for user requesting doggo</param>
        public async Task GetDoggo(int id, string requestUser)
        {
            NewDoggo();

            // call WebApi to get dog's basic details
            HttpResponseMessage response = await _http.GetAsync($"api/Doggo/{id}");

            if (response.IsSuccessStatusCode)
            {
                // set Dog instance into state
                Doggo = await response.Content.ReadFromJsonAsync<Dog>();

                // set intial dog weight for MatSlider component and authorized editor bool
                initialWeight = Doggo.Weight;
                loading = false;
                NotifyStateChanged();
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // user is not authorized to edit this dog (likely not owner)
                _notification.DisplayMessage(NotificationType.NotAuthorizedOwnerEditError);
                _navigate.ToAllDoggos();
                return;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // dog not found / does not exist
                _notification.DisplayMessage(NotificationType.DogNotFound, "Basic Details Profile");
                _navigate.ToAllDoggos();
                return;
            }
            else
            {
                // general error message
                _notification.DisplayMessage(
                    NotificationType.GeneralError, 
                    "There was an error getting dog's Basic Details Profile."
                );
                _navigate.ToAllDoggos();
                return;
            }
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
        /// <param name="requestUser">User Id <see cref="string"/> for user updating dog</param>
        /// <returns><see cref="bool"/> for successful response from WebApi</returns>      
        public async Task<bool> UpdateDoggo(string requestUser)
        {
            // show loading animation while updating dog
            loading = true;

            // ensure user updating dog is dog owner (also checks on server)
            if (Doggo.Owner == requestUser)
            {
                HttpResponseMessage response = await _http.PutAsJsonAsync($"api/Doggo/{Doggo.Id}", Doggo);

                if (response.IsSuccessStatusCode)
                {
                    _notification.DisplayMessage(NotificationType.DogUpdated, Doggo.Name);
                    loading = false;
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // user not authorized to update dog
                    _notification.DisplayMessage(NotificationType.NotAuthorizedOwnerEditError);
                    _navigate.ToAllDoggos();
                    return false;
                }
                else
                {
                    _notification.DisplayMessage(NotificationType.DogUpdateError, Doggo.Name);
                    return false;
                }
            }
            else
            {
                // user not authorized to update dog
                _notification.DisplayMessage(NotificationType.NotAuthorizedOwnerEditError);
                _navigate.ToAllDoggos();
                return false;
            }
        }

        /// <summary>
        /// Calls WebApi to delete single dog by dog Id, refreshes doggos
        /// </summary>
        /// <param name="dogId">Dog Id <see cref="int"/></param>
        /// <param name="dogName">Current Username <see cref="string"/></param>
        /// <param name="dogListType"><see cref="DogListType"/> active dog list type</param>
        public async Task DeleteDog(int dogId, string dogName, DogListType dogListType)
        {
            HttpResponseMessage response = await _http.DeleteAsync($"api/Doggo/{dogId}");

            if (response.IsSuccessStatusCode)
            {
                _notification.DisplayMessage(NotificationType.DogDeleted, dogName);

                // refresh all doggogs or owner's dogs and notify subscribers state has changed
                await GetDogsAndFilter(
                    dogListType == DogListType.AllDogs ? 
                    Filter : 
                    OwnersDogFilter
                );
                NotifyStateChanged();
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // user unauthorized to make delete request (likely non-owner)
                _notification.DisplayMessage(NotificationType.DogDeleteUnauthorized, dogName);
            }
            else
            {
                // failure response
                _notification.DisplayMessage(NotificationType.DogDeleteError, dogName);
            }
        }

        /// <summary>
        /// Saves Doggo data and navigates to new page based on <see cref="Navigate"/> type passed by user
        /// </summary>
        /// <param name="destination">the destniation page <see cref="Navigate"/> type</param>
        /// <param name="requestUser">Request user name string</param>
        public async Task SaveAndNavigate(Navigate destination, string requestUser)
        {
            bool success = await UpdateDoggo(requestUser);

            if (success)
            {
                switch (destination)
                {
                    case Navigate.ToProfile:
                        _navigate.ToProfile(Doggo.Id);
                        break;
                    case Navigate.ToTemperament:
                        _navigate.ToTemperament(Doggo.Id);
                        break;
                    case Navigate.ToBiography:
                        _navigate.ToBiography(Doggo.Id);
                        break;
                    case Navigate.ToDogAlbum:
                        _navigate.ToDogAlbum(Doggo.Id);
                        break;
                    case Navigate.ToOwnersPortal:
                        _navigate.ToOwnerPortal();
                        break;
                    case Navigate.ToAllDoggos:
                        _navigate.ToAllDoggos();
                        break;
                }
            }
        }
        #endregion Methods / WebApi Calls

        #region Property Methods
        /// <summary>
        /// Update <see cref="Doggo"/> Gender (on change)
        /// </summary>
        /// <param name="type"></param>
        public void UpdateGender(DogGenderTypes type)
        {
            switch (type)
            {
                case DogGenderTypes.Female:
                    Doggo.Gender = "female";
                    break;
                case DogGenderTypes.Male:
                    Doggo.Gender = "male";
                    break;
            }
        }

        /// <summary>
        /// Update <see cref="Doggo"/> birthday (on change)
        /// </summary>
        /// <param name="bday">Dog's birthday <see cref="DateTime?"/></param>
        public void UpdateBirthday(DateTime? bday)
        {
            GetAge(bday);
            Doggo.Birthday = bday;
        }        
        #endregion Property Methods            

        #region Internal Methods
        /// <summary>
        /// Sets dog.Birthday and gets current age (dog.Age) string based on birthday <see cref="DateTime?"/>.
        /// </summary>        
        /// <param name="date">birthday <see cref="DateTime?"/></param>
        /// <returns><see cref="string"/> containing current age</returns>
        private Task GetAge(DateTime? date)
        {
            Doggo.Birthday = date;

            if (!date.HasValue)
            {
                Doggo.Age = null;
                NotifyStateChanged();
                return Task.CompletedTask;
            }

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

            NotifyStateChanged();
            return Task.CompletedTask;
        }
        #endregion Internal Methods

        #region Initialize Classes
        /// <summary>
        /// Initializes new <see cref="Dog"/> instance dog in state.
        /// </summary>
        public void NewDoggo() => Doggo = new Dog();

        /// <summary>
        /// Initialize new DogsFilter instance and set in state based on current active dog list
        /// </summary>
        /// <param name="dogListType"><see cref="DogListType"/> current active dog list type</param>
        public void NewDogFilter(DogListType dogListType)
        {
            var filter = new DogsFilter()
            {
                DogListType = dogListType,
                AgeRange = new AgeRange()
                {
                    Start = 0,
                    End = 30
                },
                WeightRange = new WeightRange()
                {
                    Start = 0,
                    End = 150
                }
            };

            // set in state
            switch (dogListType)
            {
                case DogListType.AllDogs:
                    Filter = filter;
                    break;
                case DogListType.Owners:
                    OwnersDogFilter = filter;
                    break;
                default:
                    Filter = filter;
                    break;
            }
        }
        #endregion Initialize Classes
    }
}
