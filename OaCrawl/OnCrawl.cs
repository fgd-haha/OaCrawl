using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace OaCrawl
{
    class OnCrawl
    {
        private static int num = 0;

        static void Main(string[] args)
        {
            Console.WriteLine(
                "----------------------------------------吉林大学校园通知爬虫程序----------------------------------------\n");
            Console.WriteLine("请输入开始日期(格式如:2017-06-14):  ");
            var startDate = DateTime.Parse(Console.ReadLine());
            Console.WriteLine();
            Console.WriteLine("请输入结束日期(格式如:2017-06-14):  ");
            var endDate = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("\n\n");
            Console.WriteLine("通知内容保存在(.\\OaCrawl\\bin\\Debug\\InformCrawled)文件夹中");
            Console.WriteLine("附件保存在(.\\OaCrawl\\bin\\Debug\\AttachmentCrawled)文件夹中\n\n\n");
            Console.WriteLine(
                "----------------------------------------------开始------------------------------------------------------\n");
            Search(startDate, endDate);
            Console.WriteLine(
                "----------------------------------------------结束------------------------------------------------------");
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
                Console.WriteLine("\n\n\n" + day.ToString("yyyy-MM-dd") + ":\n\n\n");
                num = 0;
                string dateUrl =
                    "https://oa.jlu.edu.cn/defaultroot/PortalInformation!jldxList.action?channelId=179577&searchnr=" +
                    day.ToString("yyyy-MM-dd") + "&searchlx=3";

                HtmlWeb web = new HtmlWeb();
                HtmlDocument html = web.Load(dateUrl);

                var temps = html.DocumentNode.SelectNodes(".//div[@class = 'li rel']/a[@class = 'font14']");
                string mainUrl = "https://oa.jlu.edu.cn/defaultroot/";

                ArrayList urlList = new ArrayList();
                try
                {
                    foreach (HtmlNode item in temps)
                    {
                        urlList.Add((mainUrl + item.Attributes["href"].Value));
                    }

                    foreach (string url in urlList)
                    {
                        Download(url);
                    }
                }
                catch (System.NullReferenceException e)
                {
                }
            }
        }

        public static async Task Download(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument html = web.Load(url);

            string contentTitle = html.DocumentNode.SelectSingleNode(".//div[@class = 'content_t']").InnerText;
            string content = html.DocumentNode.SelectSingleNode(".//div[@class = 'content_font fontsize immmge']")
                .InnerText;
            content = content.Replace("&nbsp;", " ");
            string contentTime = html.DocumentNode.SelectSingleNode(".//div[@class = 'content_time']").InnerText;
            contentTime = (contentTime.Replace("&nbsp;&nbsp;", "分 ")).Replace(":", "时");

            string textName = contentTime + " " + contentTitle + ".txt";
            textName = textName.Replace('/', '-');
            string textPath = "./InformCrawled/" + textName;
            Console.WriteLine(++num + " " + textName);
            if (!Directory.Exists("./InformCrawled") || !Directory.Exists("./AttachmentCrawled/"))
            {
                Directory.CreateDirectory("./InformCrawled");
                Directory.CreateDirectory("./AttachmentCrawled");
            }

            using (StreamWriter sw = new StreamWriter(textPath))
            {
                sw.Write(content);
            }

            string InformationId = html.DocumentNode
                .SelectSingleNode(".//body/div/script[@language = 'JavaScript']/text()").InnerText;
            InformationId = Regex.Match(InformationId, @"informationId= '\d+'").ToString();
            InformationId = Regex.Match(InformationId, @"\d+").ToString();
            var Nodes = html.DocumentNode.SelectNodes(".//div[@class = 'news_aboutFile']//span");

            foreach (var node in Nodes)
            {
                string id = node.Attributes["id"].Value;
                string title = node.Attributes["title"].Value;

                var client = new WebClient();
                string urlGetRes = "https://oa.jlu.edu.cn/defaultroot/rd/jldx/BASEEncoderAjax.jsp?res=" + id + '@' +
                                   title + '@' + InformationId;
                string res = client.DownloadString(urlGetRes);
                res = (res.Replace("\n", "")).Trim();
                string urlDownload = "https://oa.jlu.edu.cn/defaultroot/rd/attachdownload.jsp?res=" + res;
                string attachPath = "./AttachmentCrawled/" + contentTime + ' ' + title;
                client.DownloadFile(urlDownload, attachPath);
                Console.WriteLine("     " + title);
            }
        }
    }
}