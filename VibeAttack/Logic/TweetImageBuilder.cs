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
            string image = null;
            //IF TWEET IMAGE
            //IF VIDEO
            string video = null;
            string rVideo = null;
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
            string rImage = null;
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

                rMediaLocations = DownloadMedia(retweet, "rMedia");
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
            List<Image> tMedia = null;
            List<Image> rMedia = null;
            //List<MediaPlayer>
            Image iPP = Image.FromFile(@ppURL);
            Image iRPP = null;
            float rtImgHeightScale = 0;
            float tImgHeightScale = 0;
            float oImgHeight = 139;
            Image circlePic = Image.FromFile(path + "/circle-pic.png");

            List<float> tImgHeightScales = new List<float>();
            List<float> tImgWidthScales = new List<float>();
            List<float> rImgHeightScales = new List<float>();
            List<float> rImgWidthScales = new List<float>();

            //GET SIZE OF OUTPUT IMAGE
            if (text != null)
            {
                textSize = drawing.MeasureString(text, fLargeText, 568);  //Size of Large Text (Tweet)
                oImgHeight += textSize.Height;
            }
            if (mediaLocations != null)
            {
                foreach(string media in mediaLocations)
                {
                    if(media.Contains(".jpg"))
                    {
                        tMedia.Add(Image.FromFile(media));
                    }
                }
                //2 Images, 
                //
                //rows = (int)(Amount / Max) + (Amount % Max)
                //for each row
                tImgHeightScale = (568 / (float)tImage.Width) * tImage.Height;
                oImgHeight += tImgHeightScale;
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
                if (rImage != null)
                {
                    rtimg = Image.FromFile(rImage);
                    rtImgHeightScale = (568 / (float)rtimg.Width) * rtimg.Height;
                    oImgHeight += rtImgHeightScale;
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
            if (tImage != null)
            {
                drawing.DrawImage(tImage, 15, currentHeight, 568, tImgHeightScale);
                currentHeight += tImgHeightScale + 20;
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
                if (rImage != null)
                {
                    drawing.DrawImage(rtimg, 15, currentHeight, 568, rtImgHeightScale);
                    currentHeight += rtImgHeightScale;
                }
                drawing.DrawPath(new Pen(Color.FromArgb(199, 199, 199), 1), RoundedRect(new Rectangle(15, (int)retweetStart,
                    568, ((int)currentHeight - (int)retweetStart)), 5));
            }

            drawing.Save();
            textBrush.Dispose();
            drawing.Dispose();
            circlePic.Dispose();
            if(tImage != null)
            {
                tImage.Dispose();
            }
            if(rUsername != null)
            {
                iRPP.Dispose();
                if (rtimg != null)
                {
                    rtimg.Dispose();
                }
            }
            iPP.Dispose();
            grayBrush.Dispose();

            Random rand = new Random();
            oImg.Save("C:/Users/William/source/repos/VibeAttack/VibeAttack/Image/Unused/image1-" + rand.Next(100) + ".png", ImageFormat.Png);
            oImg.Dispose();
            
            //Profile Picture
            //49px x 49px
            //(15, 10) Pos

            //Display Name
            //(Width of Name) x 19px
            //(74, 16)

            //Username 
            //(Width of Username) x 19px
            //(74, 35)

            //Text
            //(Max 568px) x SizeF.H
            //(15, 69)

            //Retweet Rectangle

            //Retweet PP
            //20px x 20px
            //(25, (Height of Text + 109))

            //Retweet Name
            //(Width of Name) x 19
            //(50, (Height of Text + 109))

            //Retweet Username
            //(Width of Name) x 19
            //((55 + RTName Width), (Height of Text + 109)

            //Retweet Text
            //(Max 568px) x SifeF.H
            //(25, (Height of Text + 134)

            //Retweet Image
            //(Max 568) x Sale to Width
            //(15, Height of Text + Height of RText + 149)

        }

        //Returns a list of the Height OR Width of images
        private List<string> CalculateImageDimensions(string dimension, List<Image> images, int max, float maxWidth)
        {
            List<string> r = new List<string>();
            float items = images.Count;
            int rows = (int)Math.Ceiling(items / max);
            int currentImages = 0;
            if(items % max != 0)
            {
                if(items > max)
                {
                    for(int i = 0; i < (rows - 1); i++)
                    {
                        //Find largest image from first 3
                        List<Image> imageRow = new List<Image>();
                        List<float> scaleFactors = new List<float>();
                        for(int j = currentImages; j < (currentImages + max); j++)
                        {
                            imageRow.Add(images[j]);
                        }
                        imageRow = imageRow.OrderByDescending(o => o.Height).ToList<Image>();
                        for(int j = currentImages; j < (currentImages + (max - 1)); j++)
                        {
                            scaleFactors.Add(imageRow[currentImages].Height / imageRow[j + 1].Height);
                        }
                        List<float> tmpWidth = new List<float>();
                        List<float> tmpHeight = new List<float>();
                        for(int j = 0; j < scaleFactors.Count; j++)
                        {
                            tmpWidth.Add(imageRow[j + 1].Width * scaleFactors[j]);
                            tmpHeight.Add(imageRow[j + 1].Height * scaleFactors[j]);
                        }
                        float totalWidth = imageRow[0].Width;
                        for(int j = 0; j < tmpWidth.Count; j++)
                        {
                            totalWidth += tmpWidth[j];
                        }
                        float scaleFactor = maxWidth / totalWidth;
                        for(int j = 0; j < imageRow.Count; j++)
                        {
                            r.Add((imageRow[j].Width * scaleFactor) + "." +
                                    (imageRow[j].Height * scaleFactor));
                        }
                        currentImages += max;
                    }
                    //CHECK IF ABOVE CODE WORKS FOR ONLY COMPLETE ROWS
                    //IF SO, MAKE A NEW METHOD OUT OF IT TO CALL
                    //DO INCOMEPLETE ROW
                    //items - currentImages = remainder
                }
            }
            //if (items % max != 0) //IC
            //  items > max
            //      complete rows = rows - 1
            //          loop through, 
            //      last row is incomplete 
            //  items < max
            //      last row is incomeplete
            return null;
        }

        //Returns the extra height of bitmap image needed to fit rows of images
        private float CalculateTotalImagesHeight(List<float> heights)
        {

            return 0;
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