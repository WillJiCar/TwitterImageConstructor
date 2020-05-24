using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc;
using VibeAttack.Logic;

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