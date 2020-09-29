using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;
using DogMatch.Shared.Globals;
using DogMatch.Shared.Models;

namespace DogMatch.Client.Services
{
    /// <summary>
    /// Manages client state for Temperament components and pages.
    /// </summary>   
    public class TemperamentState
    {
        #region Properties / Variables
        public DogTemperament Temperament { get; set; }
        public event Action OnChange;
        public int tabIndex;
        public bool isAuthorized = false;
        #endregion Properties / Variables

        #region DI
        private readonly HttpClient _http;
        private readonly NavigationService _navigate;
        private readonly NotificationMsgService _notification;

        public TemperamentState(HttpClient httpInstance, NavigationService navigate, NotificationMsgService notification)
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
        private void NotifyStateChanged() => OnChange?.Invoke();

        /// <summary>
        /// Calls WebApi to get Temperament for single dog.
        /// </summary>        
        /// <param name="id">Dog Id <see cref="int" /></param>
        public async Task GetTemperament(int id)
        {
            tabIndex = 0;
            NewTemperament();

            // get dog's temperament
            HttpResponseMessage response = await _http.GetAsync($"api/Temperament/{id}");

            if (response.IsSuccessStatusCode)
            {
                // set biography instance into state
                Temperament = await response.Content.ReadFromJsonAsync<DogTemperament>();

                // user is authorized to edit temperament
                isAuthorized = true;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // user is not authorized to edit this biography (likely not owner)
                _notification.DisplayMessage(NotificationType.NotAuthorizedOwnerEditError);
                _navigate.ToAllDoggos();
                return;
            }

            // notify subscribers state has changed
            if (Temperament != null)
                NotifyStateChanged();
        }

        /// <summary>
        /// Calls WebApi to update Temperament for single dog.
        /// </summary>        
        public async Task<bool> UpdateTemperament()
        {
            HttpResponseMessage response = await _http.PutAsJsonAsync($"api/Temperament/{Temperament.DogId}", Temperament);

            if (response.IsSuccessStatusCode)
            {
                _notification.DisplayMessage(NotificationType.TemperamentSaved, Temperament.DogName);
                return true;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // user is not authorized to update dog biography (likely not owner)
                _notification.DisplayMessage(NotificationType.NotAuthorizedOwnerEditError);
                _navigate.ToAllDoggos();
                return false;
            }
            else
            {
                _notification.DisplayMessage(NotificationType.TemperamentError, Temperament.DogName);
                return false;
            }
        }

        /// <summary>
        /// Changes value of templated rating field using reflection.
        /// </summary>        
        /// <param name="value">new value of property</param>
        /// <param name="name">name of property to be changed</param>
        public void ChangeValue(int value, string name)
        {
            PropertyInfo property = Temperament.GetType().GetProperty(name);
            property.SetValue(Temperament, value, null);
            NotifyStateChanged();
        }

        /// <summary>
        /// Changes value of radzen radio bools using reflection.
        /// </summary>        
        /// <param name="value">new value of property</param>
        /// <param name="name">name of property to be changed</param>
        public void ChangeBoolValue(int value, string name)
        {
            bool? val = null;

            if (value == 1)
            {
                val = true;
            }
            else if (value == 2)
            {
                val = false;
            }

            PropertyInfo property = Temperament.GetType().GetProperty(name);
            property.SetValue(Temperament, val, null);
            NotifyStateChanged();
        }

        /// <summary>
        /// Directly changes temperament tab/step <see cref="tabIndex"/>.
        /// </summary>        
        /// <param name="index">new step number</param>
        public async Task TabChanged(int index)
        {
            tabIndex = index;
            NotifyStateChanged();
            await UpdateTemperament();
        }

        /// <summary>
        /// Incrementally changes temperament tab/step <see cref="tabIndex"/>, 
        /// and calls temperament update (save) method
        /// </summary>        
        /// <param name="direction"><see cref="TemperamentDirection"/> type</param>
        public async Task StepperChange(TemperamentDirection direction)
        {         
            switch (direction)
            {
                case TemperamentDirection.Forward:
                    tabIndex++;
                    break;
                case TemperamentDirection.Back:
                    tabIndex--;
                    break;
                case TemperamentDirection.Profile:
                    _navigate.ToProfile(Temperament.DogId);
                    break;
                case TemperamentDirection.Biography:
                    _navigate.ToBiography(Temperament.DogId);
                    break;
            }            

            NotifyStateChanged();
            await UpdateTemperament();
        }

        /// <summary>
        /// Saves Temperament data and navigates to new page based on <see cref="Navigate"/> type passed by user
        /// </summary>
        /// <param name="destination">the destniation page <see cref="Navigate"/> type</param>
        public async Task SaveAndNavigate(Navigate destination)
        {
            bool success = await UpdateTemperament();
            
            // navigate to destination if temperament saved successfully
            if (success)
            {
                switch (destination)
                {
                    case Navigate.ToProfile:
                        _navigate.ToProfile(Temperament.DogId);
                        break;
                    case Navigate.ToDetails:
                        _navigate.ToUpdateDoggo(Temperament.DogId);
                        break;
                    case Navigate.ToBiography:
                        _navigate.ToBiography(Temperament.DogId);
                        break;
                    case Navigate.ToDogAlbum:
                        _navigate.ToDogAlbum(Temperament.DogId);
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

        #region Initialize New Temperament
        /// <summary>
        /// Initializes new <see cref="DogTemperament"/> instance in state.
        /// </summary>        
        public void NewTemperament() => Temperament = new DogTemperament();
        #endregion Initialize New Temperament
    }
}
