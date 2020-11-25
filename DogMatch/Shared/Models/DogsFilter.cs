using DogMatch.Shared.Globals;

namespace DogMatch.Shared.Models
{
    public class DogsFilter
    {
        public string SearchString { get; set; }
        public DogGenderTypes Gender { get; set; }
        public bool FilterWeight { get; set; }
        public WeightRange WeightRange { get; set; }
        public bool FilterAge { get; set; }
        public AgeRange AgeRange { get; set; }
        public bool ShowCompletedProfiles { get; set; }
        public DogListType DogListType { get; set; }
    }

    public class AgeRange
    {
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class WeightRange
    {
        public int Start { get; set; }
        public int End { get; set; }
    }
}
