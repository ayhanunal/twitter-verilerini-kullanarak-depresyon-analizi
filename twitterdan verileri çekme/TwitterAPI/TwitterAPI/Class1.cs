using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using HtmlAgilityPack;
using System.Net;
using System.Xml.XPath;
using System.IO;
using TwitterAPIPersons;

namespace TwitterAPI
{
    public class TwitterApplication
    {
        HtmlDocument document;
        FileStream fs;
        StreamReader sr;
        FileStream fs2;
        StreamWriter sw;
        ArrayList tweets;
        Person person;

        public void InitTwitter(string seekWord, int count, bool hashtagState, string startDate, string endDate)
        {
            Console.WriteLine("Dosya işlemleri başlatıldı");

            fs2 = new FileStream("C:/Users/" + Environment.UserName + "/infotwitter_gt.txt", FileMode.Create);
            sw = new StreamWriter(fs2);
            sw.WriteLine(seekWord);
            if (startDate != null)
            {
                sw.WriteLine(startDate);
            }
            else sw.WriteLine("null");
            if (endDate != null)
            {
                sw.WriteLine(endDate);
            }
            else sw.WriteLine("null");
            if (hashtagState) sw.WriteLine("true");
            else sw.WriteLine("false");
            sw.WriteLine(count.ToString());
            sw.Close();
            fs2.Close();

            Console.WriteLine("Twitter dökümanı oluşturulmaya başlandı");

            System.Diagnostics.Process.Start("C:/Users/Ayhan/Desktop/programlama calisma/c#/Proje 1/GetTweetsOnTwitter/GetTweetsOnTwitter/bin/Debug/GetTweetsOnTwitter.exe");

            Console.WriteLine("Döküman okunuyor");

            if (File.Exists("C:/Users/" + Environment.UserName + "/infotwitter_tweets.txt")) File.Delete("C:/Users/" + Environment.UserName + "/infotwitter_tweets.txt");
            bool exception = true;
            while (exception)
            {
                try
                {
                    exception = false;
                    fs = new FileStream("C:/Users/" + Environment.UserName + "/infotwitter_tweets.txt", FileMode.Open);
                }
                catch
                {
                    exception = true;
                }
            }

            sr = new StreamReader(fs);
            document = new HtmlDocument();
            document.LoadHtml(sr.ReadToEnd());
            sr.Close();
            fs.Close();

            Console.WriteLine("Dökümandan tweetler çekilmeye başlandı");

            tweets = new ArrayList();
            HtmlDocument subDocument = new HtmlDocument();
            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//div");
            if (nodes != null)
            {
                foreach (var item in nodes)
                {
                    if (item.Attributes["class"] != null)
                        if (item.Attributes["class"].Value.StartsWith("tweet js-stream-tweet"))
                        {
                            person = new Person();
                            person.FullName = item.Attributes["data-name"].Value;
                            person.UserName = item.Attributes["data-screen-name"].Value;
                            person.TweetID = item.Attributes["data-tweet-id"].Value;
                            subDocument.LoadHtml(item.InnerHtml);
                            HtmlNodeCollection subNodes = subDocument.DocumentNode.SelectNodes("//p");
                            foreach (var item2 in subNodes)
                            {
                                if (item2.Attributes["class"] != null)
                                {
                                    if (item2.Attributes["class"].Value.StartsWith("TweetTextSize  js-tweet-text"))
                                    {
                                        person.TweetContent = item2.InnerText;
                                    }
                                }
                            }
                            HtmlNodeCollection subNodes2 = subDocument.DocumentNode.SelectNodes("//span");
                            foreach (var item3 in subNodes2)
                            {
                                if (item3.Attributes["class"] != null)
                                {
                                    if (item3.Attributes["class"].Value.StartsWith("_timestamp js-short-timestamp"))
                                    {
                                        person.TimeStamp = item3.InnerText;

                                    }
                                }
                            }
                            if (tweets.Count == count)
                                break;
                            else
                                tweets.Add(person);
                        }
                }
            }

            Console.WriteLine("İşlemler tamamlandı");
        }

        public ArrayList TweetsArrayToString(ArrayList tweets)
        {
            ArrayList list = new ArrayList();
            foreach (Person person in tweets)
            {
                string data = "Kullanıcı Adı: " + person.UserName + "\n" + "Kullanıcı İsmi: " + person.FullName + "\n" +
                    "Tweet: " + person.TweetContent + "\n" + "Geçen Zaman: " + person.TimeStamp;
                list.Add(data);
            }
            return list;
        }

        public ArrayList GetTweetsArray()
        {
            return tweets;
        }
    }
}