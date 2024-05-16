using HtmlAgilityPack;
using MouseTagProject.DTOs;
using MouseTagProject.Interfaces;

namespace MouseTagProject.Services
{
    public class LinkedInService : ILinkedInService
    {
        public NameAndSurnameDto ScrapeSerp(string linkedInLink)
        {
            NameAndSurnameDto candidate = new NameAndSurnameDto();
            string link = string.Empty;
            if (linkedInLink.Contains('%'))
            {
                link = "https://www.google.com/search?q=" + Uri.UnescapeDataString(linkedInLink).Replace("https://www.", string.Empty) + " &num=1&start=1";
            }
            else
            {
                link = "https://www.google.com/search?q=" + linkedInLink.Replace("https://www.", string.Empty) + " &num=1&start=0";
            }

            HtmlWeb web = new HtmlWeb();
            web.UserAgent = "user-agent=Mozilla/5.0" +
                " (Windows NT 10.0; Win64; x64)" +
                " AppleWebKit/537.36 (KHTML, like Gecko)" +
                " Chrome/74.0.3729.169 Safari/537.36";

            var htmlDoc = web.Load(link);

            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='yuRUbf']");

            if (nodes != null)
            {
                var title = nodes.Descendants("h3").FirstOrDefault().InnerText;

                string[] array = title.Split(' ');
                candidate.Name = array[0].Replace(",", string.Empty);
                candidate.Surname = array[1].Replace(",", string.Empty);
            }

            if (candidate.Name == null && candidate.Surname == null)
            {
                link = "https://www.google.com/search?q=" + Uri.UnescapeDataString(linkedInLink).Replace("https://www.", string.Empty) + " &num=1&start=0";
                htmlDoc = web.Load(link);
                nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='yuRUbf']");

                if (nodes != null)
                {
                    var title = nodes.Descendants("h3").FirstOrDefault().InnerText;

                    string[] array = title.Split(' ');
                    candidate.Name = array[0].Replace(",", string.Empty);
                    candidate.Surname = array[1].Replace(",", string.Empty);
                }
            }

            if (!linkedInLink.Contains("linkedin"))
            {
                candidate.Name = null;
                candidate.Surname = null;
            }

            return candidate;
        }
    }
}
