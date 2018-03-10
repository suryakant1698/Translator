using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class User
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailValidator]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Mothertounge { get; set; }
        public Nullable<bool> isEmailVerified { get; set; }
        public Nullable<System.Guid> ActivationCode { get; set; }
        public string ResetPassworCode { get; set; }
        public Nullable<int> ImageId { get; set; }


        public void SaveNewUser(User user)
        {
            TranslatorEntities newUser = new TranslatorEntities();
            tblCustomer User = new tblCustomer();
            User.Name = user.Name;
            User.Email = user.Email;
            User.Password = user.Password;
            User.Mothertounge = user.Mothertounge;
            User.isEmailVerified = user.isEmailVerified;
            User.ActivationCode = user.ActivationCode;
            newUser.tblCustomers.Add(User);
            newUser.SaveChanges();
        }
    }
}