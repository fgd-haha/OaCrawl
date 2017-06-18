using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using static System.Net.WebClient;
using MS.Internal.Xml.XPath;
using MS.Internal.Xml;
using HtmlAgilityPack;
using System.Web;

namespace OaCrawl
{
    class OnCrawl
    {
        private static int num = 0;
        static void Main(string[] args)
        {
            Console.WriteLine("----------------------------------------吉林大学校园通知爬虫程序----------------------------------------\n");
            Console.WriteLine("请输入开始日期(格式如:2017-06-14):  ");
//            var startDate = DateTime.Parse(Console.ReadLine());
            Console.WriteLine();
            Console.WriteLine("请输入结束日期(格式如:2017-06-14):  ");
//            var endDate = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("\n\n");
            Console.WriteLine("通知内容保存在当前目录下InformCrawled文件夹中");
            Console.WriteLine("附件保存在当前目录下AttachmentCrawled文件夹中\n\n\n");
            Console.WriteLine("----------------------------------------------开始------------------------------------------------------\n");
//            Search(startDate, endDate);
            
            Search(DateTime.Parse("2017-06-14"),DateTime.Parse("2017-06-14"));
            Console.WriteLine("----------------------------------------------结束------------------------------------------------------");
        }
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public static void Search(DateTime startDate, DateTime endDate)
        {
            foreach (DateTime day in EachDay(startDate, endDate))
            {
                Console.WriteLine(day.ToString("yyyy-MM-dd"));
                string dateUrl =
                    "https://oa.jlu.edu.cn/defaultroot/PortalInformation!jldxList.action?channelId=179577&searchnr=" + day.ToString("yyyy-MM-dd") + "&searchlx=3";
//                WebClient me = new WebClient();
//                string htm = me.DownloadString(dateUrl);

                HtmlWeb web = new HtmlWeb();
                HtmlDocument html = web.Load(dateUrl);

                var temps = html.DocumentNode.SelectNodes(".//div[@class = 'li rel']/a[@class = 'font14']");
                string mainUrl = "https://oa.jlu.edu.cn/defaultroot/";
                
                ArrayList urlList = new ArrayList();
                foreach (HtmlNode item in temps)
                {
                    urlList.Add((mainUrl + item.Attributes["href"].Value));
//                    Console.WriteLine(mainUrl + item.Attributes["href"].Value);
                }

                foreach (string url in urlList)
                {
                    Download(url);
                }
            }
        }

        public static void Download(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument html = web.Load(url);

            string contentTitle = html.DocumentNode.SelectSingleNode(".//div[@class = 'content_t']").InnerText;
            string content = html.DocumentNode.SelectSingleNode(".//div[@class = 'content_font fontsize immmge']").InnerText;
            content = content.Replace("&nbsp;", " ");
            string contentTime = html.DocumentNode.SelectSingleNode(".//div[@class = 'content_time']").InnerText;
            contentTime = contentTime.Replace("&nbsp;", " ");
            string contentDepartment = html.DocumentNode.SelectSingleNode(".//div[@class = 'content_time']/span").InnerText;

            string textName = contentTime + " " + contentDepartment + " " + contentTitle + ".txt";
            textName = textName.Replace('/', '-');

//            Console.WriteLine(contentTime + " " + contentTitle + " "+ content);
//            Console.WriteLine(textName);
            










        }
    }
}
