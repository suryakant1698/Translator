using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication4.Models;

namespace WebApplication4.Controllrs
{
    public class RegisterController : Controller
    {
        // GET: Register
        [HttpGet][ActionName("Login")]
        public ActionResult Login_Get()
        {
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                return RedirectToAction("Index", "Home");
            ViewBag.DetectSrc = ApiProperties.DetectSourcceLanguage;
            ViewBag.GetLanguages = ApiProperties.GetAvailableLanguages;
            ViewBag.Translate = ApiProperties.Translate;
            ViewBag.key = ApiProperties.key;
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        [ValidateAntiForgeryToken]
        public ActionResult Login_Post(Login login, string ReturnUrl = "")
        {

            ViewBag.DetectSrc = ApiProperties.DetectSourcceLanguage;
            ViewBag.GetLanguages = ApiProperties.GetAvailableLanguages;
            ViewBag.Translate = ApiProperties.Translate;
            ViewBag.key = ApiProperties.key;
            Login newUser = new Login();
            if (!newUser.VerifyUser(login))
            {
                ViewBag.Message = "Invalid Credentials";
            }
            else
            {
                int timeout = login.RememberMe ? 525600 : 20;
                var ticket = new FormsAuthenticationTicket(login.Email, login.RememberMe, timeout);
                var encrypted = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                cookie.Expires = DateTime.Now.AddMinutes(timeout);
                cookie.HttpOnly = true;
                Response.Cookies.Add(cookie);
                if (Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult Register_Post([Bind(Exclude = "isEmailVerified,ActivationCode")]User user,string Mothertounge)
        {

            ViewBag.navbar = false;
            if (!ModelState.IsValid)
                return View();
            ViewBag.key = ApiProperties.key;
            ViewBag.GetLanguages = ApiProperties.GetAvailableLanguages;
            user.Mothertounge = Mothertounge;
            User newUser = new User();
            string activationCode = newUser.SaveNewUser(user);
            sendVerificationLinkEmail(user.Email, activationCode, "VerifyAccount");
            ViewBag.status = true;
            ViewBag.Message = "Registration Succesful an email has been sent to your registered gmail account";
            return View();
        }

        [HttpGet]
        [ActionName("Index")]
        public ActionResult Register_Get()
        {
            ViewBag.key = ApiProperties.key;
            ViewBag.GetLanguages = ApiProperties.GetAvailableLanguages;
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                return RedirectToAction("Index", "Home");
            ViewBag.DetectSrc = ApiProperties.DetectSourcceLanguage;
            ViewBag.GetLanguages = ApiProperties.GetAvailableLanguages;
            ViewBag.Translate = ApiProperties.Translate;
            ViewBag.key = ApiProperties.key;

            return View();
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
             if (id != null)
            {
                User user = new User();
                string message = user.verifyAccount(id);
                if (message == "Invalid Request")
                    ViewBag.status = false;
                else ViewBag.status = true;
                ViewBag.message = message;
                return View();
            }
            else return HttpNotFound();
        }

        [HttpGet]
        [ActionName("ForgotPassword")]
        public ActionResult ForgotPassword_Get()
        {
            return View();
        }

        [HttpPost]
        [ActionName("ForgotPassword")]
        public ActionResult ForgotPassword_Post(string Email)
        {
            User currenUser = new User();
            bool status = false;
            string resetCode=currenUser.saveResetCodeForForgotPassword(Email);
            if (resetCode == null)
            {
                status = true;
            }
            else
            sendVerificationLinkEmail(Email, resetCode, "ResetPassword");
            ViewBag.message = "A password reset link has been sent to your supplied email address if already registered";
            return View();
        }

        

        [HttpGet]
        [ActionName("ResetPassword")]
        public ActionResult ResetPassword_Get(string id)
        {
            using (TranslatorDBEntities db = new TranslatorDBEntities())
            {
                var user = db.tblCustomers.Where(a => a.ResetPassworCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPassword newUSer = new ResetPassword();
                    newUSer.ResetCode = id;
                    return View(newUSer);
                }
                else return HttpNotFound();
            }       
        }

        [HttpPost]
        [ActionName("ResetPassword")]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword_Post(ResetPassword User)
        {         
            if (ModelState.IsValid)
            {
                ResetPassword userData = new ResetPassword();
                userData.setPassword(User);
                 ViewBag.message = "Your password has been changed succesfuly you can login with your new password now";

            }

            return View();
        }
        [NonAction]
        public void sendVerificationLinkEmail(string email, string activationCode, string EmailFor)
        {
            var verifyUrl = "";
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            string subject = "", body ="";
            if (EmailFor == "VerifyAccount")
            {
                verifyUrl = "/Register/VerifyAccount/" + activationCode;
                subject = "Your account is succesfuly created ";
                body = "<br/> <br/> This is to inform you that your translator account has been created succesfully.Click on the below link to verify your account" +
                    "<a href='" + link + "'>Click here to activate</a>";
            }
            else
            {
                verifyUrl = "/Register/ResetPassword/" + activationCode;
                subject = "Reset Password";
                body = "We got request for reset your account passeord.Please click on the below link to reset password for your translator account"
                    + "<br/> <br/> <a href=" + link + ">Click here to activate Now</a>";
            }
            Mail.Email(body,email,subject);

        }
    }
}
