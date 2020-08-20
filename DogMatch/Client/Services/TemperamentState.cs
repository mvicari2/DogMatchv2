using System;
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
        #region Properties / Variables / DI
        public DogTemperament Temperament { get; set; }
        public event Action OnChange;
        public int tabIndex;        

        // DI
        private readonly HttpClient _http;
        private readonly NavigationService _navigate;
        private readonly NotificationMsgService _notification;

        public TemperamentState(HttpClient httpInstance, NavigationService navigate, NotificationMsgService notification)
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
        /// Calls WebApi to get Temperament for single dog.
        /// </summary>        
        /// <param name="id">Dog Id <see cref="int" /></param>
        public async Task GetTemperament(int id)
        {
            tabIndex = 0;
            NewTemperament();
            Temperament = await _http.GetFromJsonAsync<DogTemperament>($"api/Temperament/{id}");

            if (Temperament != null)
            {
                NotifyStateChanged();
            }
        }

        /// <summary>
        /// Calls WebApi to update Temperament for single dog.
        /// </summary>        
        public async Task UpdateTemperament()
        {
            HttpResponseMessage response = await _http.PutAsJsonAsync($"api/Temperament/{Temperament.DogId}", Temperament);

            if (response.IsSuccessStatusCode)
            {
                _notification.DisplayMessage(NotificationType.TemperamentSaved, Temperament.DogName);
            }
            else
            {
                _notification.DisplayMessage(NotificationType.TemperamentError, Temperament.DogName);
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
        #endregion Methods / WebApi Calls

        #region New Temperament
        /// <summary>
        /// Initializes new <see cref="DogTemperament"/> instance in state.
        /// </summary>        
        public void NewTemperament() => Temperament = new DogTemperament();

        #endregion New Temperament
    }
}
