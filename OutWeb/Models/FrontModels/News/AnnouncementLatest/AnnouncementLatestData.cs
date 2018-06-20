using System.Collections.Generic;

namespace OutWeb.Models.FrontModels.News.AnnouncementLatest
{
    public class AnnouncementLatestData
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string PublishDateString { get; set; }
        public string Content { get; set; }
        public int Sort { get; set; }
        public string BD_DTString { get; set; }
        public Dictionary<int, string> CateIDInfo { get; set; }

        public string Img { get; set; }
    }
}