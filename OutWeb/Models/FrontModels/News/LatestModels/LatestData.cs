using OutWeb.Models.FrontModels.News.EventLatestModels;
using OutWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.LatestModels
{
    public class LatestData
    {
        public int ID { get; set; }
        public Dictionary<int, string> TypeInfo { get; set; }
        public string Title { get; set; }
        public string Img { get; set; }
        public string ListTitleUrl { get; set; }
        public string ContentUrl { get; set; }
        public string PublishDateString { get; set; }
        public string Remark { get; set; }
        public int Sort { get; set; }
        public string BD_DTString { get; set; }
        public ListKind DataType { get; set; }

    }
}