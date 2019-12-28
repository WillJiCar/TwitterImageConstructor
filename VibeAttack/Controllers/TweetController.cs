using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tweetinvi;
using VibeAttack.Models;
using VibeAttack.Logic;

namespace VibeAttack.Controllers
{
    public class TweetController : Controller
    {
        TweetImageBuilder tBuilder = new TweetImageBuilder();
        // GET: Tweet
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(TwitterIDModel twitterIDModel)
        {
            Tweetinvi.Models.ITweet tweet = Tweet.GetTweet(Convert.ToInt64(twitterIDModel.twitterID));
            tBuilder.createImage(tweet);
            return RedirectToAction("Index", "Home");
        }
    }
}