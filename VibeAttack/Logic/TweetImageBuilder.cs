using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Web;
using Tweetinvi.Models;
using NeoSmart.Unicode;
using System.Linq;

namespace VibeAttack.Logic
{
    public class TweetImageBuilder
    {
        //TODO - For each MediaLocation, drawimage
        WebClient web = new WebClient();
        string tmpPath;
        string path = HttpContext.Current.Server.MapPath("Image");
        string mediaPath;
        public void createImage(ITweet tweet)
        {
            Directory.CreateDirectory(path + "/tmp");
            Directory.CreateDirectory(path + "/Unused");
            List<string> mediaLocations = new List<string>();
            mediaPath = HttpContext.Current.Server.MapPath("Image/Unused");
            tmpPath = HttpContext.Current.Server.MapPath("Image/tmp");
            //IF TWEET IMAGE
            //IF VIDEO
            mediaLocations = DownloadMedia(tweet, "media");

            string ppURL = tweet.CreatedBy.ProfileImageUrl;
            web.DownloadFile(ppURL, tmpPath + "\\profilePic.jpg");

            string username = "@" + tweet.CreatedBy.ScreenName;
            string name = tweet.CreatedBy.Name;
            string text = tweet.Text;
            

            //-Embeded Tweet
            ITweet retweet = null;
            string rppURL = null;
            string rName = null;
            string rUsername = null;
            string rText = null;
            List<string> rMediaLocations = new List<string>();
            //IF RETWEET
            if (tweet.QuotedTweet != null)
            {
                retweet = tweet.QuotedTweet;

                rppURL = retweet.CreatedBy.ProfileImageUrl;
                //Profile Picture 24x24
                web.DownloadFile(rppURL, tmpPath + "\\retweetProfilePic.jpg");

                rUsername = "@" + retweet.CreatedBy.ScreenName;
                //Display Name same

                rName = retweet.CreatedBy.Name;
                //Username same

                rText = retweet.Text;
                //Text 23px

                rMediaLocations = DownloadMedia(retweet, "rImages");
            }

            buildImage(username, name, text, tmpPath + "/profilePic.jpg", mediaLocations, rUsername, rName, rText, tmpPath + "/retweetProfilePic.jpg", rMediaLocations);
        }

        List<string> DownloadMedia(ITweet tweet, string prefix)
        {
            List<string> mediaLocations = new List<string>();
            if (tweet.Media.Count > 0)
            {
                for (int i = 0; i < tweet.Media.Count; i++)
                {
                    if (tweet.Media[i].MediaType == "photo")
                    {
                        web.DownloadFile(tweet.Media[i].MediaURL, tmpPath + "\\" + prefix + i + ".jpg");
                        mediaLocations.Add(tmpPath + "/" + prefix + i + ".jpg");
                    }
                    if (tweet.Media[i].MediaType == "video")
                    {
                        web.DownloadFile(tweet.Media[i].MediaURL, tmpPath + "\\" + prefix + i + ".mp4");
                        mediaLocations.Add(tmpPath + "/" + prefix + i + ".mp4");
                    }
                }
                return mediaLocations;
            }
            return null;
        }

        void buildImage(string username, string name, string text, string ppURL, List<string> mediaLocations, string rUsername, string rName, string rText, string rppURL, List<string> rMediaLocations)
        {
            int max = 2;
            //TODO
            //Create square with circle in middle to replicate circle profile picture
            //https://twitter.com/jaeraedell/status/1207893313141006336
            string font = "Segoe UI";
            Font fBoldNormalText = new Font(font, 15, FontStyle.Bold, GraphicsUnit.Pixel);
            Font fNormalText = new Font(font, 15, GraphicsUnit.Pixel);
            Font fLargeText = new Font(font, 23, GraphicsUnit.Pixel);

            Image oImg = new Bitmap(1, 1);   //Create Dummy image for graphics
            Graphics drawing = Graphics.FromImage(oImg); // Graphics from image

            SizeF textSize = new SizeF(0, 0);
            SizeF rTextSize = new SizeF(0, 0);   //Size of Normal Text (Retweet)
            SizeF rNameSize = new SizeF(0, 0);
            List<Image> tImages = new List<Image>();
            List<Image> rImages = new List<Image>();
            //List<MediaPlayer>
            Image iPP = Image.FromFile(@ppURL);
            Image iRPP = null;
            float oImgHeight = 170;
            Image circlePic = Image.FromFile(path + "/circle-pic.png");

            List<float> tImgHeightScales = new List<float>();
            List<float> tImgWidthScales = new List<float>();
            List<float> rImgHeightScales = new List<float>();
            List<float> rImgWidthScales = new List<float>();

            //Find Images from Media List, all with extension .jpg
            //GET SIZE OF OUTPUT IMAGE
            if (text != null)
            {
                textSize = drawing.MeasureString(text, fLargeText, 568);  //Size of Large Text (Tweet)
                oImgHeight += textSize.Height;
            }
            if (mediaLocations != null)
            {
                for (int i = 0; i < mediaLocations.Count; i++)
                {
                    if (mediaLocations[i].Contains(".jpg"))
                    {
                        tImages.Add(Image.FromFile(mediaLocations[i]));
                    }
                }
                if(tImages.Count != 0)
                {
                    List<string> dimensions = CalculateImageDimensions(tImages, max, 568);
                    for (int i = 0; i < dimensions.Count; i++)
                    {
                        tImgHeightScales.Add(float.Parse(dimensions[i].Split('x').ToList()[1]));
                        tImgWidthScales.Add(float.Parse(dimensions[i].Split('x').ToList()[0]));
                    }
                }
                oImgHeight += CalculateTotalImagesHeight(tImgHeightScales, max);
            }
            
            if (rUsername != null)   //If a Retweet as well
            {
                rNameSize = drawing.MeasureString(rName, fBoldNormalText);
                iRPP = Image.FromFile(@rppURL);
                if (rText != null)
                {
                    rTextSize = drawing.MeasureString(rText, fNormalText, 546);
                    oImgHeight += rTextSize.Height;
                }
                if (rMediaLocations != null)
                {
                    for (int i = 0; i < rMediaLocations.Count; i++)
                    {
                        if (rMediaLocations[i].Contains(".jpg"))
                        {
                            rImages.Add(Image.FromFile(rMediaLocations[i]));
                        }
                    }
                    if (rImages.Count != 0)
                    {
                        List<string> dimensions = CalculateImageDimensions(rImages, max, 546);
                        for (int i = 0; i < dimensions.Count; i++)
                        {
                            rImgHeightScales.Add(float.Parse(dimensions[i].Split('x').ToList()[1]));
                            rImgWidthScales.Add(float.Parse(dimensions[i].Split('x').ToList()[0]));
                        }
                        oImgHeight += CalculateTotalImagesHeight(rImgHeightScales, max);
                    }
                }
            }

            oImg.Dispose();
            drawing.Dispose();

            oImg = new Bitmap((int)598, (int)oImgHeight);
            drawing = Graphics.FromImage(oImg);
            drawing.Clear(Color.White);
            Brush textBrush = new SolidBrush(Color.Black);
            Brush grayBrush = new SolidBrush(Color.Gray);
            float currentHeight = 0;

            drawing.DrawImage(iPP, 15, 10, 49, 49);
            drawing.DrawImage(circlePic, 14, 9, 50, 50);
            drawing.DrawString(name, fBoldNormalText, textBrush, 74, 14);
            drawing.DrawString(username, fNormalText, grayBrush, 74, 35);
            currentHeight += 69;
            //IF TEXT
            if (text != null)
            {
                drawing.DrawString(text, fLargeText, textBrush, new Rectangle(15, 69, 568, (int)textSize.Height));
                currentHeight += textSize.Height + 20;
            }
            //IF IMAGE
            if (tImages.Count != 0)
            {
                currentHeight = DrawImages(tImages, max, drawing, currentHeight, tImgWidthScales, tImgHeightScales, 15);
                /*
                int rows = (int)Math.Ceiling((decimal)tImages.Count / max);
                int currentImage = 0;
                for(int i = 0; i < rows; i++)
                {
                    //For each row...
                    float currentWidth = 15;
                    for (int j = 0; j < max; j++)
                    {
                        //For each image in row...
                        drawing.DrawImage(tImages[currentImage], currentWidth, currentHeight, tImgWidthScales[currentImage], tImgHeightScales[currentImage]);
                        currentImage++;
                        currentWidth += tImgWidthScales[currentImage];
                    }
                    currentHeight += tImgHeightScales[currentImage - max];
                }
                currentHeight += 20;
                */
            }
            float retweetStart = currentHeight - 10;
            //IF RETWEET
            if (rUsername != null)
            {
                drawing.DrawImage(iRPP, 25, currentHeight, 20, 20);
                drawing.DrawImage(circlePic, 25, currentHeight, 20, 20);
                drawing.DrawString(rName, fBoldNormalText, textBrush, 50, currentHeight);
                drawing.DrawString(rUsername, fNormalText, grayBrush, rNameSize.Width + 55, currentHeight);
                currentHeight += 25;
                //IF RETWEET TEXT
                if (rText != null)
                {
                    drawing.DrawString(rText, fNormalText, textBrush, new Rectangle(25, (int)currentHeight, 546, (int)rTextSize.Height));
                    currentHeight += (15 + rTextSize.Height);
                }
                //IF RETWEET IMAGE
                if (rImages.Count != 0)
                {
                    currentHeight = DrawImages(rImages, max, drawing, currentHeight, rImgWidthScales, rImgHeightScales, 25);
                }
                drawing.DrawPath(new Pen(Color.FromArgb(199, 199, 199), 1), RoundedRect(new Rectangle(15, (int)retweetStart,
                    568, ((int)currentHeight - (int)retweetStart)), 5));
            }

            drawing.Save();
            textBrush.Dispose();
            drawing.Dispose();
            circlePic.Dispose();
            if(tImages.Count != 0)
            {
                foreach(Image img in tImages)
                {
                    img.Dispose();
                }
            }
            if(rUsername != null)
            {
                iRPP.Dispose();
                if (rImages.Count != 0)
                {
                    foreach(Image img in rImages)
                    {
                        img.Dispose();
                    }
                }
            }
            iPP.Dispose();
            grayBrush.Dispose();

            Random rand = new Random();
            oImg.Save("C:/Users/William/source/repos/VibeAttack/VibeAttack/Image/Unused/image1-" + rand.Next(100) + ".png", ImageFormat.Png);
            oImg.Dispose();

        }

        private float DrawImages(List<Image> images, int max, Graphics drawing, float currentHeight, List<float> widths, List<float> heights, float currentWidth)
        {
            float tmpCurrWidth = currentWidth;
            int rows = (int)Math.Ceiling((decimal)images.Count / max);
            int currentImage = 0;
            float itemsLeft = images.Count;
            for (int i = 0; i < rows; i++)
            {
                //For each row...
                for (int j = 0; j < max; j++)
                {
                    //For each image in row...
                    drawing.DrawImage(images[currentImage], tmpCurrWidth, currentHeight, widths[currentImage], heights[currentImage]);
                    tmpCurrWidth += widths[currentImage];
                    currentImage++;
                    if (currentImage >= images.Count)
                    {
                        break;
                    }
                }
                tmpCurrWidth = currentWidth; 
                currentHeight += heights[i * max];
            }
            return currentHeight += 20;
        }

        //Returns a list of the Height OR Width of images
        private List<string> CalculateImageDimensions(List<Image> images, int max, float maxWidth)
        {
            List<string> r = new List<string>();
            int items = images.Count;
            int rows = (int)Math.Ceiling((decimal)items / max);
            int currentImages = 0;
            //IMPERFECT GRID
            if(items % max != 0)
            {
                //COMPLETE AND IMCOMPLETE ROWS
                if(items > max)
                {
                    //COMPLETE ROWS
                    for(int i = 0; i < (rows - 1); i++)
                    {
                        List<Image> imageRow = new List<Image>();
                        for(int j = currentImages; j < (currentImages + max); j++)
                        {
                            imageRow.Add(images[j]);
                        }
                        r.AddRange(CalcRowImages(imageRow, max, maxWidth));
                        currentImages += max;
                    }
                    //INCOMPLETE ROW
                    List<Image> imageRowInc = new List<Image>();
                    for(int j = currentImages; j < items; j++)
                    {
                        imageRowInc.Add(images[j]);
                    }
                    r.AddRange(CalcRowImages(imageRowInc, (items - currentImages), maxWidth));
                }
                if(items < max)
                {
                    List<Image> imageRow = new List<Image>();
                    for (int j = currentImages; j < items; j++)
                    {
                        imageRow.Add(images[j]);
                    }
                    r.AddRange(CalcRowImages(imageRow, items, maxWidth));
                }
            }
            //PERFECT GRID
            if(items % max == 0)
            {
                //COMPLETE ROWS
                for (int i = 0; i < rows; i++)
                {
                    List<Image> imageRow = new List<Image>();
                    for (int j = currentImages; j < (currentImages + max); j++)
                    {
                        imageRow.Add(images[j]);
                    }
                    r.AddRange(CalcRowImages(imageRow, max, maxWidth));
                    currentImages += max;
                }
            }
            return r;
        }

        private List<string> CalcRowImages(List<Image> imageRow, int max, float maxWidth)
        {
            List<string> tmpResults = new List<string>();
            List<float> scaleFactors = new List<float>();
            imageRow = imageRow.OrderByDescending(o => o.Height).ToList<Image>();
            //Scale factor to match tallest image
            for (int j = 1; j < max; j++)
            {
                scaleFactors.Add((float)imageRow[0].Height / (float)imageRow[j].Height);
            }
            List<float> tmpWidth = new List<float>();
            List<float> tmpHeight = new List<float>();
            //Setting other images dimensions to new scale
            for (int j = 0; j < scaleFactors.Count; j++)
            {
                tmpWidth.Add((float)imageRow[j + 1].Width * scaleFactors[j]);
                tmpHeight.Add((float)imageRow[j + 1].Height * scaleFactors[j]);
            }
            float totalWidth = imageRow[0].Width;
            for (int j = 0; j < tmpWidth.Count; j++)
            {
                totalWidth += tmpWidth[j];
            }
            float scaleFactor = maxWidth / totalWidth;
            tmpResults.Add(((float)imageRow[0].Width * scaleFactor) + "x" +
                        ((float)imageRow[0].Height * scaleFactor));
            for (int j = 0; j < tmpWidth.Count; j++)
            {
                tmpResults.Add(((float)tmpWidth[j] * scaleFactor) + "x" +
                        ((float)tmpHeight[j] * scaleFactor));
            }
            return tmpResults;
        }

        //Returns the extra height of bitmap image needed to fit rows of images
        private float CalculateTotalImagesHeight(List<float> heights, int max)
        {
            float r = 0;
            for(int i = 0; i < heights.Count; i += max)
            {
                r += heights[i];
            }
            return r;
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}