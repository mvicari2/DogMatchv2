using System;
using System.Collections.Generic;

namespace DogMatch.Domain.Data.Models
{
    public class Addresses
    {
        public Addresses()
        {
            Dogs = new HashSet<Dogs>();
        }
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Apt { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }
        public Guid AddressGUID { get; set; }
        public virtual DogMatchUser User { get; set; }
        public virtual DogMatchUser PrimaryAddressUser { get; set; }
        public virtual DogMatchUser CreatedByUser { get; set; }
        public virtual DogMatchUser ModifiedByUser { get; set; }        
        public virtual ICollection<Dogs> Dogs { get; set; }
    }
}