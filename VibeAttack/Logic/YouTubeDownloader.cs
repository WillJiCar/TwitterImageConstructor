using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using VideoLibrary;
using MediaToolkit;
using MediaToolkit.Model;

namespace VibeAttack.Logic
{
    public class YouTubeDownloader
    {
        string clientFileName;
        string downloadUrlMP3;
        bool isDownload = false;

        public void Download(string ID)
        {
            //Gets video Information
            var youtube = YouTube.Default; //New Youtube object
            var video = youtube.GetVideo("https://www.youtube.com/watch?v=" + ID);
            clientFileName = video.FullName;

            //Define filepaths and create directories
            var videoDirectory = HttpContext.Current.Server.MapPath("~/downloads/temp_video");
            DirectoryInfo videoDirInfo = Directory.CreateDirectory(videoDirectory);

            var audioDirectory = HttpContext.Current.Server.MapPath("~/downloads/temp_audio");
            DirectoryInfo audioDirInfo = Directory.CreateDirectory(audioDirectory);
            
            //Download file to server
            var videoFilePath = Path.Combine(videoDirectory, clientFileName);
            File.WriteAllBytes(videoFilePath, video.GetBytes());

            var audioFilePath = Path.Combine(audioDirectory, clientFileName + ".mp3");
            var inputFile = new MediaFile { Filename = videoFilePath };
            var outputFile = new MediaFile { Filename = audioFilePath };

            using (var engine = new Engine(HttpContext.Current.Server.MapPath("~/content/ffmpeg.exe")))
            {
                engine.GetMetadata(inputFile);
                engine.Convert(inputFile, outputFile);
            }

            var urlSafeName = clientFileName.Replace(" ", "%20");
            var audioDownloadLink = "http://www." + HttpContext.Current.Request.Url.Host + "/downloads/temp_audio/" + urlSafeName + ".mp3";
            downloadUrlMP3 = audioDownloadLink;

            //RemoveData(new string[] { audioFilePath, videoFilePath });
        }

        public object GetData()
        {
            return new { downloadURLMP3 = downloadUrlMP3, fileName = clientFileName };
        }


        //Not working, using a package instead
        private void ManualDownload(string ID)
        {
            try
            {
                string videoInfoURL = "https://www.youtube.com/watch?v=" + ID;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(videoInfoURL);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Requests the webpage and retrieves the response.

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                //Response is stored into a reader

                string videoInfo = HttpUtility.HtmlDecode(reader.ReadToEnd());



                List<Uri> urls = GetVideoDownloadUrls(videoInfo);
                string videoTitle = HttpUtility.HtmlDecode(GetVideoTitle(videoInfo));

                foreach (Uri url in urls)
                {
                    NameValueCollection queryValues = HttpUtility.ParseQueryString(url.OriginalString);

                    if (queryValues["type"].ToString().StartsWith("video/mp4"))
                    {
                        string downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        string sFilePath = string.Format(Path.Combine(downloadPath, "Downloads\\{0}.mp4"), videoTitle);

                        WebClient client = new WebClient();
                        client.DownloadFileCompleted += client_DownloadFileCompleted;
                        client.DownloadFileAsync(url, sFilePath);
                        isDownload = true;
                        break;
                    }
                }

                if (urls.Count == 0 || !isDownload)
                {
                    //Failure
                }

                /*
                //NameValueCollection videoParams = HttpUtility.ParseQueryString(videoInfo);
                if (videoParams["reason"] != null)
                {
                    reasonResponse = videoParams["reason"];
                    return;
                }

                string[] videoURLs = videoParams["url_encoded_fmt_stream_map"].Split(',');
                foreach(string vURL in videoURLs)
                {
                    string sURL = HttpUtility.HtmlDecode(vURL);
                    NameValueCollection urlParams = HttpUtility.ParseQueryString(sURL);
                    string videoFormat = HttpUtility.HtmlDecode(urlParams["type"]);

                    sURL = HttpUtility.HtmlDecode(urlParams["url"]);
                    sURL += "&signature=" + HttpUtility.HtmlDecode(urlParams["sig"]);
                    sURL += "&type=" + videoFormat;
                    sURL += "&title=" + HttpUtility.HtmlDecode(videoParams["title"]);

                    videoFormat = urlParams["quality"] + " - " + videoFormat.Split(';')[0].Split('/')[1];


                }
                */
            }
            catch
            {

            }
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //Success!
        }

        private List<Uri> GetVideoDownloadUrls(string videoInfo)
        {
            string pattern = "url=";
            string queryStringEnd = "&quality";

            List<Uri> finalUrls = new List<Uri>();
            List<string> urlList = Regex.Split(videoInfo, pattern).ToList<string>();

            foreach(string url in urlList)
            {
                string sURL = HttpUtility.UrlDecode(url).Replace("\\u0026", "&");
                int index = sURL.IndexOf(queryStringEnd, StringComparison.Ordinal);
                if(index > 0)
                {
                    sURL = sURL.Substring(0, index).Replace("&sig=", "&signature=");
                    finalUrls.Add(new Uri(Uri.UnescapeDataString(sURL)));
                }
            }
            return finalUrls;
        }

        private string GetVideoTitle(string videoInfo)
        {
            int startIndex = videoInfo.IndexOf("<title>");
            int endIndex = videoInfo.IndexOf("</title>");

            if(startIndex > -1 && endIndex > -1)
            {
                string title = videoInfo.Substring(startIndex + 7, endIndex - (startIndex + 7));
                return title;
            }
            return "null";
        }

        private void RemoveData(string[] filePaths)
        {
            /*
            var startTime = TimeSpan.Zero;
            var endTime = TimeSpan.FromMinutes(1);
            System.Timers.Timer once = new System.Timers.Timer(60000);
            once.Elapsed += (s, e) =>
            {
                foreach(string filePath in filePaths)
                {
                    if (File.Exists(filePath))
                    {
                        //File.Delete(filePath);
                    }
                    
                }
            };
            once.AutoReset = false;
            once.Start();
            */
        }
    }
}