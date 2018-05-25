using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class ResetPassword
    {

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "Password and confirm password do not match")]
        public string ConfirmPassword { get; set; }
        public string ResetCode { get; set; }

        public void setPassword(ResetPassword userData)
        {
            using (TranslatorDBEntities db = new TranslatorDBEntities())
            {
                var user = db.tblCustomers.Where(a => a.ResetPassworCode == userData.ResetCode).FirstOrDefault();
                user.Password = Crypto.Hash(userData.NewPassword);
                user.ResetPassworCode = "";
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
            }
        }
    }      
}