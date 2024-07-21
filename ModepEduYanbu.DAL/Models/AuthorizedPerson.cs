using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class AuthorizedPerson
    {
        public string AuthorizedPersonId { get; set; }
        [Required]
        public string IdNo { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string SchoolMinistryNo { get; set; }
        public bool ViaNoor { get; set; }
        public DateTime CreatedData { get; set; } = DateTime.Now;

        //Role: one-to-many relationship
        public string RoleId { get; set; }
        [ForeignKey("RoleId")]
        public ApplicationRole Role { get; set; }


        //AddByUser: one-to-many relationship: One user can add more than an authorized person,
        //but one person is added by one user.
        public string AddedByUserId { get; set; }
        [ForeignKey("AddedByUserId")]
        public ApplicationUser AddedByUser { get; set; }
    }
}
