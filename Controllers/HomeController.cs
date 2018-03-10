using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication4.Models;
using System.Web.Security;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [Authorize]
        public ActionResult Index()
        {
            //string email = Request.Cookies["Key"].Value;
            string email = Request.Cookies[FormsAuthentication.FormsCookieName].Value;

            return View();
            
        }
        [HttpGet]
        [ActionName("AddPic")]
        public ActionResult AddPic_Get()
        {
            Dp pic = new Dp();
            return View(pic);
        }

        [HttpPost]
        [ActionName("AddPic")]
        public ActionResult AddPic_Post(Dp imageModel,HttpPostedFileBase image)
        {
            
            using (TranslatorEntities db = new TranslatorEntities())
            {
                if (image != null)
                {
                    imageModel.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(imageModel.ImageData, 0, image.ContentLength);

                    //db.tblCustomers.Add(imageModel);
                }
                else
                {
                    ViewBag.Message = "Please  select a pic to upload";
                    return View();
                }
            }
                return View();
        }
    }
}