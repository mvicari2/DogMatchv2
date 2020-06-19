using System;

namespace DogMatch.Shared.Models
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
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
    }
}
