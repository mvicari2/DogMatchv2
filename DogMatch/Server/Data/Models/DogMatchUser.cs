using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DogMatch.Server.Data.Models
{
    public class DogMatchUser : IdentityUser
    {
        public DogMatchUser()
        {
            Dogs = new HashSet<Dogs>();
            DogsCreatedByUser = new HashSet<Dogs>();
            DogsModifiedByUser = new HashSet<Dogs>();
            Addresses = new HashSet<Addresses>();
            AddressesCreatedByUser = new HashSet<Addresses>();
            AddressesModifiedByUser = new HashSet<Addresses>();
            AlbumImages = new HashSet<Images>();
            BiographiesCreatedByUser = new HashSet<Biography>();
            BiographiesModifiedByUser = new HashSet<Biography>();
            TemperamentsCreatedByUser = new HashSet<Temperament>();
            TemperamentsModifiedByUser = new HashSet<Temperament>();
        }
        public string FirstName { get; set; }
        public string MI { get; set; }
        public string LastName { get; set; }
        public int? PrimaryAddressId { get; set; }
        public DateTime? Birthday { get; set; }
        public byte[] UserImage { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastModified { get; set; }
        public string LastModifiedBy { get; set; }
        public Guid UserGUID { get; set; }
        public virtual Addresses PrimaryAddress { get; set; }
        public virtual ICollection<Dogs> Dogs { get; set; }
        public virtual ICollection<Dogs> DogsCreatedByUser { get; set; }
        public virtual ICollection<Dogs> DogsModifiedByUser { get; set; }
        public virtual ICollection<Addresses> Addresses { get; set; }
        public virtual ICollection<Addresses> AddressesCreatedByUser { get; set; }
        public virtual ICollection<Addresses> AddressesModifiedByUser { get; set; }
        public virtual ICollection<Images> AlbumImages { get; set; }
        public virtual ICollection<Biography> BiographiesCreatedByUser { get; set; }
        public virtual ICollection<Biography> BiographiesModifiedByUser { get; set; }
        public virtual ICollection<Temperament> TemperamentsCreatedByUser { get; set; }
        public virtual ICollection<Temperament> TemperamentsModifiedByUser { get; set; }
    }
}