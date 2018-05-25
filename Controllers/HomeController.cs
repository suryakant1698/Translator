using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication4.Models;
using Newtonsoft.Json;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home

        [Authorize]
        public ActionResult Index()
        {
            #region
            ViewBag.navbar = true;
            ViewBag.DetectSrc = ApiProperties.DetectSourcceLanguage;
            ViewBag.GetLanguages = ApiProperties.GetAvailableLanguages;
            ViewBag.Translate = ApiProperties.Translate;
            ViewBag.key = ApiProperties.key;
            #endregion
            DisplayPic();
            return View();
        }

        public ActionResult Translator()
        {
            ViewBag.DetectSrc = ApiProperties.DetectSourcceLanguage;
            ViewBag.GetLanguages = ApiProperties.GetAvailableLanguages;
            ViewBag.Translate = ApiProperties.Translate;
            ViewBag.key = ApiProperties.key;
            return View();
        }

        [HttpGet]
        [ActionName("AddPic")]
        [Authorize]
        public ActionResult AddPic_Get()
        {
            ViewBag.navbar = true;
            DisplayPic();
            return View();
        }

        [HttpPost]
        public ActionResult AddPic(Dp imageModel, HttpPostedFileBase image)
        {


            string fileExtension = "", fileType = "";
            using (TranslatorDBEntities db = new TranslatorDBEntities())
            {
                if (image != null)
                {
                    int slashIndex = image.ContentType.IndexOf("/");
                    fileType = image.ContentType.Substring(0, slashIndex);
                    if (fileType != "img")
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

                        //db.tblCustomers.Add(imageModel);
                    }
                    else
                    {
                        ViewBag.message = "Please select an image only";

                    }
                }

                else
                {
                    ViewBag.Message = "Please  select a picture to upload";

                }
            }
            DisplayPic();
            return View(imageModel);

        }

        [NonAction]
        public void DisplayPic()
        {
            using (TranslatorDBEntities db = new TranslatorDBEntities())
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

        [HttpPost]
        public JsonResult SaveTranslation(History trView)
        {
            string email = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            TranslatorDBEntities db = new TranslatorDBEntities();
            var user = db.tblCustomers.Where(a => a.Email == email).FirstOrDefault();
            tblHistory newData = new tblHistory();
            newData.UserID = user.id;
            var jsonString = trView.ToString();

            newData.SourceLanguage = trView.SourceLanguage;
            newData.SourceText = trView.SourceText;
            newData.DestinationLanguage = trView.DestinationLanguage;
            newData.DestinationText = trView.DestinationText;
            db.tblHistories.Add(newData);
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}