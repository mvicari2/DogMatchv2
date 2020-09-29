using DogMatch.Shared.Globals;
using DogMatch.Shared.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DogMatch.Client.Services
{
    /// <summary>
    /// Manages state for Dog Album Images components and pages
    /// </summary>
    public class DogAlbumState
    {
        #region Properties / Variables
        public DogAlbumImages Album { get; set; }      
        public bool loading = false;
        public bool uploadInProgress = false;
        public event Action OnChange;
        #endregion Properties / Variables

        #region DI
        private readonly HttpClient _http;
        private readonly NavigationService _navigate;
        private readonly NotificationMsgService _notification;

        public DogAlbumState(HttpClient httpInstance, NavigationService navigate, NotificationMsgService notification)
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
        /// Calls WebApi to get Dog Album for single dog.
        /// </summary>        
        /// <param name="id">Dog Id <see cref="int" /></param>
        public async Task GetDogAlbumImages(int id)
        {
            // show animation and initialize state properties
            loading = true;
            NewDogAlbum();

            // get dog album
            HttpResponseMessage response = await _http.GetAsync($"api/DogAlbum/{id}");

            if (response.IsSuccessStatusCode)
            {
                // set dog album into state
                Album = await response.Content.ReadFromJsonAsync<DogAlbumImages>();
                loading = false;

                // notify subscribers state has changed
                if (Album != null)
                    NotifyStateChanged();
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // user is not authorized to edit this dog album (likely not owner)
                _notification.DisplayMessage(NotificationType.NotAuthorizedOwnerEditError);
                _navigate.ToAllDoggos();
                return;
            }
        }

        /// <summary>
        /// Calls WebApi to update/save Dog Album for single dog.
        /// </summary>        
        public async Task<bool> UpdateAlbumImages(bool getAlbumOnSuccess = false)
        {
            // call WebApi, put dog album
            HttpResponseMessage response = await _http.PutAsJsonAsync($"api/DogAlbum/{Album.DogId}", Album);

            if (response.IsSuccessStatusCode)
            {
                // show success notification
                _notification.DisplayMessage(NotificationType.DogAlbumSaved, Album.DogName);       

                if (getAlbumOnSuccess) // refresh Album state object
                    await GetDogAlbumImages(Album.DogId);

                // stop/hide animation
                loading = uploadInProgress = false;

                return true;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // user is not authorized to update dog album (likely not owner)
                _notification.DisplayMessage(NotificationType.NotAuthorizedOwnerEditError);
                _navigate.ToAllDoggos();
                return false;
            }
            else
            {
                // display save error notification
                _notification.DisplayMessage(NotificationType.DogAlbumError, Album.DogName);
                return false;
            }
        }

        /// <summary>
        /// Removes single image from album images list or flags already saved image for deletion
        /// </summary>
        /// <param name="index">Album image index <see cref="int"/> to remove from List</param>
        public void RemoveImageFromAlbum(int index)
        {
            // if image has not been saved to disk yet, remove completely from image list
            if (Album.Images[index].Id == 0)
            {
                Album.Images.RemoveAt(index);
            }
            else
            {
                // if image is already saved to disk then flag it for deletion
                Album.Images[index].Delete = true;
            }            
            NotifyStateChanged();
        }

        /// <summary>
        /// Removes all current unsaved images from Album Image List and flags images already saved for deletion
        /// </summary>
        public void RemoveAllImagesFromAlbum()
        {
            // first remove any images not yet saved to disk or database
            Album.Images = Album.Images
                .Where(i => i.Id != 0)
                .ToList();

            // then flag for deletion any images already saved, remove image string
            foreach (var image in Album.Images)
            {
                image.Delete = true;
                image.ImageString = null;
            }

            // alert subscribers that state has changed
            NotifyStateChanged();
        }

        /// <summary>
        /// Calls notification for uploaded images exceed count limit
        /// </summary>
        public void AlertOverImageLimit() =>
            _notification.DisplayMessage(NotificationType.DogAlbumExcessImages);

        /// <summary>
        /// Saves Dog Album and navigates to new page based on <see cref="Navigate"/> type passed by user
        /// </summary>
        /// <param name="destination">the destniation page <see cref="Navigate"/> type</param>
        public async Task SaveAndNavigate(Navigate destination)
        {
            // show loading animation during upload, alert subscribers
            loading = uploadInProgress = true;
            NotifyStateChanged();

            // delay update by 100ms until loading animation starts (else animation won't fire)
            await Task.Delay(100);

            // call update
            bool success = await UpdateAlbumImages();

            // navigate if dog album is updated successfully
            if (success)
            {
                switch (destination)
                {
                    case Navigate.ToProfile:
                        _navigate.ToProfile(Album.DogId);
                        break;
                    case Navigate.ToDetails:
                        _navigate.ToUpdateDoggo(Album.DogId);
                        break;
                    case Navigate.ToBiography:
                        _navigate.ToBiography(Album.DogId);
                        break;
                    case Navigate.ToTemperament:
                        _navigate.ToTemperament(Album.DogId);
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

        #region Initialize Class
        /// <summary>
        /// Initializes new <see cref="DogAlbumImages"/> instance in state.
        /// </summary>        
        public void NewDogAlbum() => Album = new DogAlbumImages();
        #endregion Initialize Class
    }
}
