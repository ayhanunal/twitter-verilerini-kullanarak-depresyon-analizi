using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using TwitterAPI;
using TwitterAPIPersons;
using HtmlAgilityPack;
using System.Collections;

namespace GetTweetsOnTwitter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static int Tweet_Sayıcı(HtmlAgilityPack.HtmlDocument doc)
        {
            int count = 0;
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div");
            if (nodes != null)
            {
                foreach (var item in nodes)
                {
                    if (item.Attributes["class"] != null)
                        if (item.Attributes["class"].Value.StartsWith("tweet js-stream-tweet"))
                        {
                            count++;
                        }
                }
            }
            return count;
        }

        FileStream fs;
        FileStream fs2;
        StreamReader sr;
        StreamWriter sw;
        Uri url;
        HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
        string adet;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Hide();
            fs = new FileStream("C:/Users/" + Environment.UserName + "/infotwitter_gt.txt", FileMode.Open);
            sr = new StreamReader(fs);
            string aramaKelimesi = sr.ReadLine();
            string başlangıçTarihi = sr.ReadLine();
            string bitişTarihi = sr.ReadLine();
            string hashtagDurumu = sr.ReadLine();
            adet = sr.ReadLine();
            webBrowser1.ScriptErrorsSuppressed = true;
            if (hashtagDurumu == "false")
            {
                if (başlangıçTarihi == "null" && bitişTarihi == "null")
                    url = new Uri("https://twitter.com/search?l=tr&q=" + aramaKelimesi + "&src=typd&lang=tr");
                else
                    url = new Uri("https://twitter.com/search?l=tr&q=" + aramaKelimesi + "%20since%3A" + başlangıçTarihi + "%20until%3A" + bitişTarihi + "&src=typd&lang=tr");
            }
            else
            {
                if (başlangıçTarihi == "null" && bitişTarihi == "null")
                    url = new Uri("https://twitter.com/search?l=tr&q=%23" + aramaKelimesi + "&src=typd&lang=tr");
                else
                    url = new Uri("https://twitter.com/search?l=tr&q=%23" + aramaKelimesi + "%20since%3A" + başlangıçTarihi + "%20until%3A" + bitişTarihi + "&src=typd&lang=tr");
            }
            sr.Close();
            fs.Close();
            webBrowser1.Navigate(url.AbsoluteUri);
            while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            document.LoadHtml(webBrowser1.DocumentText);
            if (Tweet_Sayıcı(document) >= Convert.ToInt32(adet))
            {
                fs2 = new FileStream("C:/Users/" + Environment.UserName + "/infotwitter_tweets.txt", FileMode.Create);
                sw = new StreamWriter(fs2);
                sw.Write(webBrowser1.DocumentText);
                sw.Close();
                fs2.Close();
                this.Close();
            }
            else
            {
                timer1.Start();
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            webBrowser1.Document.Window.ScrollTo(0, webBrowser1.Document.Body.ScrollRectangle.Height);
            document.LoadHtml(webBrowser1.DocumentText);
            if (Tweet_Sayıcı(document) >= Convert.ToInt32(adet))
            {
                fs2 = new FileStream("C:/Users/" + Environment.UserName + "/infotwitter_tweets.txt", FileMode.Create);
                sw = new StreamWriter(fs2);
                sw.Write(webBrowser1.DocumentText);
                sw.Close();
                fs2.Close();
                this.Close();
            }
        }
    }
}