using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TechNews.Data
{
    public class RssFeedService
    {
        private readonly HttpClient _httpClient;

        public RssFeedService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Article>> GetRssFeedAsync()
        {
            var feedUrl = "https://techxplore.com/rss-feed/breaking/machine-learning-ai-news/";
            var articles = new List<Article>();

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, feedUrl);
                request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var xdoc = XDocument.Parse(responseContent);

                foreach (var item in xdoc.Descendants("item"))
                {
                    var mediaThumbnail = item.Element(XName.Get("thumbnail", "http://search.yahoo.com/mrss/"));
                    var thumbnailUrl = mediaThumbnail?.Attribute("url")?.Value;

                    var article = new Article
                    {
                        Title = item.Element("title")?.Value,
                        Description = item.Element("description")?.Value,
                        Link = item.Element("link")?.Value,
                        Category = item.Element("category")?.Value,
                        PubDate = item.Element("pubDate")?.Value,
                        Thumbnail = thumbnailUrl
                    };

                    articles.Add(article);
                }
            }
            catch (HttpRequestException e)
            {
                // Handle HTTP request errors
                Console.WriteLine($"Error fetching RSS feed: {e.Message}");
            }
            catch (Exception e)
            {
                // Handle other errors
                Console.WriteLine($"Error parsing XML: {e.Message}");
            }

            return articles;
        }
    }

    public class Article
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Category { get; set; }
        public string PubDate { get; set; }
        public string Thumbnail { get; set; }
    }
}
