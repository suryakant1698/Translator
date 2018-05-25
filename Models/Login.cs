using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Login
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name ="Remember Me?")]
        public bool RememberMe { get; set; }

        public bool VerifyUser(Login user)
        {
            using (TranslatorDBEntities db = new TranslatorDBEntities())
            {
                var userCredential = db.tblCustomers.Where(a=>a.Email==user.Email).FirstOrDefault();
                if (userCredential == null)
                    return false;
                else
                {
                    if (string.Compare(Crypto.Hash(user.Password), userCredential.Password) == 0 && (bool)userCredential.isEmailVerified)
                    {                        
                        return true;
                    }
                    else return false;

                }
            }
        }

    }
}