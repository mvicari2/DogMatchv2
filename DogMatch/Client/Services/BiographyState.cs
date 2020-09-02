using DogMatch.Shared.Globals;
using DogMatch.Shared.Models;
using System;
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
        #region Properties / Variables / DI
        public DogBiography Biography { get; set; }
        public AccordionExpansion Expanded { get; set; }
        public event Action OnChange;

        // DI
        private readonly HttpClient _http;
        private readonly NavigationService _navigate;
        private readonly NotificationMsgService _notification;

        public BiographyState(HttpClient httpInstance, NavigationService navigate, NotificationMsgService notification)
        {
            _http = httpInstance;
            _navigate = navigate;
            _notification = notification;
        }
        #endregion Properties / Variables / DI

        #region Methods / WebApi Calls
        /// <summary>
        /// Calls WebApi to get Biography for single dog.
        /// </summary>        
        /// <param name="id">Dog Id <see cref="int" /></param>
        public async Task GetBiography(int id)
        {
            NewExpansionState();
            NewBiography();
            Biography = await _http.GetFromJsonAsync<DogBiography>($"api/Biography/{id}");

            if (Biography != null)
            {
                NotifyStateChanged();
            }
        }

        /// <summary>
        /// Calls WebApi to update Biography for single dog.
        /// </summary>        
        public async Task UpdateBiography()
        {
            HttpResponseMessage response = await _http.PutAsJsonAsync($"api/Biography/{Biography.DogId}", Biography);

            if (response.IsSuccessStatusCode)
            {
                _notification.DisplayMessage(NotificationType.BiographySaved, Biography.DogName);
            }
            else
            {
                _notification.DisplayMessage(NotificationType.BiographyError, Biography.DogName);
            }
        }

        /// <summary>
        /// Changes the current expanded biography text area panel
        /// </summary>
        /// <param name="nextExpansion">The next panel <see cref="BiographyExpansion"/> type to open</param>
        public async Task ChangeExpanded(BiographyExpansion nextExpansion)
        {
            switch (nextExpansion)
            {
                case BiographyExpansion.About:
                    UpdateAccordionState("AboutExpanded");
                    break;
                case BiographyExpansion.Memory:
                    UpdateAccordionState("MemoryExpanded");
                    break;
                case BiographyExpansion.Food:
                    UpdateAccordionState("FoodExpanded");                    
                    break;
                case BiographyExpansion.Toy:
                    UpdateAccordionState("ToyExpanded");
                    break;
                case BiographyExpansion.Sleep:
                    UpdateAccordionState("SleepExpanded");
                    break;
                case BiographyExpansion.Walk:
                    UpdateAccordionState("WalkExpanded");
                    break;
                case BiographyExpansion.AllClosed:
                    CleanExpandedState();
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
            await UpdateBiography();
        }
        #endregion Methods / WebApi Calls

        #region Internal Methods
        /// <summary>
        /// Invokes OnChange Action to notify subscribers state has changed.
        /// </summary>    
        private void NotifyStateChanged() => OnChange?.Invoke();

        /// <summary>
        /// Uses reflection to change the passed name's <see cref="bool"/> property to true and expand it's accordion panel
        /// </summary>
        /// <param name="name">name <see cref="string"/> of <see cref="bool"/> property to make true</param>
        private void UpdateAccordionState(string name)
        {
            // initialize new/clean expanded class (resets bools to false/closed)
            CleanExpandedState();

            // set property passed to method as true using reflection
            PropertyInfo property = Expanded.GetType().GetProperty(name);
            property.SetValue(Expanded, true, null);

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
