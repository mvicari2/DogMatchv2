# DogMatchv2
#### Created and Maintained by Matt Vicari
	
----------------
DogMatch v2 Blazor App (Blazor WebAssembly (WASM), Progressive Web App (PWA), .Net Core 5, SQL Server)

Dog Match is a social application for dog owners to find other dogs and dog owners that would be good matches for dog playdates, social dog walks, and more. 
The unique dog temperament and personality matching can find other dogs in your area for your dog to meet.

### Code (projects):
1. Client: Blazor WebAssembly Client SPA (WASM Progressive web Application)
2. Domain: Business Logic Layer (BLL Services), Data Repository Layer (uses Entity Framework Core 5.x)
3. Server: WebApi Controllers, Identity Server, etc.
4. Shared: Global enums, models, and DTOs

#### The original DogMatch React/Node.js/Express/MongoDB (MERN) application has been completely rewritten, redesigned, and enhanced using Blazor WASM, .Net Core 5, and SQL Server.

## Compiling Application
### Configure SQL server database (uses Entity Framework Core code first approach)
  1. Create new SQL Server database, named 'DogMatch' and update appsettings.json file in DogMatch.Server project with SQL credentials (and any changes to SQL configuration), 
  SQL credentials must have data read and write permissions (to write from EF Core).
  2. Using Visual Studio Package Manager console (pointed to the domain project, where Entity DB context exists) run 'Add-Migration' to create initial database migration script.
  3. Run ‘Update-Database' to generate database from code first EF Core context in domain data layer.
### Additional Steps:
  1.	For image uploads (dog profile and dog album images), create two file paths (directories), one for the dog profile images, and one for the dog album images. Images will be served from DogMatch.Server, update appsettings.config file with both file paths. 
### To Debug: 
  1. In Visual Studio, set DogMatch.Server as startup project and run IIS, and attach to your favorite browser. Create new owner accounts, new dogs, and start finding the best matches for your dog!

## Version/Master History
### v0.6.2 Random Dog Feature Update (11/28/2021):
  * Added new Random Dog feature, which generates a random dog (in a client modal from a nav menu button) that includes an image of the generated dog with the correct breed, age and name of the dog:  
	* New Random Dog Domain Service which calls open source public APIs that utilize the Stanford dogs dataset to get dog image and breed information and generate random name.
	* New random dog models and AutoMapper config mappings.
	* New random dog WebApi controller and endpoint.
    * Client app: new RandomDogState service to manage client state and call WebApi for random dog modal. New client modal to display random dog data and image, and request another dog. New Random button in nav bar to open random dog modal from anywhere in client app.
  * New DomainExtensions class with service and data layer extension methods.
### v0.6.1 Created DogMatch README file (3/11/2021):
  * Includes:
    * Information on setting up SQL database for DogMatchv2 project
    * Configuring and Debugging project
    * Complete Master branch release version history with detailed update notes and functionality explanations
### v0.6.0 Feature Update Dog Matching (3/7/2021):
  * Added new features for dog matching (similar to dating services matching), which returns other matching dogs to a primary dog based on a rankings derived from a dog's temperament ratings, basic details, and biography information provided by the dog's owner
  * Getting matches for a dog will return a list of up to 10 dog matches in order of the quality of match to the primary dog as determined in the new dog matching service. 
  * New features include:
    * New Dog Matching domain service:
      * Gets all possible matches (completed dog profiles only) for a primary dog
      * Filters them down to a list of the top 10 matches based on quality of match using match scores derived from temperament ratings, basic details, and other demographics provided by a dog owner
      * Scores are filtered by their place in a specific score range, the more range matches to the primary dog the higher quality of match
      * Additional information regarding the matching logic can be found in the comments/documentation of the domain matching service
    * New dog matching WebApi controller
    * New domain repository methods for getting dogs with completed profiles only (for possible matches)
    * New client app service to manage the dog matching state and actions related to the dog matches modal
    * New client side components for dog matches, which can be used in any other client component. 
      * Currently the dog matches modal is used in 2 locations, on the All Dogs list (as an owner action), and on the dog profile page (only can be accessed by dog owner)
      * Dog Matches modal displays (up to) the top 10 matches for a primary dog, with some basic demographics for the matched dogs and button navigation to the matches profile
    * Added temperament score types for easier handling of score category types
    * New client notification messages related to dog matching actions
    * Various minor enhancements, improvements, and bug fixes, including some css updates (moving inline style tags to css classes in app.css)
### v0.5.10 Removed Remaining EF Core Db Context Migrations (11/30/2020):
  * Removed remaining EF Core Db Context Migration and designer from github integration
### v0.5.9 .Net 5 Upgrade, CSS Overhaul, Package Upgrades, and More (11/30/2020):
  * Upgraded all DogMatch projects to .Net 5 (core) (client/server/domain/shared), including:
    * Blazor WebAssembly upgrade to 5.0
    * EF Core upgrade to 5.0
    * Automapper upgrade to v10.1.1
    * Identity Server to 5.0, 
    * Newtonsoft Json to 5.0
    * Plus all other major and minor package updates (except client file reader)
  * CSS overhaul (Blazor .Net 5 now allows component level css files): 
    * Cleaned up css, moved non-global styles to component level style sheets (including main layout and nav menu component css files, among others)
    * removed unused styles, and various css improvements and enhancements
  * EF Core upgrade, updated some methods to use latest ef core linq capabilities
  * Changed WASM client HttpClientFactory service from transient to scoped lifetime (due to transient memory leaks), and therefore updated all client state management services from singleton to scoped lifetimes (Blazor wasm treats these as singleton anyway)
  * Removed EF Core Db Context migration files from git integration, now only keeping DbContext model snapshot synced to github in migrations folder
  * Removed depreciated UseDatabaseErrorPage, replaced with AddDatabaseDeveloperPageExceptionFilter service
  * Various additional minor bug fixes, improvements, and enhancements
### v0.5.8 Dog Search and Filter Feature Expansion (11/24/2020):
  * Major expansion of the existing dog search by adding new dog filtering options: 
    * Added new Dog Search and Filter client component, now being using in both All Dogs list and Owner's Dog list
    * Filter includes weight range filtering, age range filtering, completed profiles filtering, and gender filtering (all with new UI controls)
    * All Dogs and Owner's Dogs requests now handled by same server and domain methods, using new DogsFilter object to determine which results to return (including for search, also uses new DogListType to determine active dogs list)
    * Created query builder for getting dog entities in DogsRepository in domain layer, which uses new DogsFilter object to determine query
    * UI/UX enhancements for All Dogs, Owner's Portal (Owner's Dogs), and search/filter client components.
### v0.5.7 Search Dogs Feature (11/11/2020):
  * Created initial search dogs functionality, allows searching of all dogs:
    * Created client search component which includes search text input, search btn, refresh btn
    * Updated WebApi to accept search string for GET all dogs
    * Updated Dog Service for search methods in domain layer
    * Updated Dog Repository in domain layer and added internal search method
### v0.5.6 Added Global Assembly Info (11/1/2020):
  * Added global assembly info file (GlobalAssemblyInfo.cs):
    * Added link to all projects
    * Configured global assembly versions for footers
    * Disabled assembly info auto generation in each individual project
### v0.5.5 Dog Profile Expansion (part 4) (11/1/2020):
  * Added new client components for dog album section in Dog Profile
    * Display's all album images for dog
    * Modal pop-up to display individual album image when selected
    * Forward/backward image navigation through album in individual image modal
  * Reconfigured dog profile sections into tab (tab buttons) based sections instead of horizontal on page
### v0.5.4 Dog Profile Expansion (part 3) (10/12/2020):
  * Added Dog Profile Biography client razor components that display the 6 biography string field values in a horizontal tab configuration
  * Added separate client components for handling incomplete temperament and biography dog profiles
  * Various minor enhancements
### v0.5.3 Dog Profile Expansion (part 2) (10/11/2020):
  * Added Temperament Graph client component to display temperament ratings scores in 13 categories
  * New and better error handling and client error messages for dog not found and general errors
  * Various minor improvements and enhancements
### v0.5.2 Dog Profile Expansion (part 1) (10/9/2020):
  * Further preparation for dog profile expansion:
    * Created new dog profile domain service that handles getting full dog profile logic (now includes Temperament scores for 13 categories (mostly averages of related properties, score out of possible 100), Biography data, album images)
    * Created new dog profile WebApi controller
    * Created new dog repository methods to find full dog profile entity
    * New shared models/DTOs
    * Related updates to existing client razor components and client service for dog profile state management
### v0.5.1 Logging Update (10/4/2020):
  * Added new log entries for various errors (and warnings and trace logs) in domain and server layers (in repositories, services, controllers)
  * Changed EF Core AddRangeAsync to AddRange to make methods thread safe (AddRangeAsync should only be used for SqlServerValueGenerationStrategy.SequenceHiLo)
### v0.5.0 Feature Update: Dog Image Album Feature, NLog, Session (and more) (9/29/2020):
  * Added dog album images functionality, where users can upload (and remove) up to 12 images to a single dog's album. This functionality includes:
    * For client project:
      * New Dog Album razor component page
      * New image uploader component (w/ file drop zone)
      * New album preview component (w/ image removal functionality)
      * Dog album client state management service
      * Album navigator component
      * Updated dog album client navigation
      * New album notification messages
    * For domain:
      * Expanded Image Service w/several new methods to handle multi-image uploads (saving to disk and creating/saving or removing dog image entities)
      * Expanded Dog Image domain repository (which includes adding/saving and soft deleting dog image ranges)
      * New dog repository method to get dog with all active album images
      * New automapper mapping config for album images
    * For Server:
      * Added new Dog Album WebApi controller to handle dog album requests
      * Added new static files config for album images
      * Added new shared models (DTOs) for Dog Album Images
  * Added NLog and intial logging config to write to file, configured Program.cs and Startup.cs to handle logging using NLog. Started adding new log entries to Dog Album and a few additional locations (further logging expansion to follow)
  * Configured application session state (services.AddSession), currently used to create session variables while logging (such as user Id session variable)
  * Various additional improvements:
    * Updated git ignore to ignore all vs code launch and config files (was testing project using vs code on my linux machine)
    * Removed old User Controller
    * Added new styles, etc.
### v0.4.7 Dog Images Repository Update (9/13/2020):
  * Created new Dog Images domain repository and interface
  * Removed dbcontext injection from Image Service constructor
  * Fixed issue with the create initial dog page/form, where pressing enter key wouldn't submit form/dog name
  * Added loading animation while waiting for dog update/save on doggo details page (when staying on page)
### v0.4.6 Profile Security Update (9/13/2020):
  * Updated/added Dog Profile forms security:  
    * Now blocks access to view/edit/submit/save profile forms for non-owner of the dog (for dog profile, biography, temperament, basic details, delete dog)
    * Added loading animations for profile forms prior to data get and owner authorization
    * New routing for non-authorized users and new access denied notification message. 
  * Update to Biography expansion panel state handling, fixes issue where couldn't close panel without opening another
  * Added new domain User Service and User Repository
  * Various minor bug fixes and enhancements
### v0.4.5 Nav Menu Update (9/7/2020):
  * Updated Nav Menus including main nav and user profile dropdown btn:
    * Added outfocus event handling to close menus on out focus
    * Added new icons
    * Updated formatting, colors, styles/css classes (consistent view width breakpoints)
    * Permission based nav menu items (removes dog related menu links when user not authorized)
    * Additional other minor fixes and improvements
  * Fixed spelling error in Temperament property and updated Db Context, entity model, shared models, and all uses of incorrect property (was spelled incorrectly everywhere). Generated new initial Db migrations due to Db Context updates.
### v0.4.4 Dog Profile Card Update (9/5/2020):
  * Split Dog Profile card into separate components, updated styles for profile card
  * Made the dog list paginator controls a static/fixed element above the footer
  * Various minor updates and bug fixes
### v0.4.3 Navigator Update (9/1/2020):
  * Added new row of navigation chips to dog related pages/components (Temperament, Dog Details, Dog Profile) and updated chip styles
  * Added new Dog Profile State service to manage client state for dog profile (in preparation for future dog profile expansion)
  * Added delete button, delete confirmation modal, and client state method for delete dog profile
### v0.4.2 Dog Colors Update (8/30/2020):
  * Added new Dog Colors client component with searchable multi-select list
  * Added new dog color repository
  * New dog service method to handle adding/removing dog colors
  * Updated db context for Color due to error (previously had DogId as key instead of own PK)
  * Regenerated initial db migrations after db context update
### v0.4.1 Doggo Details & Client Packages Update (8/27/2020):
  * Overhauled the Doggos Details client page component by splitting out input fields into separate components
  * Updated Basic Details client components to better looking MatBlazor components for breed/date/weight/gender inputs
  * Overhauled profile image uploader and changed input to custom drop zone area (and split to separate component)
  * Updated all client nuget packages (including wasm packages) to most recent
  * Updated DogList component (fonts, max-width for dog cards, button themes/styles)
  * Updated Create Initial Doggo page with new MatTextField and css class (removed style string from dog state)
  * Moved footer style strings to actual css classes in app.css
### v0.4.0 Feature Update (Dog Biography) (8/19/2020):
  * Added Biography Profile features including:
    * New Biography WebApi (server),
    * New Service (domain), 
    * New Repository (domain)
    * New Blazor Page and Components for Biography component tree (client)
      * Expandable panels to house text areas, chip button navigation component
    * New state management service (client)
    * Various minor improvements and bug fixes.
### v0.3.1 DogList Update (8/10/2020):
  * Added new DogList component to reuse dog profile list
  * Updated profile list card layout
  * Added handling for returning 0 dogs from WebApi for dog lists
### v0.3.0 Feature Update (Domain Upgrade) (8/9/2020):
  * Migrated services, repositories, DbContext/efcore from server to separate Domain project
  * Updated data models/database (include new separate table for user images (user profile images, non-dog images))
  * Generated new initial db migrations
  * Updated server packages to 3.1.6
  * Added new configuration string for file path directories (image path)
### v0.2.3 Owners Update (8/6/20):
  * Updated Owner's Portal page
  * Created new WebApi controller for Owner requests
  * New methods in Dog service and Dog repository to return dogs by owner/user
  * Updated "var"s to their types for better readability, and other minor syntax updates
### v0.2.2 Owner Button / Delete Dog Update (8/3/2020):
  * Added new Owner Portal button for navigation to dog updates and deletion for each doggo where user is dog owner
  * Added new server and client side delete functionality to soft delete dogs
  * Added new Owner Portal page
  * Various minor improvements and enhancements
### v0.2.1 Dog Repository Update (7/26/2020):
  * Added new Dog Repository layer (moved ef core data methods from Dog Service layer to repository layer)
  * Added new delete dog (soft delete) method to Dog Service
  * Minor updates to Dog WebApi controller
### v0.2.0 Feature Update (7/20/2020):
  * Addition of Temperament features including Temperament Pages and nested Components
    * Temperament state service for managing client temperament state (components subscribed to change action)
    * Templated field component
    * Temperament WebApi 
    * Temperament service layer
    * Temperament repository layer
    * New model mappings.
  * New injectable client notification message service
  * New injectable client navigation service
  * New Global type enums
  * Added regions and method summaries to most c# methods
  * Various minor code updates and improvements
### v0.1.2 Nav Update  (6/28/2020):
  * Navbar update (new horizontal nav for client and server (identity pages))
  * Identity nav btn dropdown
  * Conditional navigation
  * Static footer  
  * Added app landing page
### v.0.1.1 (6/19/2020):
  * Scaffolded custom Identity Server pages for login/register/profile etc.
### v.0.1.0 Feature Update (6/18/2020):
  * Added new WebApi's for Dog and User 
  * New Server services for Dog and Images
  * New Shared models, DTOs
  * New Pages (Create Dog, Update Dog, All Doggo display)
  * New Automapper config
  * New Client service to manage client side Dog State 
  * Updated Db Context and entity models to handle static images instead of binary files stored in database
  * Created new initial db migration
  * New Client libraries (Radzen, Spinkit, MatBlazor)
  * Various minor improvements and enhancements
### v0.0.1 (5/24/20):
  * Added DogMatch DbContext and models for ef core, created initial database migration to create db from code.


