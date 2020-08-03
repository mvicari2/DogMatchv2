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
        DogDeleteError = 8
    }

    public enum TemperamentDirection
    {
        Forward = 1,
        Back = 2,
        Profile = 3
    }

    public enum DeleteDogResponse
    {
        Success = 1,
        Failed = 2,
        Unauthorized = 3
    }
}
