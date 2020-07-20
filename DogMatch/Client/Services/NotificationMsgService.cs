﻿using Radzen;
using DogMatch.Shared.Globals;

namespace DogMatch.Client.Services
{
    /// <summary>
    /// Injectable Client Side Notification Message Service.
    /// </summary>   
    public class NotificationMsgService
    {
        private readonly NotificationService _service;
        public NotificationMsgService(NotificationService service) => _service = service;

        #region Public Methods
        /// <summary>
        /// Displays <see cref="NotificationMessage"/>.
        /// </summary>        
        /// <param name="type"><see cref="NotificationType"/> type</param>
        /// <param name="dogName">dog name string</param>
        public void DisplayMessage(NotificationType type, string dogName)
        {
            switch (type)
            {
                case NotificationType.TemperamentSaved:
                    TemperamentSaved(dogName);
                    break;
                case NotificationType.TemperamentError:
                    TemperamentError(dogName);
                    break;
                case NotificationType.DogCreated:
                    DogCreated(dogName);
                    break;
                case NotificationType.DogUpdated:
                    DogUpdated(dogName);
                    break;
                case NotificationType.DogUpdateError:
                    DogUpdateError(dogName);
                    break;
            }          
        }
        #endregion Public Methods

        #region Notify Methods
        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for Temperament Error,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="dogName">dog name string</param>
        private void TemperamentError(string dogName) =>
            _service.Notify(
                GetErrorMessage(
                    "Error Saving Temperament!",
                    $"Temperament Profile for {dogName} Failed to Save.",
                    6000
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for Temperament Saved,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="dogName">dog name string</param>
        private void TemperamentSaved(string dogName) =>
            _service.Notify(
                GetSuccessMessage(
                    "Temperament Profile Saved!",
                    $"Temperament Profile for {dogName} Saved Successfully.",
                    3000
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for Dog Created,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="dogName">dog name string</param>
        private void DogCreated(string dogName) =>
            _service.Notify(
                GetSuccessMessage(
                    "Doggo Created!",
                    $"New Profile for {dogName} Created.",
                    4000
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for Dog Updated,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="dogName">dog name string</param>
        private void DogUpdated(string dogName) =>
            _service.Notify(
                GetSuccessMessage(
                    "Doggo Updated!",
                    $"Profile for {dogName} Updated.",
                    5000
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for Dog Update Error,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="dogName">dog name string</param>
        private void DogUpdateError(string dogName) =>
            _service.Notify(
                GetErrorMessage(
                    "Update Failed!",
                    $"Profile for {dogName} Fail to Update.",
                    5000
                ));

        #endregion Notify Methods

        #region Toaster Notification Messages
        /// <summary>
        /// Initialize new Error <see cref="NotificationMessage"/> object instance.
        /// </summary>        
        /// <param name="title">message title</param>
        /// <param name="body">message body</param>
        /// <param name="duration">message duration in ms</param>
        /// <returns>new Error <see cref="NotificationMessage"/></returns>
        private NotificationMessage GetErrorMessage(string title, string body, int duration)
        {            
            return new NotificationMessage()
            {
                Severity = NotificationSeverity.Error,
                Summary = title,
                Detail = body,
                Duration = duration
            };
        }

        /// <summary>
        /// Initialize new Success <see cref="NotificationMessage"/> object instance.
        /// </summary>        
        /// <param name="title">message title</param>
        /// <param name="body">message body</param>
        /// <param name="duration">message duration in ms</param>
        /// <returns>new Success <see cref="NotificationMessage"/></returns>
        private NotificationMessage GetSuccessMessage(string title, string body, int duration)
        {
            return new NotificationMessage()
            {
                Severity = NotificationSeverity.Success,
                Summary = title,
                Detail = body,
                Duration = duration
            };
        }        
        #endregion Toaster Notification Messages
    }
}
