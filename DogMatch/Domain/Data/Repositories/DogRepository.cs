using DogMatch.Domain.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Extensions.Logging;
using System;
using DogMatch.Shared.Globals;
using DogMatch.Shared.Models;
using System.Text.RegularExpressions;

namespace DogMatch.Domain.Data.Repositories
{
    public class DogRepository : IDogRepository
    {
        #region DI
        private readonly DogMatchDbContext _context;
        private readonly DbSet<Dogs> _dbSet;
        private readonly ILogger<DogRepository> _logger;
        private readonly ITemperamentRepository _temperamentRepository;

        public DogRepository(DogMatchDbContext context, ILogger<DogRepository> logger, ITemperamentRepository temperamentRepository)
        {
            _context = context;
            _dbSet = context.Set<Dogs>();
            _logger = logger;
            _temperamentRepository = temperamentRepository;
        }
        #endregion DI

        #region Repository Methods
        /// <summary>
        /// Find single <see cref="Dogs"/> entity by dog Id <see cref="int"/>.
        /// </summary>        
        /// <param name="id">Dog Id integer</param>
        /// <returns>single <see cref="Dogs"/> entity</returns>
        public async Task<Dogs> FindDogById(int id) =>
            await _dbSet
            .AsNoTracking()
            .Include(d => d.Owner)
            .Include(d => d.DogProfileImage)
            .Include(d => d.Colors)
            .SingleOrDefaultAsync(d => d.Id == id && d.IsDeleted != true);

        /// <summary>
        /// Find single <see cref="Dogs"/> entity with navigation properties for Owner, Profile Image, Colors, Biography, Temperament and Album Images included
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns>Single <see cref="Dogs"/> entity for full profile</returns>
        public async Task<Dogs> FindFullDogProfileById(int id)
        {
            var dog = await _dbSet
                .AsNoTracking()
                .Include(d => d.Owner)
                .Include(d => d.DogProfileImage)
                .Include(d => d.Colors)
                .Include(d => d.Biography)
                .Include(d => d.Temperament)
                .SingleOrDefaultAsync(d => d.Id == id && d.IsDeleted != true);

            // for now (until efcore 5 / lambdas within include), get album images separately for active/non-deleted album images
            if (dog != null)
                dog.AlbumImages = _context.DogImages
                    .AsNoTracking()
                    .Where(i => i.DogId == id &&
                        !(i.IsProfileImage ?? false) &&
                        !(i.IsDeleted ?? false))
                    .ToList();

            return dog;
        }

        /// <summary>
        /// Find dogs, filter and search using <see cref="DogsFilter"/> object property values
        /// </summary>
        /// <param name="filter"><see cref="DogsFilter"/> object containing filter and search values</param>
        /// <param name="ownerId">request user <see cref="string"/>, for finding dogs owned by user</param>
        /// <returns><see cref="IEnumerable{Dogs}"/> found dogs list</returns>
        public async Task<IEnumerable<Dogs>> FindDogsAndFilter(DogsFilter filter, string ownerId)
        {
            IQueryable<Dogs> query = _dbSet
                        .Include(d => d.Owner)
                        .Include(d => d.DogProfileImage)                        
                        .Where(d => !(d.IsDeleted ?? false));

            // include Temperament, Biography, and Colors if show completed profiles requested
            if (filter.ShowCompletedProfiles)
                query = query
                    .Include(d => d.Temperament)
                    .Include(d => d.Biography)
                    .Include(d => d.Colors);

            // only owner's dogs if requested
            if (filter.DogListType == DogListType.Owners)
                query = query.Where(d => d.OwnerId == ownerId);

            // filter by gender if gender not set to all
            if (filter.Gender != DogGenderTypes.All)
                query = query.Where(d =>
                    d.Gender == (filter.Gender == DogGenderTypes.Female ? 'f' : 'm')
                );

            // filter using weight range if requested
            if (filter.FilterWeight)
                query = query.Where(d =>
                    d.Weight > filter.WeightRange.Start &&
                    d.Weight < filter.WeightRange.End
                );

            // filter using age range (in years) if requested, based on dog's age using today's date
            if (filter.FilterAge)
            {
                DateTime now = DateTime.Today;
                int today = (now.Year * 100 + now.Month) * 100 + now.Day;

                query = query.Where(d =>
                    ((today - ((d.Birthday.Value.Year * 100 + d.Birthday.Value.Month) * 100 + d.Birthday.Value.Day)) / 10000) >= filter.AgeRange.Start &&
                    ((today - ((d.Birthday.Value.Year * 100 + d.Birthday.Value.Month) * 100 + d.Birthday.Value.Day)) / 10000) <= filter.AgeRange.End
                );
            }
            
            // execute query
            IEnumerable<Dogs> results = await query.ToListAsync();

            // filter for only completed profiles if requested (basic detials, temperament ratings, and about dog completed)
            if (filter.ShowCompletedProfiles)
                results = FilterDogsForCompletedProfiles(results);

            // search remaining results if search string is not null or empty
            if (!string.IsNullOrWhiteSpace(filter.SearchString))
                results = Search(results, filter.SearchString);

            return results;
        }

        /// <summary>
        /// Finds single <see cref="Dogs"/> entity and includes all active, non-deleted Dog Album Images (<see cref="DogImages"/>)
        /// </summary>
        /// <param name="id">Dog Id <see cref="int"/></param>
        /// <returns>Single <see cref="Dogs"/> entity object instance with album images</returns>
        public async Task<Dogs> FindDogWithAlbumImagesById(int id)
        {
            Dogs dog = await _dbSet
                .AsNoTracking()
                // lambda within include not available in efcore 3.x, but will be in efcore 5.0 (saving for future upgrade)
                //.Include(d => d.AlbumImages.Where(a => !(a.IsDeleted ?? false)
                //    && !(a.IsProfileImage ?? false)))
                .SingleOrDefaultAsync(d => d.Id == id && d.IsDeleted != true);

            // for now (until efcore 5), get album images separately for active/non-deleted album images
            if (dog != null)
                dog.AlbumImages = _context.DogImages
                    .AsNoTracking()
                    .Where(i => i.DogId == id &&
                        !(i.IsProfileImage ?? false) &&
                        !(i.IsDeleted ?? false))
                    .ToList();

            return dog;
        }

        /// <summary>
        /// Writes new, single <see cref="Dogs"/> entity to database.
        /// </summary>        
        /// <param name="dog"><see cref="Dogs"/> entity object</param>
        /// <returns>The new <see cref="Dogs"/> entity with sql generated id <see cref="int"/>.</returns>
        public async Task<Dogs> SaveNewDog(Dogs dog)
        {
            _dbSet.Add(dog);
            await _context.SaveChangesAsync();
            _logger.LogTrace($"New Dog {dog.Id} created by {dog.CreatedBy}");

            return dog;
        }

        /// <summary>
        /// Save/Update single <see cref="Dogs"/> entity
        /// </summary>
        /// <param name="dog"><see cref="Dogs"/> entity object.</param>
        /// <returns><see cref="bool"/> to confirm changes saved.</returns>
        public async Task<bool> SaveDog(Dogs dog)
        {
            _context.Entry(dog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!DogExists(dog.Id))
                    _logger.LogError(ex, $"Failed saving updated Dog entity, Dog doesn't exist (attempted dog id {dog.Id}).");
                else
                    _logger.LogError(ex, $"Db Update Concurrency Exception while saving updated Dog entity for dog id {dog.Id}.");

                return false;
            }
        }
        #endregion Repository Methods

        #region Internal
        /// <summary>
        /// Checks if <see cref="Dogs"/> record exists
        /// </summary>
        /// <param name="id">Dog Id</param>
        /// <returns><see cref="bool"/>, true if <see cref="Dogs"/> record exists.</returns>
        private bool DogExists(int id) =>
            _dbSet.Any(d => d.Id == id && d.IsDeleted != false);

        /// <summary>
        /// Searches all dogs and returns any results containing search string
        /// </summary>
        /// <param name="dogs">dogs <see cref="IEnumerable{Dogs}"/> object to search</param>
        /// <param name="searchStr">search <see cref="string"/></param>
        /// <returns>Search Results <see cref="IEnumerable{Dogs}"/></returns>
        private IEnumerable<Dogs> Search(IEnumerable<Dogs> dogs, string searchStr)
        {
            // format search string, remove special characters and split terms into string array
            searchStr = Regex.Replace(searchStr, @"[^0-9a-zA-Z]+", " ");
            var searchStrArr = searchStr.Trim().ToUpper()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // search and return results
            return dogs.Where(d =>
                searchStrArr.Any(s => d.Name.ToUpper().Contains(s)) ||
                searchStrArr.Any(s => d.Breed.ToUpper().Contains(s)) ||
                searchStrArr.Any(s => d.Owner.UserName.ToUpper().Contains(s)))
                .ToList();
        }

        /// <summary>
        /// Finds dogs in a set where the profile is complete which includes completion of all basic details, all temperament ratings, and about dog bio
        /// </summary>
        /// <param name="dogs">Dog list <see cref="IEnumerable{Dogs}"/> to filter for completed profiles</param>
        /// <returns><see cref="IEnumerable{Dogs}"/> only dogs with completed profiles</returns>
        private IEnumerable<Dogs> FilterDogsForCompletedProfiles(IEnumerable<Dogs> dogs) =>
            dogs.Where(d =>
                // check basic details
                !string.IsNullOrWhiteSpace(d.Name) &&
                !string.IsNullOrWhiteSpace(d.Breed) &&
                d.Birthday.HasValue &&
                d.ProfileImageId.HasValue &&
                d.Weight.HasValue &&
                d.Gender.HasValue &&
                d.Colors.Count() > 0 &&

                // check about dog (no other bio properties checked)
                d.BiographyId.HasValue &&
                d.BiographyId.HasValue ?
                !string.IsNullOrWhiteSpace(d.Biography.AboutDoggo) : false &&

                // check if all temperament ratings completed
                d.TemperamentId.HasValue &&
                d.TemperamentId.HasValue ?
                _temperamentRepository.HasCompletedTemperament(d.Temperament) : false)
            .ToList();
        #endregion Internal
    }
}
