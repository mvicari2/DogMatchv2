using AutoMapper;
using DogMatch.Shared.Models;
using DogMatch.Domain.Data.Models;
using System;

namespace DogMatch.Domain.Infrastructure
{
    public class AutoMapperProfileDomainConfiguration : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfileServerConfiguration"/> class. 
        /// </summary>
        public AutoMapperProfileDomainConfiguration() : this("DogMatch") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfileServerConfiguration"/> class for the given profile name. 
        /// </summary>
        /// <param name="profileName">The profile name.</param>
        protected AutoMapperProfileDomainConfiguration(string profileName) : base(profileName)
        {
            CreateMap<Dog, Dogs>()
                .ForMember(d => d.Id, cfg => cfg.MapFrom(src => src.Id))
                .ForMember(d => d.Name, cfg => cfg.MapFrom(src => src.Name))
                .ForMember(d => d.Breed, cfg => cfg.MapFrom(src => src.Breed))
                .ForMember(d => d.Birthday, cfg => cfg.MapFrom(src => src.Birthday))
                .ForMember(d => d.Gender, cfg => cfg.MapFrom(src => src.Gender == "female" ? 'f' : src.Gender == "male" ? 'm' : ' '))
                .ForMember(d => d.Weight, cfg => cfg.MapFrom(src => src.Weight))
                .ForMember(d => d.Owner, src => src.Ignore()) // ignore temporarily
                .ForMember(d => d.DogProfileImage, src => src.Ignore());

            CreateMap<Dogs, Dog>()
                .ForMember(d => d.Id, cfg => cfg.MapFrom(src => src.Id))
                .ForMember(d => d.Name, cfg => cfg.MapFrom(src => src.Name))
                .ForMember(d => d.Breed, opt => {
                    opt.PreCondition(src => (src.Breed != null));
                    opt.MapFrom(src => src.Breed);
                })
                .ForMember(d => d.Birthday, opt => {
                    opt.PreCondition(src => (src.Birthday != null));
                    opt.MapFrom(src => src.Birthday);
                })
                .ForMember(d => d.Age, opt => {
                    opt.PreCondition(src => (src.Birthday != null));
                    opt.MapFrom(src => GetAge(src.Birthday));
                })
                .ForMember(d => d.Gender, cfg => cfg.MapFrom(src => src.Gender == 'f' ? "female" : src.Gender == 'm' ? "male" : "unknown"))
                .ForMember(d => d.Weight, cfg => cfg.MapFrom(src => src.Weight))
                .ForMember(d => d.ProfileImage, cfg => cfg.MapFrom(src => src.DogProfileImage != null ? "/ProfileImage/" + src.DogProfileImage.Filename : "dogmatch_paw.png"))
                .ForMember(d => d.Owner, cfg => cfg.MapFrom(src => src.Owner.UserName)); // using username for owner until collecting first/last name of user

            CreateMap<Temperament, DogTemperament>()
                .ForMember(d => d.Id, cfg => cfg.MapFrom(src => src.Id))
                .ForMember(d => d.DogId, cfg => cfg.MapFrom(src => src.DogId))
                .ForMember(d => d.DogName, cfg => cfg.MapFrom(src => src.Dog.Name))
                .ForMember(d => d.OwnerId, cfg => cfg.MapFrom(src => src.Dog.Owner.Id))
                .ForMember(d => d.OwnerName, cfg => cfg.MapFrom(src => src.Dog.Owner.UserName));

            CreateMap<DogTemperament, Temperament>()
               .ForMember(d => d.Id, cfg => cfg.MapFrom(src => src.Id))
               .ForMember(d => d.DogId, cfg => cfg.MapFrom(src => src.DogId));

            CreateMap<Biography, DogBiography>()
                .ForMember(d => d.Id, cfg => cfg.MapFrom(src => src.Id))
                .ForMember(d => d.DogId, cfg => cfg.MapFrom(src => src.DogId))
                .ForMember(d => d.DogName, cfg => cfg.MapFrom(src => src.Dog.Name))
                .ForMember(d => d.OwnerId, cfg => cfg.MapFrom(src => src.Dog.Owner.Id))
                .ForMember(d => d.OwnerName, cfg => cfg.MapFrom(src => src.Dog.Owner.UserName));

            CreateMap<DogBiography, Biography>()
                .ForMember(d => d.Id, cfg => cfg.MapFrom(src => src.Id))
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
