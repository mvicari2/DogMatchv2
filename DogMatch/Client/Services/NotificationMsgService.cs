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
        /// <param name="argStr"><see cref="string"/> argument to pass to notificaton methods</param>
        public void DisplayMessage(NotificationType type, string argStr = null)
        {
            switch (type)
            {
                case NotificationType.TemperamentSaved:
                    TemperamentSaved(argStr);
                    break;
                case NotificationType.TemperamentError:
                    TemperamentError(argStr);
                    break;
                case NotificationType.DogCreated:
                    DogCreated(argStr);
                    break;
                case NotificationType.DogUpdated:
                    DogUpdated(argStr);
                    break;
                case NotificationType.DogUpdateError:
                    DogUpdateError(argStr);
                    break;
                case NotificationType.DogDeleted:
                    DogDeleted(argStr);
                    break;
                case NotificationType.DogDeleteUnauthorized:
                    DogDeleteUnauthorized(argStr);
                    break;
                case NotificationType.DogDeleteError:
                    DogDeleteError(argStr);
                    break;
                case NotificationType.BiographySaved:
                    BiographySaved(argStr);
                    break;
                case NotificationType.BiographyError:
                    BiographyError(argStr);
                    break;
                case NotificationType.NotAuthorizedOwnerEditError:
                    NotAuthorizedOwnerEditError();
                    break;
                case NotificationType.DogAlbumSaved:
                    DogAlbumSaved(argStr);
                    break;
                case NotificationType.DogAlbumError:
                    DogAlbumError(argStr);
                    break;
                case NotificationType.DogAlbumExcessImages:
                    DogAlbumExcessImages();
                    break;
                case NotificationType.DogNotFound:
                    DogNotFound(argStr);
                    break;
                case NotificationType.GeneralError:
                    GeneralError(argStr);
                    break;
                case NotificationType.MatchesUnauthorized:
                    MatchesUnauthorized();
                    break;
            }
        }
        #endregion Public Methods

        #region Notify Methods (Internal)
        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for Temperament Error,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="dogName">dog name <see cref="string"/></param>
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
        /// <param name="dogName">dog name <see cref="string"/></param>
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
        /// <param name="dogName">dog name <see cref="string"/></param>
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
        /// <param name="dogName">dog name <see cref="string"/></param>
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
        /// <param name="dogName">dog name <see cref="string"/></param>
        private void DogUpdateError(string dogName) =>
            _service.Notify(
                GetErrorMessage(
                    "Update Failed!",
                    $"Profile for {dogName} Fail to Update.",
                    5000
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for Dog Deleted successfully,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="dogName">dog name <see cref="string"/></param>
        private void DogDeleted(string dogName) =>
            _service.Notify(
                GetSuccessMessage(
                    "Dog Deleted!",
                    $"Profile for {dogName} was deleted.",
                    5000
                ));

        /// <summary>
        /// Creates new warning <see cref="NotificationMessage"/> 
        /// for unauthorized user (non-owner) delete attempt,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="dogName">dog name <see cref="string"/></param>
        private void DogDeleteUnauthorized(string dogName) =>
            _service.Notify(
                GetErrorMessage(
                    "Unauthorized",
                    $"You must be the dog's owner to delete profile for {dogName}.",
                    7500
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for generic Dog Delete Error,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="dogName">dog name <see cref="string"/></param>
        private void DogDeleteError(string dogName) =>
            _service.Notify(
                GetErrorMessage(
                    "Delete Failed!",
                    $"Delete failed for {dogName}, please try again.",
                    6000
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for Dog Biography Saved successfully,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="dogName">dog name <see cref="string"/></param>
        private void BiographySaved(string dogName) =>
            _service.Notify(
                GetSuccessMessage(
                    "Biography Saved!",
                    $"Biography for {dogName} was saved successfully.",
                    2500
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for Dog Biography saved error,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="dogName">dog name <see cref="string"/></param>
        private void BiographyError(string dogName) =>
            _service.Notify(
                GetErrorMessage(
                    "Save Failed!",
                    $"Saving Biography failed for {dogName}, please try again.",
                    2500
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for Dog Biography saved error,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        private void NotAuthorizedOwnerEditError() =>
            _service.Notify(
                GetErrorMessage(
                    "Access Denied",
                    $"You do not have permissions to edit this dog, or you are not the dog's owner.",
                    10000
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for Dog Album Saved successfully,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="dogName">dog name <see cref="string"/></param> 
        private void DogAlbumSaved(string dogName) =>
            _service.Notify(
                GetSuccessMessage(
                    "Dog Album Saved!",
                    $"Dog Album for {dogName} was saved successfully.",
                    10000
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for Dog Album saved error,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="dogName">dog name <see cref="string"/></param>     
        private void DogAlbumError(string dogName) =>
            _service.Notify(
                GetErrorMessage(
                    "Save Failed!",
                    $"Saving Dog Album for {dogName} failed, please try again.",
                    10000
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for user attempting to upload more dog album images than allowed,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>           
        private void DogAlbumExcessImages() =>
            _service.Notify(
                GetWarningMessage(
                    "Uploaded more than 12 Images",
                    $"Some Images were not saved, please remove images before adding additonal.",
                    10000
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for Dog Not Found,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="action">page or action <see cref="string"/> called when dog not found</param>
        private void DogNotFound(string action) =>
            _service.Notify(
                GetErrorMessage(
                    "Dog Not Found",
                    $"Dog does not exist or could not be found while getting {action}",
                    8500
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for a general error with passed message,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>        
        /// <param name="message">action or message <see cref="string"/> to add to error message</param>
        private void GeneralError(string message = null) =>
            _service.Notify(
                GetErrorMessage(
                    "Error!",
                    $"{message}",
                    7000
                ));

        /// <summary>
        /// Creates new <see cref="NotificationMessage"/> for unauthorized to get matches for dog,
        /// and calls <see cref="NotificationService"/> Notify.
        /// </summary>
        private void MatchesUnauthorized() =>
            _service.Notify(
                GetErrorMessage(
                    "Error!",
                    $"You are not authorized to get dog matches for this dog.",
                    8500
                ));
        #endregion Notify Methods (Internal)

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

        /// <summary>
        /// Initialize new Info <see cref="NotificationMessage"/> object instance.
        /// </summary>        
        /// <param name="title">message title</param>
        /// <param name="body">message body</param>
        /// <param name="duration">message duration in ms</param>
        /// <returns>new Info <see cref="NotificationMessage"/></returns>
        private NotificationMessage GetInfoMessage(string title, string body, int duration)
        {
            return new NotificationMessage()
            {
                Severity = NotificationSeverity.Info,
                Summary = title,
                Detail = body,
                Duration = duration
            };
        }

        /// <summary>
        /// Initialize new Warning <see cref="NotificationMessage"/> object instance.
        /// </summary>        
        /// <param name="title">message title</param>
        /// <param name="body">message body</param>
        /// <param name="duration">message duration in ms</param>
        /// <returns>new Warning <see cref="NotificationMessage"/></returns>
        private NotificationMessage GetWarningMessage(string title, string body, int duration)
        {
            return new NotificationMessage()
            {
                Severity = NotificationSeverity.Warning,
                Summary = title,
                Detail = body,
                Duration = duration
            };
        }
        #endregion Toaster Notification Messages
    }
}
