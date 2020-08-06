using AutoMapper;
using DogMatch.Shared.Models;
using DogMatch.Server.Data.Models;
using System;

namespace DogMatch.Server.Infrastructure
{
    public class AutoMapperProfileServerConfiguration : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfileServerConfiguration"/> class. 
        /// </summary>
        public AutoMapperProfileServerConfiguration() : this("DogMatch") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfileServerConfiguration"/> class for the given profile name. 
        /// </summary>
        /// <param name="profileName">The profile name.</param>
        protected AutoMapperProfileServerConfiguration(string profileName) : base(profileName)
        {
            CreateMap<Dog, Dogs>()
                .ForMember(dest => dest.Id, cfg => cfg.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, cfg => cfg.MapFrom(src => src.Name))
                .ForMember(dest => dest.Breed, cfg => cfg.MapFrom(src => src.Breed))
                .ForMember(dest => dest.Birthday, cfg => cfg.MapFrom(src => src.Birthday))
                .ForMember(dest => dest.Gender, cfg => cfg.MapFrom(src => src.Gender == "female" ? 'f' : src.Gender == "male" ? 'm' : ' '))
                .ForMember(dest => dest.Weight, cfg => cfg.MapFrom(src => src.Weight))
                .ForMember(x => x.Owner, src => src.Ignore()) // ignore temporarily
                .ForMember(x => x.ProfileImage, src => src.Ignore());

            CreateMap<Dogs, Dog>()
                .ForMember(dest => dest.Id, cfg => cfg.MapFrom(src => src.Id))
                .ForMember(d => d.Name, cfg => cfg.MapFrom(src => src.Name))
                .ForMember(dest => dest.Breed, opt => {
                    opt.PreCondition(src => (src.Breed != null));
                    opt.MapFrom(src => src.Breed);
                })
                .ForMember(dest => dest.Birthday, opt => {
                    opt.PreCondition(src => (src.Birthday != null));
                    opt.MapFrom(src => src.Birthday);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.PreCondition(src => (src.Birthday != null));
                    opt.MapFrom(src => GetAge(src.Birthday));
                })
                .ForMember(d => d.Gender, cfg => cfg.MapFrom(src => src.Gender == 'f' ? "female" : src.Gender == 'm' ? "male" : "unknown"))
                .ForMember(d => d.Weight, cfg => cfg.MapFrom(src => src.Weight))
                .ForMember(d => d.ProfileImage, cfg => cfg.MapFrom(src => src.ProfileImage != null ? "/ProfileImage/" + src.ProfileImage.Filename : "dogmatch_paw.png"))
                .ForMember(d => d.Owner, cfg => cfg.MapFrom(src => src.Owner.UserName)); // using username for owner until collecting first/last name of user

            CreateMap<Temperament, DogTemperament>()
                .ForMember(dest => dest.Id, cfg => cfg.MapFrom(src => src.Id))
                .ForMember(d => d.DogId, cfg => cfg.MapFrom(src => src.DogId))
                .ForMember(d => d.DogName, cfg => cfg.MapFrom(src => src.Dog.Name))
                .ForMember(d => d.OwnerId, cfg => cfg.MapFrom(src => src.Dog.Owner.Id))
                .ForMember(d => d.OwnerName, cfg => cfg.MapFrom(src => src.Dog.Owner.UserName));

            CreateMap<DogTemperament, Temperament>()
               .ForMember(dest => dest.Id, cfg => cfg.MapFrom(src => src.Id))
               .ForMember(d => d.DogId, cfg => cfg.MapFrom(src => src.DogId));
        }       

        // uses birthday datetime to return string w/current age of dog
        public string GetAge(DateTime? bday)
        {
            DateTime today = DateTime.Today;
            int a = (today.Year * 100 + today.Month) * 100 + today.Day;
            int b = (bday.Value.Year * 100 + bday.Value.Month) * 100 + bday.Value.Day;

            int age = (a - b) / 10000;

            string ageStr;
            // if less than 1 year old then determine age in months
            if (age < 1)
            {
                int m1 = (today.Month - bday.Value.Month);
                int m2 = (today.Year - bday.Value.Year) * 12;
                int months = m1 + m2;

                ageStr = months + " months old";
            }
            else if (age == 1)
            {
                ageStr = age + " year old";
            }
            else
            {
                ageStr = age + " years old";
            }

            return ageStr;
        }
    }
}
