namespace DogMatch.Shared.Globals
{
    public enum NotificationType
    {
        DogCreated = 1,
        DogUpdated = 2,
        DogUpdateError = 3,
        TemperamentSaved = 4,
        TemperamentError = 5,
        DogDeleted = 6,
        DogDeleteUnauthorized = 7,
        DogDeleteError = 8,
        BiographySaved = 9,
        BiographyError = 10
    }

    public enum TemperamentDirection
    {
        Forward = 1,
        Back = 2,
        Profile = 3,
        Biography = 4
    }

    public enum DeleteDogResponse
    {
        Success = 1,
        Failed = 2,
        Unauthorized = 3
    }

    public enum BiographyExpansion
    {
        About = 1,
        Memory = 2,
        Food = 3,
        Toy = 4,
        Sleep = 5,
        Walk = 6,
        AllClosed = 7
    }

    public enum Navigate
    {
        ToProfile = 1,
        ToDetails = 2,
        ToTemperament = 3,
        ToBiography = 4,
        ToOwnersPortal = 5,
        ToAllDoggos = 6
    }

    public enum DogGenderTypes
    {
        female = 1,
        male = 2
    }
}
