using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class EmailValidator:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var user = (User)validationContext.ObjectInstance;
            TranslatorDBEntities newUser = new TranslatorDBEntities();
            if (user.Email == null) return new ValidationResult("Email is required");
            var isValid = newUser.tblCustomers.SingleOrDefault(c=>c.Email==user.Email);            
            if (isValid == null)
                return ValidationResult.Success;
            else
                return new ValidationResult("Email already registered");
        }
    }
}