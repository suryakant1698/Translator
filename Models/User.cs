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


        public string SaveNewUser(User user)
        {
            TranslatorEntities newUser = new TranslatorEntities();
            tblCustomer User = new tblCustomer();
            User.Name = user.Name;
            User.Email = user.Email;
            User.Password = Crypto.Hash(user.Password);
            User.ActivationCode = Guid.NewGuid();
            //User.Password = user.Password;
            User.Mothertounge = user.Mothertounge;
            User.isEmailVerified =false;
           // User.ActivationCode = user.ActivationCode;
            newUser.tblCustomers.Add(User);
            newUser.SaveChanges();
            return User.ActivationCode.ToString();
        }
        public string verifyAccount(string id)
        {
            using (TranslatorEntities db = new TranslatorEntities())
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var user = db.tblCustomers.Where(a=>a.ActivationCode==new Guid(id)).FirstOrDefault();
                if (user == null)
                    return "Invalid Request";
                else
                {
                    if (!(bool)user.isEmailVerified)
                        return "Account already verified";
                    else
                    {
                        user.isEmailVerified = true;
                        db.SaveChanges();
                        return "Account activated succesfuly";
                    }
                }
            }
        }

        public string saveResetCodeForForgotPassword(string Email)
        {
            using (TranslatorEntities db = new TranslatorEntities())
            {
                var user = db.tblCustomers.Where(a => a.Email == Email).FirstOrDefault();
                if (user != null)
                {
                    string resetCode = Guid.NewGuid().ToString();                    
                    user.ResetPassworCode = resetCode;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    return resetCode;
                    //ViewBag.Message = "Forgot Password link has been sent to your provided email address if registered";
                }
                else return null;
            }

        }
    }
}