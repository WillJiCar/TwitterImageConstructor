using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VibeAttack.Models;

namespace VibeAttack.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(PictureModel picture)
        {
            if(picture.Files != null)
            {
                foreach (var file in picture.Files)
                {
                    //https://www.youtube.com/watch?v=9LPIu_BvatE
                    //TODO: Check if filesize, and file dimensions match
                    if (file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                        file.SaveAs(path);
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}