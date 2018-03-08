﻿using System;
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

        [HttpGet]
        [ActionName("Login")]
        public ActionResult Login_Get()
        {

            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        [ValidateAntiForgeryToken]
        public ActionResult Login_Post(Login login, string ReturnUrl = "")
        {
            string message = "";
            using (TranslatorEntities te = new TranslatorEntities())
            {
                var user = te.tblCustomers.Where(n => n.Email == login.Email).FirstOrDefault();
                if (user != null)
                {
                    if (string.Compare(Crypto.Hash(login.Password), user.Password) == 0)
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
                        else return RedirectToAction("Login", "Register");
                    }
                    else {
                        message = "Invalid Credential provided";
                    }
                }
                else message = "Invalid Credential Provided";
            }
            ViewBag.Message = message;
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
        public ActionResult Register_Post([Bind(Exclude = "isEmailVerified,ActivationCode")]User user)
        {
            bool status = false;
            string message = "";
            if (!ModelState.IsValid)
                return View();
            user.ActivationCode = Guid.NewGuid();
            user.Password = Crypto.Hash(user.Password);
            user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
            user.isEmailVerified = false;
            User newUser = new User();
            newUser.SaveNewUser(user);
            sendVerificationLinkEmail(user.Email, user.ActivationCode.ToString(), "VerifyAccount");
            message = "Registration Succesful an email has been sent to your registered gmail account";
            ViewBag.Message = message;
            status = true;
            ViewBag.Status = status;
            return View();


        }

        [HttpGet]
        [ActionName("Index")]
        public ActionResult Register_Get()
        {
            return View();
        }


        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            if (id != null)
            {
                using (TranslatorEntities te = new TranslatorEntities())
                {
                    bool status = false;
                    te.Configuration.ValidateOnSaveEnabled = false;
                    var v = te.tblCustomers.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                    if (v != null)
                    {
                        if (!(bool)v.isEmailVerified)
                        {
                            ViewBag.Message = "Your account has been activated aready";
                            ViewBag.Status = false;
                        }
                        else
                        {
                            v.isEmailVerified = true;
                            status = true;
                            te.SaveChanges();
                        }
                    }
                    else
                    {
                        status = false;
                        ViewBag.Message = "Invalid Request";
                    }
                    ViewBag.status = status;
                }


            }
            return View();
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
            bool status = false;
            using (TranslatorEntities db = new TranslatorEntities())
            {
                var user = db.tblCustomers.Where(a => a.Email == Email).FirstOrDefault();
                if (user != null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    sendVerificationLinkEmail(user.Email, resetCode, "ResetPassword");
                    user.ResetPassworCode = resetCode;
                    db.Configuration.ValidateOnSaveEnabled = false;

                    db.SaveChanges();
                }
                else status = true;
            }

            return View();
        }

        [NonAction]
        public void sendVerificationLinkEmail(string email, string activationCode, string EmailFor)
        {
            //var scheme = Request.Url.Scheme;
            //var host = Request.Url.Host;
            //var port = Request.Url.Port;
            var verifyUrl = "/Register/'" + EmailFor + "'/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("suryakant.rocky@gmail.com");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "suryasharma";
            string subject = "", body = "";
            if (EmailFor == "VerifyAccount")
            {

                subject = "Your account is succesfuly created ";
                body = "<br/> <br/> This is to inform you that your translator account has been created succesfully.Click on the below link to verify your account" +
                    "<a href='" + link + "'>Click here to activate</a>";


            }
            else
            {
                subject = "Reset Password";
                body = "We got request for reset your account passeord.Please click on the below link to reset password for your translator account"
                    + "<br/><br/><a href='" + link + "'>Click here</a>";
            }
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        [HttpGet]
        [ActionName("ResetPassword")]
        public ActionResult ResetPassword_Get(string id)
        {
            using (TranslatorEntities db = new TranslatorEntities())
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
            var message = "";
            if (ModelState.IsValid)
            {
                using (TranslatorEntities db = new TranslatorEntities())
                {
                    var user = db.tblCustomers.Where(a => a.ResetPassworCode == User.ResetCode).FirstOrDefault();
                    user.Password = Crypto.Hash(User.NewPassword);
                    user.ResetPassworCode = "";
                    db.Configuration.ValidateOnSaveEnabled=false;
                    db.SaveChanges();
                    message = "Your password has been changed succesfuly you can login with your new password now";
                    
                }
                ViewBag.message = message;

            }

            return View();
        }
    }
}