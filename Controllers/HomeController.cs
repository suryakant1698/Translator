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
            DisplayPic();
            return View();
        }

        [Authorize]
        [HttpGet]
        [ActionName("AddPic")]
        public ActionResult AddPic_Get()
        {

            return View();
        }

        [Authorize]
        [HttpPost]
        [ActionName("AddPic")]
        public ActionResult AddPic_Post(Dp imageModel, HttpPostedFileBase image)
        {
            DisplayPic();
            string fileExtension = "", fileType = "";
            using (TranslatorEntities db = new TranslatorEntities())
            {
                if (image != null)
                {
                    int slashIndex = image.ContentType.IndexOf("/");
                    fileType = image.ContentType.Substring(0, slashIndex);
                    if (fileType != "image")
                    {
                        imageModel.ImageData = new byte[image.ContentLength];
                        image.InputStream.Read(imageModel.ImageData, 0, image.ContentLength);
                        string email = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        tblCustomer currentUser = db.tblCustomers.Where(a => a.Email == email).FirstOrDefault();
                        currentUser.ImageData = imageModel.ImageData;
                        string postedFileContentType = image.ContentType;
                        fileExtension = image.ContentType.Substring(slashIndex + 1, postedFileContentType.Length - slashIndex - 1);
                        currentUser.ImageType = fileType;
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        ViewBag.Message = "Photo uploaded succesfuly";

                        return View(imageModel);
                        //db.tblCustomers.Add(imageModel);
                    }
                    else
                    {
                        ViewBag.message = "Please select an image only";
                        return View();
                    }
                }

                else
                {
                    ViewBag.Message = "Please  select a pic to upload";
                    return View();
                }
            }

        }
        [NonAction]
        public void DisplayPic()
        {
            using (TranslatorEntities db = new TranslatorEntities())
            {
                string email = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                tblCustomer currentUser = db.tblCustomers.Where(a => a.Email == email).FirstOrDefault();
                if (currentUser.ImageData != null)
                {
                    string fileType = currentUser.ImageType;
                    ViewBag.Base64String = "data:image/" + fileType + ";base64," + Convert.ToBase64String(currentUser.ImageData, 0, currentUser.ImageData.Length);
                }
            }
        }
    }
}