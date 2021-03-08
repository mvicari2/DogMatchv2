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
        BiographyError = 10,
        NotAuthorizedOwnerEditError = 11,
        DogAlbumSaved = 12,
        DogAlbumError = 13,
        DogAlbumExcessImages = 14,
        DogNotFound = 15,
        GeneralError = 16,
        MatchesUnauthorized = 17
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
        ToDogAlbum = 5,
        ToOwnersPortal = 6,
        ToAllDoggos = 7
    }

    public enum DogGenderTypes
    {
        All = 0,
        Female = 1,
        Male = 2
    }

    public enum DogListType
    {
        AllDogs = 1,
        Owners = 2
    }

    public enum TemperamentScoreTypes
    {
        Playfullness = 1,
        Friendliness = 2,
        Athletic = 3,
        Training = 4,
        Empathy = 5,
        Intelligence = 6,
        Aggression = 7,
        Anxiety = 8,
        Instinct = 9,
        Confidence = 10,
        Stubbornness = 11,
        Shedding = 12,
        Smelliness = 13
    }
}
