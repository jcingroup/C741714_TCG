using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.AnnouncementLatest
{
    public class AnnouncementLatestContent
    {
        public string PreviousIDStr { get; set; }
        public string NextIDStr { get; set; }

        public AnnouncementLatestData Data { get; set; }

    }
}