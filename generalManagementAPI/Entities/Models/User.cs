using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set;}
        public string Password { get; set;}
        public string Email { get; set;}
        public string PhoneNumber { get; set;}
        public string Name { get; set;}
        public string Surname { get; set;}
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfRegister { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
}