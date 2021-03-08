using AutoMapper;
using DogMatch.Shared.Models;
using DogMatch.Domain.Data.Models;
using System;
using System.Linq;

namespace DogMatch.Domain.Infrastructure
{
    public class AutoMapperProfileDomainConfiguration : Profile
    {
        /// <summary>
        /// Initializes a new instance of configuration class
        /// </summary>
        public AutoMapperProfileDomainConfiguration() : this("DogMatch") { }

        /// <summary>
        /// Initializes a new instance of configuration class
        /// </summary>
        /// <param name="profileName">profile name</param>
        protected AutoMapperProfileDomainConfiguration(string profileName) : base(profileName)
        {
            CreateMap<Dog, Dogs>()
                .ForMember(d => d.Id, cfg => cfg.MapFrom(src => src.Id))
                .ForMember(d => d.Name, cfg => cfg.MapFrom(src => src.Name))
                .ForMember(d => d.Breed, cfg => cfg.MapFrom(src => src.Breed))
                .ForMember(d => d.Birthday, cfg => cfg.MapFrom(src => src.Birthday))
                .ForMember(d => d.Gender, cfg => cfg.MapFrom(src => 
                    src.Gender == "female" ? 'f' : 
                    src.Gender == "male" ? 'm' : ' '
                ))
                .ForMember(d => d.Weight, cfg => cfg.MapFrom(src => src.Weight))  
                .ForMember(d => d.Colors, cfg => cfg.Ignore())
                .ForMember(d => d.Owner, cfg => cfg.Ignore()) // ignore temporarily
                .ForMember(d => d.DogProfileImage, cfg => cfg.Ignore());

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
                .ForMember(d => d.Gender, cfg => cfg.MapFrom(src => 
                    src.Gender == 'f' ? "female" : 
                    src.Gender == 'm' ? "male" : "unknown"
                ))
                .ForMember(d => d.Weight, cfg => cfg.MapFrom(src => src.Weight))
                .ForMember(d => d.ProfileImage, cfg => cfg.MapFrom(src => 
                    src.DogProfileImage != null ? 
                    "/ProfileImage/" + src.DogProfileImage.Filename : 
                    "dogmatch_paw.png"
                ))
                .ForMember(d => d.Colors, cfg => cfg.MapFrom(src => src.Colors.Select(c => c.ColorString).ToList()))
                .ForMember(d => d.OwnerId, cfg => cfg.MapFrom(src => src.Owner.Id))
                .ForMember(d => d.Owner, cfg => cfg.MapFrom(src => src.Owner.UserName)); // using username for owner until collecting first/last name of user

            CreateMap<Dogs, Match>()
                .ForMember(d => d.DogId, cfg => cfg.MapFrom(src => src.Id))
                .ForMember(d => d.DogName, cfg => cfg.MapFrom(src => src.Name))
                .ForMember(d => d.Breed, opt => {
                    opt.PreCondition(src => (src.Breed != null));
                    opt.MapFrom(src => src.Breed);
                })
                .ForMember(d => d.Age, opt => {
                    opt.PreCondition(src => (src.Birthday != null));
                    opt.MapFrom(src => GetAge(src.Birthday));
                })
                .ForMember(d => d.Gender, cfg => cfg.MapFrom(src => 
                    src.Gender == 'f' ? "female" : 
                    src.Gender == 'm' ? "male" : "unknown"
                ))
                .ForMember(d => d.Weight, cfg => cfg.MapFrom(src => src.Weight))
                .ForMember(d => d.ProfileImage, cfg => cfg.MapFrom(src => 
                    src.DogProfileImage != null ? 
                    "/ProfileImage/" + src.DogProfileImage.Filename : 
                    "dogmatch_paw.png"
                ))                
                .ForMember(d => d.OwnerId, cfg => cfg.MapFrom(src => src.Owner.Id))
                .ForMember(d => d.OwnerName, cfg => cfg.MapFrom(src => src.Owner.UserName));

            CreateMap<string, Color>()
                .ForMember(d => d.ColorString, cfg => cfg.MapFrom(src => src))
                .ForMember(d => d.Id, cfg => cfg.Ignore())
                .ForMember(d => d.DogId, cfg => cfg.Ignore())
                .ForMember(d => d.ColorGUID, cfg => cfg.Ignore());               

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

            CreateMap<DogImages, AlbumImage>()
                .ForMember(d => d.Id, cfg => cfg.MapFrom(src => src.Id))
                .ForMember(d => d.ImageString, cfg => cfg.MapFrom(src => "/AlbumImage/" + src.Filename))
                .ForMember(d => d.Extension, cfg => cfg.Ignore())
                .ForMember(d => d.Delete, cfg => cfg.MapFrom(src => false));
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
