using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home

        [Authorize]
        public ActionResult Index()
        {
            var user = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            return View();

        }

        [Authorize]
        [HttpGet]
        [ActionName("AddPic")]
        public ActionResult AddPic_Get()
        {
            Dp pic = new Dp();
            return View(pic);
        }

        [Authorize]
        [HttpPost]
        [ActionName("AddPic")]
        public ActionResult AddPic_Post(Dp imageModel, HttpPostedFileBase image)
        {
            using (TranslatorEntities db = new TranslatorEntities())
            {
                if (image != null)
                {
                    imageModel.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(imageModel.ImageData, 0, image.ContentLength);
                    string email = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                    tblCustomer currentUser = db.tblCustomers.Where(a => a.Email == email).FirstOrDefault();
                    currentUser.ImageData = imageModel.ImageData;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    ViewBag.Message = "Photo uploaded succesfuly";
                    return View(imageModel);
                    //db.tblCustomers.Add(imageModel);
                }
                else
                {
                    ViewBag.Message = "Please  select a pic to upload";
                    return View();
                }
            }
        }
    }
}