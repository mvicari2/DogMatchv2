﻿using Microsoft.AspNetCore.Components;

namespace DogMatch.Client.Services
{
    /// <summary>
    /// Injectable Client Side Navigation and Routing.
    /// </summary>   
    public class NavigationService
    {
        // DI
        private readonly NavigationManager _service;
        public NavigationService(NavigationManager service) => _service = service;

        #region Navigation Methods
        /// <summary>
        /// Navigates to dog profile page.
        /// </summary>        
        /// <param name="id">dog id int</param>
        public void ToProfile(int id) =>
            _service.NavigateTo($"/DoggoProfile/{id}");

        /// <summary>
        /// Navigates to Doggo Details update page.
        /// </summary>        
        /// <param name="id">dog id int</param>
        public void ToUpdateDoggo(int id) =>
            _service.NavigateTo($"/DoggoDetails/{id}");

        /// <summary>
        /// Navigates to Temperament Profile page.
        /// </summary>        
        /// <param name="id">dog id int</param>
        public void ToTemperament(int id) =>
            _service.NavigateTo($"/Temperament/{id}");

        /// <summary>
        /// Navigates to All Doggos page.
        /// </summary>        
        public void ToAllDoggos() =>
            _service.NavigateTo("/AllDoggos/");

        /// <summary>
        /// Navigates to Login page.
        /// </summary>        
        public void ToLogin() =>
            _service.NavigateTo("/authentication/login");

        /// <summary>
        /// Navigates to User Registration page.
        /// </summary>        
        public void ToRegister() =>
            _service.NavigateTo("/authentication/register");

        #endregion Navigation
    }
}