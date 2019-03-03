using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using TwitterAPI;
using TwitterAPIPersons;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace Basit_Veri_Madeni
{
    class Program
    {
        static TwitterApplication uygulama;

        static void Main(string[] args)
        {
        

            string aranan;
            bool hastag = false;
            Console.Write("İlgili Tweetin Çekileceği Kelime (Hashtag Arama İçin Kelimenin Başına # Koyunuz) :");
            string kelime = Console.ReadLine();
            char durum = kelime[0];
            if (durum.Equals("#"))
            {
                hastag = true;
                aranan = donustur(kelime);
            }
            aranan = kelime;
            Console.Write("Kaç Adet Tweet Çekilsin? :");
            int tweet_sayisi = Convert.ToInt32(Console.ReadLine());        
            uygulama = new TwitterApplication();
            uygulama.InitTwitter(aranan, tweet_sayisi, hastag, null, null);
            ArrayList tweetler = uygulama.GetTweetsArray();
            ArrayList stringler = uygulama.TweetsArrayToString(tweetler);
            int i = 1;
            foreach (string tweet in stringler)
            {
                
                Console.WriteLine("------------------------------");
                Console.WriteLine(i + ". Tweet");
                Console.WriteLine(tweet);
                i++;
            }
            MySqlConnection baglan = new MySqlConnection("Server=localhost;Database=proje;Uid=root;Pwd=123456");
            int j = 1;
            foreach(Person p in tweetler)
            {
                try
                {
                    baglan.Open();
                    string kadi = p.UserName, kismi = p.FullName, tweet = p.TweetContent;
                    string komut = "insert into twitter(kullanici_adi,kullanici_ismi,tweet) values('"+kadi+"','"+kismi+"','"+tweet+"')";
                    MySqlCommand kmt = new MySqlCommand(komut, baglan);
                    kmt.ExecuteNonQuery();
                    Console.WriteLine(j + ". kayıt başarıyla kayıt edildi");
                    j++;
                    baglan.Close();
                }
                catch(Exception err)
                {
                    Console.WriteLine("bir hata oluştu :"+err);
                    baglan.Close();
                }
            }

            Console.Read();
        }
        static private string donustur(string k)
        {
            string yeni = "";
            for(int i=1;i<k.Length;i++)
            {
                yeni = yeni + k[i];
            }
            return yeni;
        }
    }
}