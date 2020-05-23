using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using VibeAttack.Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc.Cors;

namespace VibeAttack.Controllers
{
    [ApiController]
    public class YouTubeController : Controller
    {
        YouTubeDownloader downloader = new YouTubeDownloader();
        // GET: YouTube

        //[System.Web.Mvc.HttpGet]
        //[EnableCors]
        public System.Web.Mvc.ActionResult Index()
        {
            string id = Request.QueryString["id"].ToString();
            downloader.Download(id);
            object data = downloader.GetData();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}