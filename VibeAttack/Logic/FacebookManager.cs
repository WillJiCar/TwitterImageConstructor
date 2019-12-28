using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Facebook;

namespace VibeAttack.Logic
{
    public class FacebookManager
    {
        public FacebookClient myFBAPI;
        public string appID;
        public string apiKey;
        public string appSecret;

        public FacebookManager()
        {
            //myFBAPI = new FacebookClient()
        }
    }
}