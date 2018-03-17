using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.AnnouncementLatest
{
    public class AnnouncementLatestFilter
    {
        /// <summary>
        /// 選取頁面
        /// </summary>
        public int CurrentPage { get; set; }

        public string LangCode { get; set; }
    }
}