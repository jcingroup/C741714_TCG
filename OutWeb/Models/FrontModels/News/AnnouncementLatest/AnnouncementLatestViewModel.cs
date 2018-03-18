using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OutWeb.Models.FrontModels.News.AnnouncementLatest
{
    public class AnnouncementLatestViewModel
    {
        AnnouncementLatestFilter m_filter = new AnnouncementLatestFilter();
        AnnouncementLatestResult m_result = new AnnouncementLatestResult();

        public AnnouncementLatestFilter Filter { get { return this.m_filter; } set { this.m_filter = value; } }
        public AnnouncementLatestResult Result { get { return this.m_result; } set { this.m_result = value; } }
    }
}