using DogMatch.Shared.Globals;
using DogMatch.Shared.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;

namespace DogMatch.Client.Services
{
    /// <summary>
    /// Manages state for Dog Biography components and pages
    /// </summary>
    public class BiographyState
    {
        #region Properties / Variables
        public DogBiography Biography { get; set; }
        public AccordionExpansion Expanded { get; set; }
        public bool isAuthorized = false;
        public event Action OnChange;
        #endregion Properties / Variables

        #region DI
        private readonly HttpClient _http;
        private readonly NavigationService _navigate;
        private readonly NotificationMsgService _notification;

        public BiographyState(HttpClient httpInstance, NavigationService navigate, NotificationMsgService notification)
        {
            _http = httpInstance;
            _navigate = navigate;
            _notification = notification;
        }
        #endregion DI

        #region Methods / WebApi Calls
        /// <summary>
        /// Calls WebApi to get Biography for single dog.
        /// </summary>        
        /// <param name="id">Dog Id <see cref="int" /></param>
        public async Task GetBiography(int id)
        {
            // initialize state properties
            NewExpansionState();
            NewBiography();

            // get dog's biography
            HttpResponseMessage response = await _http.GetAsync($"api/Biography/{id}");

            if (response.IsSuccessStatusCode)
            {
                // set biography instance into state
                Biography = await response.Content.ReadFromJsonAsync<DogBiography>();
                isAuthorized = true;

                // notify subscribers state has changed
                if (Biography != null)
                    NotifyStateChanged();
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // user is not authorized to edit this biography (likely not owner)
                _notification.DisplayMessage(NotificationType.NotAuthorizedOwnerEditError);
                _navigate.ToAllDoggos();
                return;
            }
        }

        /// <summary>
        /// Calls WebApi to update/save Biography for single dog.
        /// </summary>        
        public async Task<bool> UpdateBiography()
        {
            HttpResponseMessage response = await _http.PutAsJsonAsync($"api/Biography/{Biography.DogId}", Biography);

            if (response.IsSuccessStatusCode)
            {
                // biography saved successfully
                _notification.DisplayMessage(NotificationType.BiographySaved, Biography.DogName);
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
                _notification.DisplayMessage(NotificationType.BiographyError, Biography.DogName);
                return false;
            }
        }

        /// <summary>
        /// Changes the expansion state of biography text area expandable panels
        /// </summary>
        /// <param name="changingExpansion">The expansion panel <see cref="BiographyExpansion"/> type to be changed</param>
        /// <param name="isSelf">
        /// <see cref="bool"/> - true if the selected panel (panel from which the request came) is the same as the desination panel (opening/closing itself)
        /// </param>
        public async Task ChangeExpanded(BiographyExpansion changingExpansion, bool isSelf = false)
        {
            switch (changingExpansion)
            {
                case BiographyExpansion.About:
                    UpdateAccordionState("AboutExpanded", isSelf);
                    break;
                case BiographyExpansion.Memory:
                    UpdateAccordionState("MemoryExpanded", isSelf);
                    break;
                case BiographyExpansion.Food:
                    UpdateAccordionState("FoodExpanded", isSelf);
                    break;
                case BiographyExpansion.Toy:
                    UpdateAccordionState("ToyExpanded", isSelf);
                    break;
                case BiographyExpansion.Sleep:
                    UpdateAccordionState("SleepExpanded", isSelf);
                    break;
                case BiographyExpansion.Walk:
                    UpdateAccordionState("WalkExpanded", isSelf);
                    break;
                case BiographyExpansion.AllClosed:
                    CleanExpandedState();
                    NotifyStateChanged();
                    break;
                default:
                    CleanExpandedState();
                    NotifyStateChanged();
                    break;
            }
            await UpdateBiography();
        }

        /// <summary>
        /// Saves Biography data and navigates to new page based on <see cref="Navigate"/> type passed by user
        /// </summary>
        /// <param name="destination">the destniation page <see cref="Navigate"/> type</param>
        public async Task SaveAndNavigate(Navigate destination)
        {
            bool success = await UpdateBiography();

            // navigate if biography is updated successfully
            if (success)
            {
                switch (destination)
                {
                    case Navigate.ToProfile:
                        _navigate.ToProfile(Biography.DogId);
                        break;
                    case Navigate.ToDetails:
                        _navigate.ToUpdateDoggo(Biography.DogId);
                        break;
                    case Navigate.ToTemperament:
                        _navigate.ToTemperament(Biography.DogId);
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

        #region Internal Methods
        /// <summary>
        /// Invokes OnChange Action to notify subscribers state has changed.
        /// </summary>    
        private void NotifyStateChanged() => OnChange?.Invoke();

        /// <summary>
        /// Uses reflection to change the <see cref="bool"/> property expansion value using the passed property name string 
        /// </summary>
        /// <param name="name">name <see cref="string"/> of <see cref="bool"/> <see cref="AccordionExpansion"/> property to be changed</param>
        /// <param name="isSelf">
        /// <see cref="bool"/> - true if the selected panel (panel from which the request came) is the same as the desination panel (opening/closing itself)
        /// </param>
        private void UpdateAccordionState(string name, bool isSelf = false)
        {
            // if not changing itself then reset all properties to false / closed
            if (!isSelf)
                CleanExpandedState();

            // use reflection to get property from property name string passed to method
            PropertyInfo property = Expanded.GetType().GetProperty(name);

            // get current value of property 
             bool value = (bool)property.GetValue(Expanded);

            // update property value (!current_propery_value)
            property.SetValue(Expanded, !value, null);

            // notify subscribers that state has changed
            NotifyStateChanged();
        }
        #endregion Internal Methods

        #region Initialize Classes
        /// <summary>
        /// Initializes new <see cref="DogBiography"/> instance in state.
        /// </summary>        
        public void NewBiography() => Biography = new DogBiography();

        /// <summary>
        /// Initializes new <see cref="AccordionExpansion"/> instance in state,
        /// and sets first property (AboutExpanded) to true
        /// </summary>
        public void NewExpansionState() =>
            Expanded = new AccordionExpansion() { AboutExpanded = true };

        /// <summary>
        /// Initializes new <see cref="AccordionExpansion"/> instance in state,
        /// and sets all values as false
        /// </summary>
        public void CleanExpandedState() => Expanded = new AccordionExpansion();

        #endregion Initialize Classes
    }

    /// <summary>
    /// Class for bools that track expansion state of accordians.
    /// Each Accordian contains a single Biography text area field.
    /// </summary>
    public class AccordionExpansion
    {
        public bool AboutExpanded { get; set; }
        public bool MemoryExpanded { get; set; }
        public bool FoodExpanded { get; set; }
        public bool ToyExpanded { get; set; }
        public bool SleepExpanded { get; set; }
        public bool WalkExpanded { get; set; }
    }
}
